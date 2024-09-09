using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    [SerializeField]
    private float collisionTraumaAmount;

    [SerializeField]
    private LayerMask traumaLayers;

    [SerializeField]
    private SpaceshipNoiseMovement spaceshipNoise;


    void OnCollisionEnter(Collision other)
    {
        if(traumaLayers == (traumaLayers | (1 << other.gameObject.layer)))
        {
            spaceshipNoise.AddTrauma(collisionTraumaAmount);
        }
    }
}
