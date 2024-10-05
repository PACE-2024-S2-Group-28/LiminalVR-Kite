using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;
using UnityEngine.Events;

public class ShipCollision : MonoBehaviour
{
    [SerializeField]
    private float collisionTraumaAmount;

    [SerializeField]
    private LayerMask traumaLayers;

    [SerializeField]
    private SpaceshipNoiseMovement spaceshipNoise;

    [SerializeField]
    private SoundScripObj sfxShipHit;

    public UnityEvent OnShipHitByAsteroid;

    void Start()
    {
        if (OnShipHitByAsteroid == null)
        OnShipHitByAsteroid = new UnityEvent();
    }   

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject != null && traumaLayers == (traumaLayers | (1 << other.gameObject.layer))) {
            spaceshipNoise.AddTrauma(collisionTraumaAmount);
            
            //sfx
            sfxShipHit?.Play(wPos: other.transform.position);
        }

        if (other.gameObject.tag == "GoldAsteroid" || other.gameObject.tag == "Rock")
        {
            OnShipHitByAsteroid.Invoke();
        }
    }
}
