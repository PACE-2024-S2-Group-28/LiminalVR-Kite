using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSock : MonoBehaviour
{
    [SerializeField]
    private bool DEBUG = false;

    [SerializeField]
    private MeshRenderer meshR;
    MaterialPropertyBlock matBlock;

    private void Start()
    {
        meshR = GetComponentInChildren<MeshRenderer>();
        matBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 totalWind = Wind.Instance.GetGlobalWind + Wind.GetLocalWind(transform.position);
        transform.forward = totalWind.normalized;
        transform.localScale = new Vector3(1f, 1f, totalWind.magnitude);

        if(!DEBUG) {
            return;
        }

        meshR.GetPropertyBlock(matBlock);
        var colour = new Color(totalWind.x, 0, totalWind.z) / Mathf.Max(totalWind.x, totalWind.z);
        matBlock.SetColor("_Color", colour);
        meshR.SetPropertyBlock(matBlock);
    }
}
