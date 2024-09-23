using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlanetScaling : MonoBehaviour
{
    [SerializeField]
    private float totalGameTime = 300;
    [SerializeField]
    private float 
        minScale = 10,
        maxScale = 200;


    void Start()
    {
        transform.localScale = Vector3.one * minScale;
        transform.DOScale(maxScale, totalGameTime);
    }
}
