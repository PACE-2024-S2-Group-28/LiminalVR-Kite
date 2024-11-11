using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [SerializeField] private float timeToRise = 1.5f;
    [SerializeField] private float shootCooldown = 2; //how long between each shot. Keep this higher than time to rotate
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
    private Quaternion startRotation;

    private LineRenderer beam; //the laser beam

    // SFX
    [SerializeField] private SoundScripObj turretFireSFX;
    [SerializeField] private SoundScripObj chargeSFX;
    [SerializeField] private AudioSource fireAudioSource;

    enum TurretState { off, cooldown, charging, firing}
    TurretState state = TurretState.cooldown;

    private void Start()
    {
        beam = GetComponent<LineRenderer>();
        beam.enabled = false;

        state = TurretState.off;
        rechargeTimer = shootCooldown;
        timeActivated = Time.time;
    }

    private float timeActivated;

    private float timeStartedCharging;
    const float chargeTime = 1.2f;
    private float timeLastFired;

    // Update is called once per frame
    void Update()
    {
        switch(state){
            case TurretState.off:
                if(Time.time <= timeActivated + timeToRise) {
                    float t = (Time.time - timeActivated) / timeToRise;
                    float y = Mathf.Lerp(-7f, 0f, easeInOutQuad(t));
                    transform.localPosition = Vector3.up * y;
                } else {
                    transform.localPosition = Vector3.zero;
                    lastFiredTime = Time.time;
                    state = TurretState.cooldown;
                }
                break;
            case TurretState.cooldown:
                if(Time.time >= timeLastFired + shootCooldown) {
                    state = TurretState.charging;
                    timeStartedCharging = Time.time;
                }
                break;
            case TurretState.charging:
                if (target == null) GetTargets();
                if (target == null) return;

                rotateTimer += Time.deltaTime;
                FaceTarget();
                if (Time.time >= timeStartedCharging + chargeTime) {
                    Fire();
                    timeLastFired = Time.time;
                    state = TurretState.cooldown;
                }

                break;
            case TurretState.firing:
                break;
        }
    }

    [SerializeField]
    private LayerMask targetMask;


    private static float lastTimeCheckedTargets;

    private void GetTargets()
    {
        if (Time.time < lastTimeCheckedTargets + .2f) return;
        lastTimeCheckedTargets = Time.time;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, targetMask); //check all nearby collisions
        Collider closestTarget = null;
        float minSqrMagDist = range*range;
        Vector3 closestSpeed = Vector3.zero;
        float sqrMagDist;

        //want to work out the future closest asteroid. The time in the future we're looking at is the time when an asteroid would be getting destroyed by the beam. This makes them seem like they're looking ahead
        foreach (var hitCollider in hitColliders) {
            Vector3 targetSpeed = hitCollider.attachedRigidbody.velocity; //speed of asteroid
            Vector3 rotateTargetPoint = hitCollider.transform.position + (targetSpeed * (timeToRotate + beamTime)); //position asteroid moves to during that time
            sqrMagDist = Vector3.SqrMagnitude(transform.position - rotateTargetPoint);
            //check that the collider is closest of all checked so far, and that it's an asteroid
            if (sqrMagDist < minSqrMagDist) {
                closestTarget = hitCollider;
                minSqrMagDist = sqrMagDist;
                closestSpeed = targetSpeed;
            }
        }
        if (closestTarget != null) { //if you found something to shoot, remember it and the turret's current facing
            target = closestTarget.transform;
            Vector3 targetPoint = target.position + (closestSpeed * timeToRotate); //position asteroid moves to while turret rotates
            startRotation = Quaternion.LookRotation(targetPoint - gunPitchPivot.position); //rotation to face the point the asteroid will be in
            rotateTimer = 0;
        }
        else { //nothing found in range
            Debug.Log("No target within range");
            rechargeTimer = shootCooldown;
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
            Vector3 targetPoint = target.position + (closestSpeed * timeToRotate); //position asteroid moves to while turret rotates
            startRotation = Quaternion.LookRotation(targetPoint - gunPitchPivot.position); //rotation to face the point the asteroid will be in
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
        Debug.Log("trying to face target");

        var targetRotQ = Quaternion.identity;
        targetRotQ = overrideTargetQ.HasValue? overrideTargetQ.Value : Quaternion.LookRotation(target.position - gunPitchPivot.position);
        var slerpedRotEuler = Quaternion.Slerp(Quaternion.Euler(new Vector3(gunPitchPivot.eulerAngles.x, gunPitchPivot.eulerAngles.y, 0)), targetRotQ, rotateTimer / timeToRotate).eulerAngles;

        gunPitchPivot.eulerAngles = new Vector3(slerpedRotEuler.x, gunPitchPivot.eulerAngles.y, gunPitchPivot.eulerAngles.z); //pitch
        baseYawPivot.eulerAngles = new Vector3(baseYawPivot.eulerAngles.x, slerpedRotEuler.y, baseYawPivot.eulerAngles.z); //yaw
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

        beam.SetPosition(0, laserStartPoint.position);
        beam.SetPosition(1, target.position);
        beam.enabled = true;
        Invoke(nameof(DisableBeam), .2f);
        
        //destroy rock and disable beam
        rechargeTimer = shootCooldown;
        if (target.gameObject.activeSelf == true) { //if the gun shot it already, don't try destroying it again
            target.parent.gameObject.GetComponent<RockDestroyer>().ChangeRock(forceDir: transform.forward, hitPos: target.position);
            AsteroidGameManager.Instance.HandleAsteroidDestruction(target.gameObject.CompareTag("GoldAsteroid"));
        }
        startRotation = Quaternion.LookRotation(target.position - gunPitchPivot.position); //get the rotation it was last pointing in
        target = null;
        rotateTimer = 0;
    }

    private void DisableBeam()
    {
        beam.enabled = false;
    }


    //attempted easing function
    const float c4 = (2 * Mathf.PI) / 3;
    private float easeOutElastic(float t){
        return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -30 * t) * Mathf.Sin((t * 16f - 0.75f) * c4) + 1f;
    }

    private float easeInOutQuad(float x) {
        return x < 0.5 ? 2 * x* x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }
}
