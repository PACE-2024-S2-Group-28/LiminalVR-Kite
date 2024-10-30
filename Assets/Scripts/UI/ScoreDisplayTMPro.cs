using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class ScoreDisplayTMPro : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private string scorePrefix;

    private int displayScore;

    [SerializeField]
    private Slider gameProgressSlider;

    void Start()
    {
        AsteroidGameManager.Instance.Action_OnScoreChanged = UpdateScoreDisplay;
        UpdateScoreDisplay(0);
    }

    private void Update()
    {
        gameProgressSlider.value = AsteroidGameManager.Instance.GameProgress;
    }

    public void UpdateScoreDisplay(int score)
    {
        displayScore = score;
        scoreText.text = scorePrefix + displayScore;
    }
}
