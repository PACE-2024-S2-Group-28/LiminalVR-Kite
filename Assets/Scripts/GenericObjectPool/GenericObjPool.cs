using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[RequireComponent(typeof(GenericObjPoolUserI))]
public class GenericObjPool : MonoBehaviour
{
    /// <summary>
    /// An Object to put anything that needs to be managed but not have a parent or follow a parent currently.
    /// Avoids clutter in scenegraph.
    /// </summary>
    public static Transform FreeParent;

    [SerializeField]
    private int numObjects = 10;

    [SerializeField]
    private bool objectsAsChildren = true;

    private GenericObjPoolUserI user;

    private GameObject externParent;
    private GameObject parent;
    private GameObject[] objects;
    private int nextObjID = 0;

    private void Start()
    {
        if(FreeParent == null)
            FreeParent = new GameObject("Anti-clutter ObjectPool Parent").transform;
    }

    private void InitialisePool()
    {
        if(user == null)
            user = GetComponent<GenericObjPoolUserI>();

        if (objectsAsChildren)
            parent = this.gameObject;
        else
        {
            parent = externParent = new GameObject(this.name + " object pool");
            externParent.transform.parent = FreeParent;
        }

        objects = new GameObject[numObjects];
        for (int i = 0; i < numObjects; i++)
        {
            objects[i] = new GameObject(this.name + " Obj " + i);
            objects[i].transform.parent = parent.transform;
            user.ObjSetup(objects[i]);
        }
    }

    private void DestroyPool()
    {
        for (int i = 0; i < objects.Length; i++)
            if(objects[i]!=null)
                GameObject.Destroy(objects[i]);

        objects = null;
        if(externParent != null)
            GameObject.Destroy(externParent);
    }

    private void OnEnable()
    {
        InitialisePool();
    }

    private void OnDisable()
    {
        DestroyPool();
    }

    //gives the specified component from a pool object while cycling through them
    public T SupplyObject<T>(float resetTime = 0) where T : Component
    {
        var obj = objects[nextObjID];
        if (obj == null) { 
            Debug.LogError("object pool object is null, likely destroyed"); 
            return null; 
        }
        
        //increment and loop to 0
        nextObjID += 1;
        nextObjID %= numObjects;
        
        return obj.GetComponent<T>();
    }

    //private void OnValidate()
    //{
    //    for(int i = 0; i < objects.Length; i++)
    //    {
    //        EditorApplication.delayCall += () => DestroyImmediate(objects[i]);
    //    }

    //    InitialisePool();
    //}
}
