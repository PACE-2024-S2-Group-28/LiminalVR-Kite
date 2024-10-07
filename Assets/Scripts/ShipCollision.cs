using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

public class ShipCollision : MonoBehaviour
{
    [SerializeField]
    private float collisionTraumaAmount;

    [SerializeField]
    private LayerMask traumaLayers;

    [SerializeField]
    private SpaceshipNoiseMovement spaceshipNoise;
    [SerializeField] private GameObject collisionParticles;

    [SerializeField]
    private SoundScripObj sfxShipHit;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != null && traumaLayers == (traumaLayers | (1 << other.gameObject.layer)))
        {
            spaceshipNoise.AddTrauma(collisionTraumaAmount);

            //sfx
            sfxShipHit?.Play(wPos: other.transform.position);
        }

        // Collision particlses
        if (collisionParticles != null)
        {
            ContactPoint contact = other.contacts[0];
            Instantiate(collisionParticles, contact.point, Quaternion.identity);
        }
    }
}
