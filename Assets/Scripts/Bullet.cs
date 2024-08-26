using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float timeAlive = 3; 
    private float timer;
    private Gun gun;
    public Gun Gun {
        set {
            gun = value;
        }
    }

    // Update is called once per frame
    void Update() //move the bullet
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); //move it

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
        ReturnToGun();
    }

    private void ReturnToGun() {
        transform.position = gun.transform.position; 
        gameObject.SetActive(false); //disable the bullet
        gun.BulletReturned(this); //let gun know this bullet is available
    }
}
