using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MultiBulletUpgrade : UpgradeListener
{
    [SerializeField] private Gun gun;
    [SerializeField] private int multiBulletCount = 5;

    public override void DoUpgrade()
    {
        gun.ActivateMultiBulletUpgrade(multiBulletCount);
        DestroyThis();
    }
}