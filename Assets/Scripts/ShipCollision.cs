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
    [SerializeField] private GameObject collisionParticles;

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
        if (other.gameObject != null && traumaLayers == (traumaLayers | (1 << other.gameObject.layer))) {
            spaceshipNoise.AddTrauma(collisionTraumaAmount);

            //sfx
            sfxShipHit?.Play(wPos: other.transform.position);
        }

        if (other.gameObject.tag == "GoldAsteroid" || other.gameObject.tag == "Rock") {
            RockDestroyer rockDestroyer = other.transform.parent.GetComponent<RockDestroyer>();
            if (rockDestroyer != null) {
                rockDestroyer.ChangeRock();
            }
        }
        OnShipHitByAsteroid.Invoke();
        // Destroy(other.gameObject);

        //// Collision particlses
        //if (collisionParticles != null) {
        //    ContactPoint contact = other.contacts[0];
        //    Instantiate(collisionParticles, contact.point, Quaternion.identity);
        //}

        RockParticleManager.PlayRockParticlesAt(other.contacts[0].point);
    }
}
