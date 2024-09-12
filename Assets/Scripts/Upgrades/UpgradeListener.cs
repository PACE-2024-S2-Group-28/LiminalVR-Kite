using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeListener : MonoBehaviour
{
    [SerializeField] private int upgradesRequired;

    void Start()
    {
        GameManager.Instance.UpgradeUnlocked.AddListener(UpgradeHappened);
    }

    public virtual void UpgradeHappened() //count down until it's time to do an upgrade
    {
        upgradesRequired--;
        if (upgradesRequired == 0) {
            DoUpgrade();
        }
    }

    public virtual void DoUpgrade() {}

    public void DestroyThis() { //for when you've done all your upgrades and are no longer useful
        GameManager.Instance.UpgradeUnlocked.RemoveListener(UpgradeHappened);
        Destroy(this);
    }
}
