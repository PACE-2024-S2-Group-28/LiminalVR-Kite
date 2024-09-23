using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;
using NaughtyAttributes;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    static private GameObject bulletParent = null;

    [SerializeField] private Bullet bullet;
    [SerializeField] private float shootCooldown = 2; //how long between each shot
    [SerializeField] private float bulletLife = 3; //how many seconds a bullet exists before it returns to the gun

    private int totalBullets; //how many bullets to put into the stack of available bullets
    private Stack<Bullet> bulletsShot = new Stack<Bullet>();
    private Stack<Bullet> bulletsAvailable = new Stack<Bullet>();

    [SerializeField] private float rechargeTimer;

    private enum InputHand { Primary, Secondary }
    [SerializeField]
    private InputHand controllerHand = InputHand.Primary;
    private IVRInputDevice inputDevice;

    [SerializeField]
    private Vector3 localFirePos = Vector3.zero;


    [Header("Multi-Bullet Upgrade Properties")]
    private bool multiBulletUpgradeActive = false;
    [SerializeField] private int bulletCount = 1;

    //events to subscribe vfx or sfx or any related behaviour
    public UnityEvent<Gun> event_tryFire;
    public UnityEvent<Gun> event_shotFired;
    public UnityEvent<Gun> event_outOfAmmo;
    //public UnityEvent<Gun> event_missFire;




    void Start()
    {
        //Get the primaryInput        
        inputDevice = (controllerHand == InputHand.Primary) ? VRDevice.Device?.PrimaryInputDevice : VRDevice.Device?.SecondaryInputDevice;

        totalBullets = Mathf.CeilToInt(bulletLife / shootCooldown) + 1;

        InitializeBullets(totalBullets);
    }

    private void InitializeBullets(int bulletAmount)
    {
        if (bulletParent == null)
            bulletParent = new GameObject("Bullet_Parent"); // So the bullets don't flood the hierarchy

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



        Debug.Log(multiBulletUpgradeActive);
        if (multiBulletUpgradeActive)
        {
            FireMultipleBullets();
        }
        else
        {
            Debug.Log("FireSingleBullet");

            FireSingleBullet();
        }

        event_shotFired?.Invoke(this);
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

    private void FireMultipleBullets()
    {
        float angleStep = 360f / bulletCount; // Full circle divided by number of bullets
        float radius = 1f; // Distance from the center of the circular pattern

        for (int i = 0; i < bulletCount; i++)
        {
            Bullet shotBullet = bulletsAvailable.Pop();
            shotBullet.gameObject.SetActive(true);

            // Position the bullet in a circular pattern
            float currentAngle = angleStep * i;
            float angleRad = Mathf.Deg2Rad * currentAngle;

            Vector3 offset = new Vector3(
                Mathf.Cos(angleRad) * radius,  // x
                Mathf.Sin(angleRad) * radius,  // y 
                0                             // z 
            );

            shotBullet.transform.position = transform.TransformPoint(localFirePos + offset);
            shotBullet.transform.forward = transform.forward;
            shotBullet.Rb.velocity = transform.forward * shotBullet.Speed;

            shotBullet.ResetTimer();
            bulletsShot.Push(shotBullet);
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
