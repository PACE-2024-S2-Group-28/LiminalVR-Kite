using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootCooldown = 2; //how long between each shot
    private int totalBullets = 3; //how many bullets to put into the stack of available bullets
    private Stack<GameObject> bulletsShot = new Stack<GameObject>();
    private Stack<GameObject> bulletsAvailable = new Stack<GameObject>();
    [SerializeField] private float rechargeTimer;
    private IVRInputDevice primaryInput;
    void Start()
    {
        //Get the primaryInput
        primaryInput = VRDevice.Device.PrimaryInputDevice;

        //make the bullet stacks
        for (int i = 0; i < totalBullets; i++) {
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.LookRotation(transform.forward, transform.up)); //make the bullet go forward
            newBullet.GetComponent<Bullet>().Gun = transform;
            newBullet.SetActive(false);
            bulletsAvailable.Push(newBullet);
        }
    }

    // Handle shooting the gun and managing the cooldown
    void Update()
    {
        rechargeTimer -= Time.deltaTime;
        if (primaryInput.GetButtonDown(VRButton.One) && rechargeTimer <= 0 && bulletsAvailable.Count > 0)
        {
            rechargeTimer = shootCooldown;
            GameObject shotBullet = bulletsAvailable.Pop();
            shotBullet.SetActive(true);
            shotBullet.transform.position = transform.position;
            shotBullet.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            shotBullet.GetComponent<Bullet>().ResetTimer();
            bulletsShot.Push(shotBullet);
        }
    }

    public void BulletReturned(GameObject bulletReturned) { //so that the gun knows that it can fire this bullet again
        bulletsAvailable.Push(bulletReturned);
    }
}
