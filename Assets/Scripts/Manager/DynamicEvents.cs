using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Events;
using NaughtyAttributes;

public class DynamicEvents : MonoBehaviour
{
    private static DynamicEvents instance;
    public static DynamicEvents Instance { get { return instance; } }

    [SerializeField]
    [Expandable]
    private StringArraySO eventNamesSO;
    public String[] EventNames { get { return eventNamesSO.Strings; } }

    public Dictionary<string, Action<float>> eventDict = new Dictionary<string, Action<float>>();

    private void Awake()
    {
        eventDict = new Dictionary<string, Action<float>>();

        foreach(string eventName in EventNames) {
            eventDict.Add(eventName, (float value) => { });
        }
    }

    private void Start()
    {
        if (instance != null) {
            GameObject.Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// Invoke event by name, events are genereated on start with a dictionary linking to matching name and arent directly accessible
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="volumeMult"></param>
    public void InvokeEventByName(string eventName, float volumeMult = 1f)
    {
        //Debug.Log("Invoking event: " + eventName);
        eventDict[eventName]?.Invoke(volumeMult);
    }

    [Button]
    public void InvokeEvents()
    {
        foreach (var e in eventDict)
        {
            e.Value?.Invoke(1f);
        }
    }

    public String[] GetEventNames(string[] currentNames = null)
    {
        return null;
    }
}
