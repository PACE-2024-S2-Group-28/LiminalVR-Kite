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
    
    [SerializeField]
    private int totalBullets = 3; //how many bullets to put into the stack of available bullets
    private Stack<Bullet> bulletsShot = new Stack<Bullet>();
    private Stack<Bullet> bulletsAvailable = new Stack<Bullet>();
    
    [SerializeField] private float rechargeTimer;
    
    private enum InputHand {Primary, Secondary}
    [SerializeField]
    private InputHand controllerHand = InputHand.Primary;
    private IVRInputDevice inputDevice;

    [SerializeField]
    private Vector3 localFirePos = Vector3.zero;

    //events to subscribe vfx or sfx or any related behaviour
    public UnityEvent<Gun> event_tryFire;
    public UnityEvent<Gun> event_shotFired;
    public UnityEvent<Gun> event_outOfAmmo;
    //public UnityEvent<Gun> event_missFire;

    void Start()
    {
        //Get the primaryInput
        
        inputDevice = (controllerHand == InputHand.Primary) ? VRDevice.Device?.PrimaryInputDevice : VRDevice.Device?.SecondaryInputDevice;

        //parent so bullets dont flood scene heirarchy
        if (bulletParent == null)
            bulletParent = new GameObject("Bullet_Parent");

        //make the bullet stacks
        for (int i = 0; i < totalBullets; i++) {
            Bullet newBullet = Instantiate(bullet, transform.position, Quaternion.LookRotation(transform.forward, transform.up)); //make the bullet go forward
            newBullet.transform.parent = bulletParent.transform;
            newBullet.Gun = this;
            newBullet.gameObject.SetActive(false);
            bulletsAvailable.Push(newBullet);
        }
        Debug.Log("stack count = " + bulletsAvailable.Count);
    }

    // Handle shooting the gun and managing the cooldown
    void Update()
    {
        Debug.Log("stack count = " + bulletsAvailable.Count);
        if (inputDevice == null) return;


        rechargeTimer -= Time.deltaTime;
        if (inputDevice.GetButtonDown(VRButton.One) && rechargeTimer <= 0) {
            TryFire();
        }
    }

    [Button]
    private void TryFire()
    {
        event_tryFire?.Invoke(this);

        //ammo and recharge check
        if (bulletsAvailable.Count <= 0) {
            //event_missFire?.Invoke(this);
            event_outOfAmmo?.Invoke(this);
            return;
        }

        rechargeTimer = shootCooldown;
        Bullet shotBullet = bulletsAvailable.Pop();
        shotBullet.gameObject.SetActive(true);
        shotBullet.transform.position = transform.TransformPoint(localFirePos);
        shotBullet.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        shotBullet.StartVelocity();
        shotBullet.ResetTimer();
        bulletsShot.Push(shotBullet);

        event_shotFired?.Invoke(this);
    }

    public void BulletReturned(Bullet bulletReturned) { //so that the gun knows that it can fire this bullet again
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
