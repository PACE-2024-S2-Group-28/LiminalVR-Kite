using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerEventsTest : MonoBehaviour
{
    public void TriggerEvents()
    {
        AudioEventsGlobal.instance.positionTest.OptInvoke(wPos: new Vector3(20, 0, 0));
    }
}
