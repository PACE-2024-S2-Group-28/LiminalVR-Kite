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

    private void OnEnable()
    {
        
    }

    void Update()
    {
        scoreDisplay.UpdateScoreDisplay(Mathf.FloorToInt(Time.time));
        barController.UpdateBarFill(Time.time%1);
    }
}
