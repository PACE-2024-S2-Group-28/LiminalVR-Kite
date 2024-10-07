using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [SerializeField] private float shootCooldown = 2; //how long between each shot
    [SerializeField] private float beamTime = 2; //how many seconds it takes to destroy an asteroid
    [SerializeField] private float range;
    [SerializeField] private float timeToRotate = 0.5f; //how many seconds it takes to rotate to face an asteroid before firing. Keep this lower than shootCooldown
    private float rotateTimer;
    private float rechargeTimer;
    private float beamTimer;
    private Transform target = null;
    private Vector3 targetPoint; //where the asteroid will be when the turret finishes rotating
    private Quaternion startRotation; //rotation of the transform before it started firing
    private Quaternion toRotation; //rotation of the transform when facing the asteroid's future position
    private LineRenderer beam; //the laser beam

    // SFX
    [SerializeField] private SoundScripObj turretFireSFX;

    void Awake()
    {
        rechargeTimer = shootCooldown;
    }

    private void Start()
    {
        beam = GetComponent<LineRenderer>();
        beam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        rechargeTimer -= Time.deltaTime;
        if (rechargeTimer <= 0)
        {
            if (target == null)
            {
                FindFiringSolution(); //pick a target to lock onto
            }
            else
            { //rotate to face asteroid, then fire a beam at it
                if (rotateTimer < timeToRotate)
                {
                    FaceTarget();
                }
                else
                {
                    Fire();
                }
            }
        }
        else
        { //rotate back to face forward
            transform.rotation = Quaternion.Slerp(startRotation, toRotation, rotateTimer / timeToRotate);
            rotateTimer += Time.deltaTime;
        }
    }

    private void FindFiringSolution()
    { //checks for nearby asteroids, then rotates to immediately face the nearest one
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range); //check all nearby collisions
        Collider closestTarget = null;
        float closestDistance = range;
        Vector3 closestSpeed = Vector3.zero;
        float distanceToThis;

        //want to work out the future closest asteroid. The time in the future we're looking at is the time when an asteroid would be getting destroyed by the beam. This makes them seem like they're looking ahead
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Rock"))
            { //only target rocks
                Vector3 targetSpeed = hitCollider.attachedRigidbody.velocity; //speed of asteroid
                Vector3 rotateTargetPoint = hitCollider.transform.position + (targetSpeed * (timeToRotate + beamTime)); //position asteroid moves to during that time
                distanceToThis = Vector3.Distance(transform.position, rotateTargetPoint);
                //check that the collider is closest of all checked so far, and that it's an asteroid
                if (distanceToThis < closestDistance)
                {
                    closestTarget = hitCollider;
                    closestDistance = distanceToThis;
                    closestSpeed = targetSpeed;
                }
            }
        }
        if (closestTarget != null)
        { //if you found something to shoot, remember it and the turret's current facing
            target = closestTarget.transform;
            startRotation = transform.rotation;
            targetPoint = target.position + (closestSpeed * timeToRotate); //position asteroid moves to while turret rotates
            toRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up); //direction to point to asteroid
            rotateTimer = 0;
        }
        else
        { //nothing found in range
            Debug.Log("No target within range");
            rechargeTimer = shootCooldown;
        }
    }

    private void FaceTarget()
    {
        transform.rotation = Quaternion.Slerp(startRotation, toRotation, rotateTimer / timeToRotate);
        rotateTimer += Time.deltaTime;
        if (rotateTimer >= timeToRotate)
        { //activate beam when facing asteroid
            beamTimer = beamTime;
            beam.enabled = true;
        }
    }

    private void Fire()
    {
        beamTimer -= Time.deltaTime;
        if ((beamTimer > 0) && (target.gameObject.activeSelf == true))
        { //move the laser beam and rotate to face the rock
            transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            beam.SetPosition(0, transform.position);
            beam.SetPosition(1, target.position);
        }
        else
        { //destroy rock and disable beam
            beam.enabled = false;
            rechargeTimer = shootCooldown;
            if (target.gameObject.activeSelf == true)
            { //if the gun shot it already, don't try destroying it again
                target.parent.gameObject.GetComponent<RockDestroyer>().ChangeRock(forceDir: transform.forward, hitPos: target.position);
            }
            target = null;
            rotateTimer = 0;
            startRotation = transform.rotation;
            toRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }
    }

}
