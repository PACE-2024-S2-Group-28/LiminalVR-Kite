using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScaling : MonoBehaviour
{
    float totalGameTime => AsteroidGameManager.Instance.GameLength;
    
    [SerializeField]
    private float 
        minScale = .2f,
        maxScale = 1.2f;

    private void Update()
    {
        float gameProgress = Mathf.Clamp01(Time.time / totalGameTime);
        transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, gameProgress);
    }
}
