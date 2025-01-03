﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    public float Speed { set{speed = value;} get { return speed; } }
    [SerializeField] private float timeAlive = 3;
    public float TimeAlive { set {timeAlive = value;}}

    [SerializeField]
    private float hitForceMagnitude = 3f;

    [SerializeField]
    private Rigidbody rb;
    public Rigidbody Rb { get { return rb; } }

    private float timer;
    private Gun gun = null;
    public Gun Gun { set{ gun = value; firedBy = gun.transform;}}
    private Transform firedBy;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    public void StartVelocity()
    {
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update() //move the bullet
    {
        //transform.Translate(Vector3.forward * speed * Time.deltaTime); //move it

        //after a certain time, the bullet fades out and retuns to the gun's list of bullets to fire
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Miss();
        }
    }

    private void Miss()
    {
        //fading out still to come
        if (gun != null)
        {
            ReturnToGun();
        }
    }

    public void ResetTimer()
    {
        timer = timeAlive;
    }

    private Tag[] collisionTags = new Tag[]{Tag.Bullet, Tag.Forcefield};
    private void OnTriggerEnter(Collider other)
    {

        if (!firedBy.IsChildOf(other.transform) && !FakeTag.CheckTags(other.gameObject, collisionTags)) { //don't collide with what fired you
            //play some particle effect, and destroy meteors
            gun.TriggerSparks(transform.position);
            var rockScript = other.transform.parent.GetComponent<RockDestroyer>();
            if (rockScript != null) {
                //is asteroid
                rockScript.ChangeRock(forceDir: transform.forward, hitPos: transform.position);
                AsteroidGameManager.Instance.HandleAsteroidDestruction(FakeTag.CheckTag(other.gameObject, Tag.GoldAsteroid));
                //only collide with rocks. Pass through ship (otherwise turret bullets collide with the turret)

            } else {
                //check if is asteroid pieces
                var rockPieceS = other.GetComponent<RockPiece>();
                rockPieceS?.DestroyRockPiece();
            }


            if (gun != null) {
                ReturnToGun();
            }
        }

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!firedBy.IsChildOf(collision.transform))
    //    { //don't collide with what fired you
    //        //play some particle effect, and destroy meteors

    //        var rockScript = collision.collider.transform.parent.GetComponent<RockDestroyer>();
    //        //if(rockScript==null && collision.transform.parent!=null) {
    //        //    rockScript = collision.transform.parent.GetComponent<RockDestroyer>();
    //        //}
    //        if (rockScript != null)
    //        {
    //            rockScript.ChangeRock(forceDir: transform.forward, forceMag: collision.impulse.magnitude * hitForceMagnitude, hitPos: transform.position);
    //            //only collide with rocks. Pass through ship (otherwise turret bullets collide with the turret)

    //        }

    //        if (gun != null)
    //        {
    //            ReturnToGun();
    //        }
    //    }
    //}

    private void ReturnToGun()
    {
        rb.velocity = Vector3.zero;
        transform.position = gun.transform.position;
        gameObject.SetActive(false); //disable the bullet
        gun.BulletReturned(this); //let gun know this bullet is available
    }

    [SerializeField]
    private TrailRenderer trail;
    [SerializeField]
    private float trailTime = .5f;

    private void OnDisable()
    {
        trail.Clear();    
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(ResetTrail());
    }

    private IEnumerator ResetTrail()
    {
        trail.time = 0;
        yield return null;
        trail.time = trailTime;
    }
}
