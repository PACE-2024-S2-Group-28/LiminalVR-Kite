﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTurret : UpgradeListener
{
    [SerializeField] private GameObject turret;
    public override void DoUpgrade()
    {
        Debug.Log("Turret Activated");
        turret.SetActive(true);
        DestroyThis(); //don't need to keep listening
    }
}