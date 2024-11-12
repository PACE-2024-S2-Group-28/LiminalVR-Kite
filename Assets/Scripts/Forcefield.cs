using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

public class Forcefield : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem hitPSys;

    [SerializeField]
    private SoundScripObj hitSFX;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private Tag[] collisionTags = {Tag.Asteroid, Tag.AsteroidPieces};

    private void OnCollisionEnter(Collision collision)
    {
        if(FakeTag.CheckTags(collision.collider.gameObject, collisionTags))
        {
            Vector3 hitPos = collision.GetContact(0).point;

            rend.material.SetFloat("_HitTime", Time.time);
            rend.material.SetVector("_HitPos", hitPos);

            hitSFX?.Play(wPos: collision.transform.position);
            hitPSys.transform.position = hitPos;
            hitPSys.Play();
        }
        
    }
}
