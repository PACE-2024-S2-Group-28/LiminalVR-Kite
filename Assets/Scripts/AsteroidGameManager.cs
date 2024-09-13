using UnityEngine;
using UnityEngine.Events;
using System;
using NaughtyAttributes;
using System.Collections.Generic;

public class AsteroidGameManager : MonoBehaviour
{
    private static AsteroidGameManager instance;
    public static AsteroidGameManager Instance => instance;

    private static int score;
    public static int Score => score;

    [SerializeField]
    private AsteroidSpawner asteroidSpawner;
    public AsteroidSpawner Spawner => asteroidSpawner;

    [SerializeField]
    private float totalGameTime = 300;
    [SerializeField]
    [MinMaxSlider(0f, 20f)]
    private Vector2 minMaxSpawnRate;
    [SerializeField]
    private AnimationCurve spawnCurve;

    [System.Serializable]
    public class GoldenAsteroidData {
        public float gameProgress;
        public Vector3 position;

        public GoldenAsteroidData(float progress, Vector3 pos) {
            gameProgress = progress;
            position = pos;
        }
    }

    public List<GoldenAsteroidData> goldenAsteroidData = new List<GoldenAsteroidData>();

    // Events
    public Action<int> Action_OnScoreChanged;

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

    private void Update()
    {
        float gameProgress = Time.time / totalGameTime;
        float spawnRate = Mathf.Lerp(minMaxSpawnRate.x, minMaxSpawnRate.y, spawnCurve.Evaluate(gameProgress));
        asteroidSpawner.AdjustSpawnTickRate(spawnRate);
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
            UpdateScore(200);
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
    }

    public void AdjustAsteroidSpeed(float multiplier)
    {
        asteroidSpawner.AsteroidSpeedRange = new Vector2(asteroidSpawner.AsteroidSpeedRange.x * multiplier, asteroidSpawner.AsteroidSpeedRange.y * multiplier);
    }
}
