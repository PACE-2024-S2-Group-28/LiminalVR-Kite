using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldAsteroidMaterialAnimation : MonoBehaviour
{
    [SerializeField]
    private Renderer rockRenderer;
    private Material instanceMat;

    private float offset;

    [SerializeField]
    private float shineSpeed;

    public void Update()
    {
        GetMat();
        offset += Time.deltaTime * shineSpeed;
        instanceMat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }

    public void GetMat()
    {
        if(rockRenderer != null && instanceMat == null)
        {
            instanceMat = rockRenderer.material;
        }
    }

}
