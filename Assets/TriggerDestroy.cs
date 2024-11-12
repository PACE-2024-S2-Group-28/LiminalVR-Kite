using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy : MonoBehaviour
{

    public void Start()
    {
        Debug.Log("TRIGGER DESTROY IS ON " + gameObject.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        //var otherT = other.transform;
        //var parentRockDestroyerS = otherT.parent.GetComponent<RockDestroyer>();
        //if(parentRockDestroyerS!=null) {
        //    parentRockDestroyerS.event
        //}

        if(FakeTag.CheckTags(other.gameObject, FakeTag.AllAsteroidTags)){
            Debug.Log("destroying a stroid");
            RockDestroyer.SEvent_RockDestroyed?.Invoke();
        }
        
        GameObject.Destroy(other.gameObject);
    }
}
