using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeListener : MonoBehaviour
{
    [SerializeField] private int upgradesRequired;
    [SerializeField] private AudioClip voiceLine;
    private RadioCallouts radio;

    void Start()
    {
        AsteroidGameManager.Instance.UpgradeUnlocked.AddListener(UpgradeHappened);
        radio = FindObjectOfType<RadioCallouts>();
    }

    public virtual void UpgradeHappened() //count down until it's time to do an upgrade
    {
        upgradesRequired--;
        if (upgradesRequired == 0) {
            DoUpgrade();
            if (voiceLine != null) {
                radio.PlayLine(voiceLine);
            }
        }
    }

    public virtual void DoUpgrade() {}

    public void DestroyThis() { //for when you've done all your upgrades and are no longer useful
        AsteroidGameManager.Instance.UpgradeUnlocked.RemoveListener(UpgradeHappened);
        Destroy(this);
    }
}
