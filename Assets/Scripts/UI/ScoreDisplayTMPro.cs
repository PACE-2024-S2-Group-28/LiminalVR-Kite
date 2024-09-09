using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScoreDisplayTMPro : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro scoreText;

    [SerializeField]
    private string scorePrefix;

    private int displayScore;

    void Start()
    {
        GameManager.Instance.Action_OnScoreChanged = UpdateScoreDisplay;
        UpdateScoreDisplay(0);
    }


    public void UpdateScoreDisplay(int score)
    {
        displayScore = score;
        scoreText.text = scorePrefix + displayScore;
    }
}
