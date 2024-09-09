using UnityEngine;
using UnityEngine.Events;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    private static int score;
    public static int Score => score;

    [SerializeField]
    private int upgradeThreshold;

    [SerializeField]
    private AsteroidSpawner asteroidSpawner;
    public AsteroidSpawner Spawner => asteroidSpawner;

    // Events
    public Action<int> Action_OnScoreChanged;

    public Action<float> Action_IncrementUpgradeProgress;

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

        UpdateUpgradeProgress(score);
    }

    void UpdateUpgradeProgress(int s)
    {
        float floatScore = s;
        float floatThreshold = upgradeThreshold;
        Action_IncrementUpgradeProgress?.Invoke((floatScore%floatThreshold)/floatThreshold);
    }

    public void AdjustAsteroidSpeed(float multiplier)
    {
        asteroidSpawner.AsteroidSpeedRange = new Vector2(asteroidSpawner.AsteroidSpeedRange.x * multiplier, asteroidSpawner.AsteroidSpeedRange.y * multiplier);
    }
}
