using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int score = 0;
    [SerializeField, ReadOnly] private int highScore = 0;
    
    [SerializeField, MinMaxSlider(0.5f, 2f)]
    private Vector2 difficultySpeedMultiplierRange = new Vector2(1f, 1.5f);
    
    public UnityEvent<int> OnScoreChange = new UnityEvent<int>();

    private const int RedAsteroidScore = 5;
    private const int GoldAsteroidScore = 200;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;
        }
        OnScoreChange.Invoke(score);
    }

    public void AdjustDifficulty(float level)
    {
        float multiplier = Mathf.Lerp(difficultySpeedMultiplierRange.x, difficultySpeedMultiplierRange.y, level);
        AsteroidSpawner.Instance.UpdateSpeedMultiplier(multiplier);
    }
    public void HandleAsteroidDestruction(bool isGoldAsteroid)
    {
        int scoreBonus = isGoldAsteroid ? GoldAsteroidScore : RedAsteroidScore;
        AddScore(scoreBonus);
    }
}
