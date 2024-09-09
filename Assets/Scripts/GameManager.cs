using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }

    [SerializeField]
    public AsteroidSpawner asteroidSpawner;

    // Events
    public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        RockDestroyer.SEvent_RockDestroyed.AddListener(HandleAsteroidDestruction);
    }

    public void HandleAsteroidDestruction(bool isGoldAsteroid)
    {
        if (isGoldAsteroid)
        {
            UpdateScore(200);
        }
        else
        {
            UpdateScore(5);
        }
    }

    void UpdateScore(int scoreAdd)
    {
        Score += scoreAdd;
        OnScoreChanged.Invoke(Score);
        Debug.Log($"Score updated: {Score}");
    }

    public void AdjustAsteroidSpeed(float multiplier)
    {
        asteroidSpawner.asteroidSpeedRange = new Vector2(asteroidSpawner.asteroidSpeedRange.x * multiplier, asteroidSpawner.asteroidSpeedRange.y * multiplier);
    }
}
