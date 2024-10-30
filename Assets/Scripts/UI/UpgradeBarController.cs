using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UpgradeBarController : MonoBehaviour
{
    [SerializeField][Range(0,1)]
    private float fillAmount;

    private Vector3 fillV3;

    [SerializeField]
    private Transform barFillTransform;

    [SerializeField]
    private Slider upgradeSlider;

    void Start()
    {
        AsteroidGameManager.Instance.Action_IncrementUpgradeProgress += UpdateBarFill;
        UpdateBarFill(0);
    }
    public void UpdateBarFill(float fill)
    {
        fillAmount = Mathf.Clamp01(fill);
        upgradeSlider.value = fillAmount;

        //fillV3 = new Vector3(fillAmount, 1, 1);
        //barFillTransform.localScale = fillV3;
    }
}
