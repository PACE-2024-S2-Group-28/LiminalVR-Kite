using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //var otherT = other.transform;
        //var parentRockDestroyerS = otherT.parent.GetComponent<RockDestroyer>();
        //if(parentRockDestroyerS!=null) {
        //    parentRockDestroyerS.event
        //}

        if (other.CompareTag("Rock")) RockDestroyer.SEvent_RockDestroyed?.Invoke();
        GameObject.Destroy(other.gameObject);
    }
}
