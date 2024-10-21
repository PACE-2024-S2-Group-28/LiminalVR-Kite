using UnityEngine;
using UnityEngine.Events;
using System;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;

using Liminal.SDK.Core;
using Liminal.Core.Fader;

public class AsteroidGameManager : MonoBehaviour
{
    private static AsteroidGameManager instance;
    public static AsteroidGameManager Instance => instance;

    private static int score;
    public static int Score => score;

    [SerializeField]
    private int upgradeThreshold;
    private int upgradeCount = 0; //so it knows how many you've unlocked, thus the next threshold to cross 
    
    [SerializeField]
    private AsteroidSpawner asteroidSpawner;
    public AsteroidSpawner Spawner => asteroidSpawner;

    // Events
    public Action<int> Action_OnScoreChanged;

    public UnityEvent UpgradeUnlocked;

    public Action<float> Action_IncrementUpgradeProgress;

    [SerializeField]
    private float totalGameTime = 300;
    public float GameLength => totalGameTime;

    [SerializeField]
    [MinMaxSlider(0f, 20f)]
    private Vector2 minMaxSpawnRate;

    [SerializeField]
    private AnimationCurve spawnCurve;
    public AnimationCurve DifficultyCurve => spawnCurve;

    [System.Serializable]
    public class GoldenAsteroidData {
        public float spawnTime;
        public Vector3 position;

        public GoldenAsteroidData(float progress, Vector3 pos) {
            spawnTime = progress;
            position = pos;
        }
    }

    [SerializeField]
    private List<GoldenAsteroidData> goldenAsteroidData = new List<GoldenAsteroidData>();
    public GoldenAsteroidData[] GoldSpawns => goldenAsteroidData.ToArray();

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private int currGoldIdx = 0;

    private void Update()
    {
        
        float newSpawnRate = Mathf.Lerp(minMaxSpawnRate.x, minMaxSpawnRate.y, SampleDifficultyCurve());
        asteroidSpawner.AdjustSpawnRate(newSpawnRate);

        if(Time.time >= goldenAsteroidData[currGoldIdx].spawnTime) {
            Debug.Log(String.Format("Spawning idx {0} gold asteroid", currGoldIdx));
            asteroidSpawner.SpawnAsteroid(true, goldenAsteroidData[currGoldIdx++].position);
        }
    }

    public float SampleDifficultyCurve()
    {
        float gameProgress = Time.time / totalGameTime;
        return spawnCurve.Evaluate(gameProgress);
    }

    public void RecordGoldenAsteroidSpawn(Vector3 position)
    {
        float gameProgress = Time.time / totalGameTime;
        goldenAsteroidData.Add(new GoldenAsteroidData(gameProgress, position));
        Debug.Log($"Golden Asteroid spawned at {position} - {gameProgress * 100}% through game.");
    }

    void Start()
    {
        //RockDestroyer.SEvent_RockDestroyed.AddListener(HandleAsteroidDestruction); //this function needs to change or have seperate function to trigger on event, or new event
    }

    public void HandleAsteroidDestruction(bool isGoldAsteroid)
    {
        if (isGoldAsteroid) {
            UpdateScore(90);
        }
        else {
            UpdateScore(5);
        }
    }

    void UpdateScore(int scoreAdd)
    {
        score += scoreAdd;
        Action_OnScoreChanged?.Invoke(score);
        Debug.Log($"Score updated: {score}");
        if (score >= upgradeThreshold * (upgradeCount + 1)) {
            print("Reached an upgrade");
            upgradeCount++;
            UpgradeUnlocked.Invoke();
        }

        UpdateUpgradeProgress(score);
    }

    void UpdateUpgradeProgress(int s)
    {
        float floatScore = s;
        float floatThreshold = upgradeThreshold;
        Action_IncrementUpgradeProgress?.Invoke((floatScore%floatThreshold)/floatThreshold);
    }

    private void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
            if (goldenAsteroidData == null || goldenAsteroidData.Count <= 0) return;

            GUIStyle style = new GUIStyle();
            style.fontSize = 18;
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            Gizmos.color = Color.yellow;
            foreach (var spawn in goldenAsteroidData) {
                Gizmos.DrawWireSphere(spawn.position, 3.5f);
                Handles.Label(spawn.position, "" + spawn.spawnTime, style);
            }
        #endif
    }

    public void EndExperience()
    {
        var fader = ScreenFader.Instance;
        fader.FadeTo(Color.black, 2f); // fade to "black out" after 2 sec
        ExperienceApp.End();
    }
}
