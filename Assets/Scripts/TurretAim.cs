using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    [SerializeField] private float shootCooldown = 2; //how long between each shot
    private float rechargeTimer;
    [SerializeField] private float bulletLife = 3; //how many seconds a bullet exists before it returns to the gun
    [SerializeField] private float bulletSpeed = 5;
    [SerializeField] private float range;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform bulletParent; //where they appear in hierarchy
    
    private int totalBullets; //how many bullets to put into the stack of available bullets
    private Stack<Bullet> bulletsAvailable = new Stack<Bullet>();
    // Start is called before the first frame update
    void Start()
    {
        totalBullets =  Mathf.CeilToInt(bulletLife/shootCooldown) + 1; //work out how big to make the bullet pools
        rechargeTimer = shootCooldown;
        range = bulletSpeed * bulletLife;

        //make the bullet stacks
        for (int i = 0; i < totalBullets; i++) {
            Bullet newBullet = Instantiate(bullet, bulletParent); //make the bullet go forward
            newBullet.Turret = this;
            newBullet.TimeAlive = bulletLife;
            newBullet.Speed = bulletSpeed;
            newBullet.gameObject.SetActive(false);
            bulletsAvailable.Push(newBullet);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rechargeTimer -= Time.deltaTime;
        if (rechargeTimer <= 0) {
            FindFiringSolution();
        }
    }

    private void FindFiringSolution() { //checks for nearby asteroids, then rotates to immediately face the nearest one
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range); //check all nearby collisions
        Collider closestTarget = null;
        float closestDistance = range;
        float distanceToThis;
        rechargeTimer = shootCooldown;
        foreach (var hitCollider in hitColliders) {
            distanceToThis = Vector3.Distance(transform.position, hitCollider.transform.position);
            //check that the collider is closest of all checked so far, and that it's an asteroi
            if ((distanceToThis < closestDistance) && (hitCollider.gameObject.CompareTag("Rock"))) {
                closestTarget = hitCollider;
                closestDistance = distanceToThis;
            }
        }
        if (closestTarget != null) { //if you found something to shoot, work out the angle based on where the target is heading
            float timeToHit = closestDistance/bulletSpeed; //time until intercept
            Vector3 targetSpeed = closestTarget.attachedRigidbody.velocity; //speed of asteroid
            Vector3 targetPoint = closestTarget.transform.position + (targetSpeed * timeToHit); //position asteroid moves to during that time
            transform.GetChild(0).rotation = Quaternion.LookRotation(targetPoint - transform.position, bulletSpawnPoint.TransformDirection(Vector3.up));
            Debug.DrawRay(transform.position, targetPoint - transform.position, Color.yellow, shootCooldown);
            Debug.DrawRay(transform.position, closestTarget.transform.position - transform.position, Color.red, shootCooldown);
            Fire();
        }
        else {
            Debug.Log("No target within range");
        }
    }

    private void Fire() {
        Bullet shotBullet = bulletsAvailable.Pop();
        shotBullet.gameObject.SetActive(true);
        shotBullet.transform.position = bulletSpawnPoint.position;
        shotBullet.transform.rotation = Quaternion.LookRotation(transform.GetChild(0).forward, transform.up);
        shotBullet.StartVelocity();
        shotBullet.ResetTimer();
    }

    public void BulletReturned(Bullet bulletReturned) { //so that the gun knows that it can fire this bullet again
        bulletsAvailable.Push(bulletReturned);
    }
}
