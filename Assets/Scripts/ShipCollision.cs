using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;
using UnityEngine.Events;

public class ShipCollision : MonoBehaviour
{
    [SerializeField]
    AsteroidGameManager gameManager;

    [SerializeField]
    private float collisionTraumaAmount;

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
        if (other.gameObject != null && FakeTag.CheckTags(other.collider.gameObject, FakeTag.AllAsteroidTags)) {
            spaceshipNoise.AddTrauma(collisionTraumaAmount);

            //sfx
            sfxShipHit?.Play(wPos: other.transform.position);

            RockDestroyer rockDestroyer = other.transform.parent.GetComponent<RockDestroyer>();
            if (rockDestroyer != null) {
                rockDestroyer.ChangeRock();
            }

            //trigger difficulty
            gameManager.AsteroidTriggerDyanmicDifficulty();
            OnShipHitByAsteroid.Invoke();

            RockParticleManager.PlayRockParticlesAt(other.contacts[0].point);
            
        }
        
    }
}
