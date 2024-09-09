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

    public void UpdateScore(int score)
    {
        displayScore = score;
        scoreText.text = scorePrefix + displayScore;
    }
}
