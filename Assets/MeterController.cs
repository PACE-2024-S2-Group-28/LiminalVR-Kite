using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterController : MonoBehaviour
{
    [SerializeField]
    private Renderer meterRenderer;

    private float meterFill = 1f;

    [SerializeField]
    private float meterOffsetMax;

    void Start()
    {
        meterRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.5f, 1));
    }

    public void UpdateMeter(float m)
    {
        meterFill = m;

        float osY = meterOffsetMax * (1f - meterFill);
        meterRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.5f, osY));
    }
}
