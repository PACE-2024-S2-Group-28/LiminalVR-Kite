using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActivateTurret : UpgradeListener
{
    [SerializeField] private GameObject turret;

    [Button]
    public override void DoUpgrade()
    {
#if UNITY_EDITOR
        if(!Application.isPlaying) return;
#endif

        Debug.Log("Turret Activated");
        turret.SetActive(true);
        DestroyThis(); //don't need to keep listening
    }
}
