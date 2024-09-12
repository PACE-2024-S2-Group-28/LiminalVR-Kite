using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTurret : UpgradeListener
{
    [SerializeField] private GameObject turret;
    public override void DoUpgrade() {
        turret.SetActive(true);
        DestroyThis(); //don't need to keep listening
    }
}
