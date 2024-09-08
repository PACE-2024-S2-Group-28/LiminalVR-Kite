using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float timeAlive = 3;
    public float TimeAlive {
        set {
            timeAlive = value;
        }
    }

    [SerializeField]
    private float hitForceMagnitude = 3f;

    [SerializeField]
    private Rigidbody rb;
    
    private float timer;
    private Gun gun;
    public Gun Gun {
        set {
            gun = value;
        }
    }

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
        if (timer <= 0) {
            Miss();
        }
    }

    private void Miss() {
        //fading out still to come
        ReturnToGun();
    }

    public void ResetTimer() {
        timer = timeAlive;
    }

    private void OnCollisionEnter(Collision collision) {
        //play some particle effect, and destroy meteors
        var rockScript = collision.collider.transform.parent.GetComponent<RockDestroyer>();
        //if(rockScript==null && collision.transform.parent!=null) {
        //    rockScript = collision.transform.parent.GetComponent<RockDestroyer>();
        //}

        if(rockScript!=null) {
            rockScript.ChangeRock(forceDir: transform.forward, forceMag: collision.impulse.magnitude * hitForceMagnitude, hitPos: transform.position);
        }

        ReturnToGun();
    }

    private void ReturnToGun() {
        rb.velocity = Vector3.zero;
        transform.position = gun.transform.position; 
        gameObject.SetActive(false); //disable the bullet
        gun.BulletReturned(this); //let gun know this bullet is available
    }
}
