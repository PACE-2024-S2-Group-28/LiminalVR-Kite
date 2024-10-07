using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldAsteroidMaterialAnimation : MonoBehaviour
{
    [SerializeField]
    private Renderer meterRenderer;


    [SerializeField]
    private float meterOffsetMax;

    public void Update()
    {
        meterRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.5f, osY));
    }
}
