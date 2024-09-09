using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    private static float score;
    public static float Score => score;

    [SerializeField]
    public AsteroidSpawner asteroidSpawner;

    // Events
    public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        score += scoreAdd;
        OnScoreChanged.Invoke(score);
        Debug.Log($"Score updated: {score}");
    }

    public void AdjustAsteroidSpeed(float multiplier)
    {
        asteroidSpawner.asteroidSpeedRange = new Vector2(asteroidSpawner.asteroidSpeedRange.x * multiplier, asteroidSpawner.asteroidSpeedRange.y * multiplier);
    }
}
