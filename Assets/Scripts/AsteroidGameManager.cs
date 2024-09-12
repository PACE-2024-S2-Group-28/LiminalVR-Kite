using UnityEngine;
using UnityEngine.Events;
using System;
using NaughtyAttributes;

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
        Debug.Log("percentage through game " + Time.time / totalGameTime);
        Debug.Log("Spawn rate to set " + Mathf.Lerp(minMaxSpawnRate.x, minMaxSpawnRate.y, spawnCurve.Evaluate(Time.time / totalGameTime)));
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
