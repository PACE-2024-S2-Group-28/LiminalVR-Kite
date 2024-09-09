using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class UIPanelTester : MonoBehaviour
{
    [SerializeField]
    private UpgradeBarController barController;
    [SerializeField]
    private ScoreDisplayTMPro scoreDisplay;

    void Update()
    {
        scoreDisplay.UpdateScore(Mathf.FloorToInt(Time.time));
        barController.UpdateBarFill(Time.time%1);
    }
}
