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

    void Update()
    {
        displayScore = Mathf.FloorToInt(Time.time);

        scoreText.text = scorePrefix + displayScore;
    }
}
