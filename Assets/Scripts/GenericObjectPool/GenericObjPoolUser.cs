using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GenericObjPoolUserI
{
    //add necessary components or make changes to the base object in pool when the pool script sets up its pool
    void ObjSetup(GameObject obj);
    
    //enforces a reference to object pool since interface cannot have fields
    GenericObjPool ObjPool { get; }

    //default function for getting an object from pool's Transform if you dont want a specific component
    //just a fallback
    Transform GetObjectTransformFromPool();
}
