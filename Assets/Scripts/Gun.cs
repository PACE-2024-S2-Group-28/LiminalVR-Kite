using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;
using NaughtyAttributes;
using UnityEngine.Events;
using ScriptableObjects;
using Liminal.SDK.VR.Utils;
using Liminal.SDK.VR.Avatars;

public class Gun : MonoBehaviour
{
    static private GameObject bulletParent = null;

    [SerializeField] private Bullet bullet;
    [SerializeField] private Transform bulletObjPoolParentT;

    [Header("Gun Settings")]
    [SerializeField] private float shootCooldown = 2; //how long between each shot
    [SerializeField] private float bulletLife = 3; //how many seconds a bullet exists before it returns to the gun
    [SerializeField] private float rechargeTimer;
    [SerializeField]
    private Vector3 localFirePos = Vector3.zero;

    private int totalBullets; //how many bullets to put into the stack of available bullets
    private Stack<Bullet> bulletsShot = new Stack<Bullet>();
    private Stack<Bullet> bulletsAvailable = new Stack<Bullet>();


    private enum InputHand { Primary, Secondary }
    [Header("Hand")]
    [SerializeField]
    private InputHand controllerHand = InputHand.Primary;
    private IVRInputDevice inputDevice;


    private MeterController meter;


    [Header("Multi-Bullet Upgrade Properties")]
    private bool multiBulletUpgradeActive = false;
    [SerializeField] private int bulletCount = 1;

    [Header("Effect Settings")]
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private SoundScripObj fireSFX;

    //events to subscribe vfx or sfx or any related behaviour
    public UnityEvent<Gun> event_tryFire;
    public UnityEvent<Gun> event_shotFired;
    public UnityEvent<Gun> event_outOfAmmo;
    //public UnityEvent<Gun> event_missFire;

    private Animator gunAnimator;

    [SerializeField]
    private ParticleSystem pSys_FireFlash;


    void Start()
    {
        //Get the primaryInput        
        inputDevice = (controllerHand == InputHand.Primary) ? VRDevice.Device?.PrimaryInputDevice : VRDevice.Device?.SecondaryInputDevice;
        var hand = (controllerHand == InputHand.Primary) ? VRAvatar.Active.PrimaryHand : VRAvatar.Active.SecondaryHand;
        hand.SetControllerVisibility(false);
        inputDevice.Pointer?.Deactivate();

        totalBullets = Mathf.CeilToInt(bulletLife / shootCooldown) + 1;
        gunAnimator = GetComponent<Animator>();
        meter = GetComponent<MeterController>();
        InitializeBullets(totalBullets);
    }

    private void InitializeBullets(int bulletAmount)
    {
        if (bulletParent == null){
            bulletParent = new GameObject("Bullet_Parent"); // So the bullets don't flood the hierarchy
            //bulletParent.transform.parent = bulletRoot.transform;
        }

        for (int i = 0; i < bulletAmount; i++)
        {
            Bullet newBullet = Instantiate(bullet, transform.position, Quaternion.LookRotation(transform.forward, transform.up)); // Makes the bullet go forward
            newBullet.transform.parent = bulletParent.transform;
            newBullet.Gun = this;
            newBullet.TimeAlive = bulletLife;
            newBullet.gameObject.SetActive(false);
            bulletsAvailable.Push(newBullet);
        }

    }



    // Handle shooting the gun and managing the cooldown
    void Update()
    {
        if (inputDevice == null) return;

        rechargeTimer -= Time.deltaTime;
        meter.UpdateMeter(rechargeTimer/shootCooldown);
        if (inputDevice.GetButtonDown(VRButton.One) && rechargeTimer <= 0)
        {
            TryFire();
        }


    }

    [Button]
    private void TryFire()
    {

        event_tryFire?.Invoke(this);

        // Cbeck recharge and ammo
        if (bulletsAvailable.Count <= 0)
        {
            event_outOfAmmo?.Invoke(this);
            return;
        }

        rechargeTimer = shootCooldown;

        gunAnimator.SetTrigger("Fire");

        pSys_FireFlash.Play();
        inputDevice.SendInputHaptics(frequency: .5f, amplitude: 1f, duration: 0.15f);

        if (multiBulletUpgradeActive)
        {
            FireMultipleBullets();
        }
        else
        {

            fireSFX?.Play(audioSourceParam: sfxSource);
            FireSingleBullet();
        }

        event_shotFired?.Invoke(this);
    }

    private void PlaySound()
    {
        fireSFX?.Play(sParent: transform);
    }

    private void FireSingleBullet()
    {
        Bullet shotBullet = bulletsAvailable.Pop();
        shotBullet.gameObject.SetActive(true);
        shotBullet.transform.position = transform.TransformPoint(localFirePos);
        shotBullet.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        shotBullet.StartVelocity();
        shotBullet.ResetTimer();
        bulletsShot.Push(shotBullet);
    }

    [SerializeField]
    private ParticleSystem bulletSparks;

    public void TriggerSparks(Vector3 worldPos)
    {
        bulletSparks.transform.position = worldPos;
        bulletSparks.Play();
    }

    [SerializeField]
    private float extraBulletRadius = .25f;

    private void FireMultipleBullets()
    {
        float angleStep = 360f / bulletCount; // Full circle divided by number of bullets
        //float extraBulletRadius = 1f; // Distance from the center of the circular pattern

        for (int i = 0; i < bulletCount; i++)
        {
            Bullet shotBullet = bulletsAvailable.Pop();
            shotBullet.gameObject.SetActive(true);

            // Position the bullet in a circular pattern
            float currentAngle = angleStep * i;
            float angleRad = Mathf.Deg2Rad * currentAngle;

            Vector3 offset = new Vector3(
                Mathf.Cos(angleRad) * extraBulletRadius,  // x
                Mathf.Sin(angleRad) * extraBulletRadius,  // y 
                0                             // z 
            );

            shotBullet.transform.position = transform.TransformPoint(localFirePos + offset);
            shotBullet.transform.forward = transform.forward;
            shotBullet.Rb.velocity = transform.forward * shotBullet.Speed;

            shotBullet.ResetTimer();
            bulletsShot.Push(shotBullet);
            Invoke(nameof(PlaySound), (float)i * 0.05f);
        }
    }




    // If the multibullet spawns 4 bullets, the bullet stack will be factored by 4
    public void ActivateMultiBulletUpgrade(int newBulletCount)
    {
        multiBulletUpgradeActive = true;
        bulletCount = newBulletCount;

        int additionalBullets = totalBullets * (bulletCount - 1);


        InitializeBullets(additionalBullets); // Add extra bullets to the stack
    }


    public void BulletReturned(Bullet bulletReturned)
    { //so that the gun knows that it can fire this bullet again
        bulletsAvailable.Push(bulletReturned);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * .01f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.TransformPoint(localFirePos), Vector3.one * .01f);
    }
}
