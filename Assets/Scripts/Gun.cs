using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    private IVRInputDevice primaryInput;
    void Start()
    {
        //Get the primaryInput
        primaryInput = VRDevice.Device.PrimaryInputDevice;
        print("gun woke up");
    }

    // Update is called once per frame
    void Update()
    {
        if (primaryInput.GetButtonDown(VRButton.One))
        {
            print("fired gun");
            //Trigger has been pressed and released
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.LookRotation(transform.forward, transform.up)); //make the bullet go forward
            newBullet.GetComponent<Bullet>().Gun = transform;
        }
    }
}
