using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioPositionalEvent : UnityEvent<Transform, Vector3?>
{
    //wrapper function is needed since default UnityEvent.Invoke(); does not support optional parameters with default values
    public void OptInvoke(Transform parent = null, Vector3? wPos = null)
    {
        base.Invoke(parent, wPos);
    }
}
