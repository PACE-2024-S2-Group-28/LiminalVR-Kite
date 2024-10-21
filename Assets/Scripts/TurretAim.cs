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
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private Transform gunPitchPivot; //can rotate around the x-axis (look up and down)
    [SerializeField] private Transform baseYawPivot; //can rotate around the z axis (look side to side)
    private float rotateTimer;
    private float rechargeTimer;
    private float beamTimer;
    private Transform target = null;
    private Vector3 targetPoint; //where the asteroid will be when the turret finishes rotating
    private Quaternion startHeadRotation; //rotation of the head before it started firing
    private Quaternion toHeadRotation; //rotation of the head when facing the asteroid's future position
    private Quaternion startBaseRotation; //rotation of the base before it started firing
    private Quaternion toBaseRotation; //rotation of the base when facing the asteroid's future position
    private LineRenderer beam; //the laser beam

    // SFX
    [SerializeField] private SoundScripObj turretFireSFX;
    [SerializeField] private SoundScripObj chargeSFX;
    [SerializeField] private AudioSource fireAudioSource;

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

        if (rechargeTimer <= 0) {
            if (target == null) {
                FindFiringSolution(); //pick a target to lock onto
            } else { //rotate to face asteroid, then fire a beam at it
                if (rotateTimer < timeToRotate) {
                    FaceTarget();
                } else {
                    Fire();
                }
            }
        } else { //rotate back to face forward
            FaceTarget(Quaternion.Euler(Vector3.forward)); //resetting to face forward
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
            //startHeadRotation = headPivot.rotation;
            //startBaseRotation = basePivot.rotation;
            //targetPoint = target.position + (closestSpeed * timeToRotate); //position asteroid moves to while turret rotates
            //Vector3 dir = targetPoint - transform.position;
            //toHeadRotation = Quaternion.LookRotation(new Vector3(0, dir.y, dir.z), Vector3.back); //direction to point to asteroid
            //toBaseRotation = Quaternion.LookRotation(new Vector3(dir.x, dir.y, 0) , transform.TransformDirection(Vector3.down)); //direction to point to asteroid
            rotateTimer = 0;
        }
        else
        { //nothing found in range
            Debug.Log("No target within range");
            rechargeTimer = shootCooldown;
        }
    }

    private void FaceTarget(Quaternion? overrideTargetQ = null)
    {
        var targetRotQ = Quaternion.identity;
        targetRotQ = overrideTargetQ.HasValue? overrideTargetQ.Value : Quaternion.LookRotation(target.position - gunPitchPivot.position);
        var slerpedRotEuler = Quaternion.Slerp(Quaternion.Euler(new Vector3(gunPitchPivot.eulerAngles.x, gunPitchPivot.eulerAngles.y, 0)), targetRotQ, rotateTimer / timeToRotate).eulerAngles;

        gunPitchPivot.eulerAngles = new Vector3(slerpedRotEuler.x, gunPitchPivot.eulerAngles.y, gunPitchPivot.eulerAngles.z); //pitch
        baseYawPivot.eulerAngles = new Vector3(baseYawPivot.eulerAngles.x, slerpedRotEuler.y, baseYawPivot.eulerAngles.z); //yaw

        rotateTimer += Time.deltaTime;
        if (rotateTimer >= timeToRotate)
        { //activate beam when facing asteroid
            beamTimer = beamTime;
            beam.enabled = true;
        }
    }


    private bool charging = false;

    //temp sound timing vars
    private float lastFiredTime;
    private float fireSoundInterval = .2f;

    private void Fire()
    {
        //triggering sound on interval instead of every frame
        if (Time.time - lastFiredTime >= fireSoundInterval) {
            turretFireSFX.Play(audioSourceParam: fireAudioSource, wPos: transform.position);
            lastFiredTime = Time.time;
        }

        beamTimer -= Time.deltaTime;
        if ((beamTimer > 0) && (target.gameObject.activeSelf == true))
        { //move the laser beam and rotate to face the rock
            Vector3 dir = target.position - transform.position;
            FaceTarget();
            //headPivot.rotation = Quaternion.LookRotation(new Vector3(0, dir.y, dir.z), Vector3.back); //direction to point to asteroid
            //basePivot.rotation = Quaternion.LookRotation(new Vector3(dir.x, dir.y, 0) , transform.TransformDirection(Vector3.down)); //direction to point to asteroid
            beam.SetPosition(0, laserStartPoint.position);
            beam.SetPosition(1, target.position);

            if (charging) {
                //chargeSFX.Play();
                charging = false;
            }
        }
        else
        { //destroy rock and disable beam
            // turretFireSFX.Play(wPos: target.position);
            //chargeSFX.Stop();
            //turretFireSFX.Play();
            charging = true;

            beam.enabled = false;
            rechargeTimer = shootCooldown;
            if (target.gameObject.activeSelf == true)
            { //if the gun shot it already, don't try destroying it again
                target.parent.gameObject.GetComponent<RockDestroyer>().ChangeRock(forceDir: transform.forward, hitPos: target.position);
                AsteroidGameManager.Instance.HandleAsteroidDestruction(target.gameObject.CompareTag("GoldAsteroid"));
            }
            target = null;
            rotateTimer = 0;

            //return to original positions. This works!
            startHeadRotation = gunPitchPivot.rotation;
            toHeadRotation = Quaternion.LookRotation(new Vector3(gunPitchPivot.position.x, gunPitchPivot.position.y, gunPitchPivot.position.z + 1) - gunPitchPivot.position, Vector3.up);
            startBaseRotation = baseYawPivot.rotation;
            toBaseRotation = Quaternion.LookRotation(Vector3.up, Vector3.right);
        }
    }

}
