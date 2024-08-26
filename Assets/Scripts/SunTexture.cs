using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SunTexture : MonoBehaviour
{
    [SerializeField]
    private Transform sunLightT;

    [SerializeField]
    private float angularSize = 3f;

    private void Start()
    {
        PlaceSunQuad();
    }

    private void Update()
    {
        PositionSunQuad();
    }

    [Button]
    public void PlaceSunQuad()
    {
        PositionSunQuad();
        RotateSunQuad();
        SetSunQuadSize();
    }

    public void PositionSunQuad()
    {
        var mainCam = Camera.main;
        transform.position = mainCam.transform.position - sunLightT.forward * (mainCam.farClipPlane-Mathf.Epsilon);
    }

    public void RotateSunQuad()
    {
        transform.rotation = sunLightT.rotation;
    }

    void SetSunQuadSize()
    {
        // Get the distance of the quad from the camera
        float distance = Camera.main.farClipPlane-Mathf.Epsilon;

        float size = Mathf.Deg2Rad * angularSize;
        // Calculate the height of the quad in world units based on the desired angular size
        float quadHeight = 2.0f * distance * Mathf.Tan(size / 2.0f);

        // Apply the calculated scale to the quad
        transform.localScale = new Vector3(quadHeight, quadHeight, 1.0f);
    }
}
