using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MultiBulletUpgrade : UpgradeListener
{
    [SerializeField] private Gun gun;
    [SerializeField] private int multiBulletCount = 5;
    [SerializeField] private float multiBulletSpreadAngle = 15f;

    public override void DoUpgrade()
    {
        gun.ActivateMultiBulletUpgrade(multiBulletCount, multiBulletSpreadAngle);
        DestroyThis();
    }
}