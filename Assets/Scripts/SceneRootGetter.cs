using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRootGetter : MonoBehaviour
{
    public GameObject root;
    
    private static SceneRootGetter instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameObject GetRoot()
    {
        return instance.root;
    }
}
