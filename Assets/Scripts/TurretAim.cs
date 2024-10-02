using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [SerializeField] private float shootCooldown = 2; //how long between each shot
    [SerializeField] private float beamTime = 2; //how many seconds it takes to destroy an asteroid
    [SerializeField] private float range;
    private float rechargeTimer;
    private float beamTimer;
    private Transform target = null;
    private LineRenderer beam; //the laser beam

    void Awake() {
        rechargeTimer = shootCooldown;
    }

    private void Start() {
        beam = GetComponent<LineRenderer>();
        beam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        rechargeTimer -= Time.deltaTime;
        if (rechargeTimer <= 0) {
            if (target == null) {
                FindFiringSolution();
            }
            else {
                Fire();
            }            
        }
    }

    private void FindFiringSolution() { //checks for nearby asteroids, then rotates to immediately face the nearest one
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range); //check all nearby collisions
        Collider closestTarget = null;
        float closestDistance = range;
        float distanceToThis;
        foreach (var hitCollider in hitColliders) {
            distanceToThis = Vector3.Distance(transform.position, hitCollider.transform.position);
            //check that the collider is closest of all checked so far, and that it's an asteroi
            if ((distanceToThis < closestDistance) && (hitCollider.gameObject.CompareTag("Rock"))) {
                closestTarget = hitCollider;
                closestDistance = distanceToThis;
            }
        }
        if (closestTarget != null) { //if you found something to shoot, remember it, and activate the laser beam
            target = closestTarget.transform;
            beamTimer = beamTime;
            beam.enabled = true;
        }
        else {
            Debug.Log("No target within range");
            rechargeTimer = shootCooldown;
        }
    }

    private void Fire() {
        beamTimer-= Time.deltaTime;
        if (beamTimer > 0) { //move the laser beam and rotate to face the rock
            transform.rotation = Quaternion.LookRotation(target.position-transform.position, Vector3.up);
            beam.SetPosition(0, transform.position);
            beam.SetPosition(1, target.position);
        }
        else {
            beam.enabled = false;            
            rechargeTimer = shootCooldown;
            target.parent.gameObject.GetComponent<RockDestroyer>().ChangeRock(forceDir: transform.forward, hitPos: target.position);
        }
    }

}
