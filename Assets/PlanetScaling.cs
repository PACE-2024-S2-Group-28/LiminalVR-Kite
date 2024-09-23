using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlanetScaling : MonoBehaviour
{
    //[SerializeField]
    private float totalGameTime = 300;
    [SerializeField]
    private float 
        minScale = .2f,
        maxScale = 1.2f;


    void Start()
    {
        totalGameTime = AsteroidGameManager.Instance.GameLength;

        transform.localScale = Vector3.one * minScale;
        transform.DOScale(maxScale, totalGameTime);
    }
}
