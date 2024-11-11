using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using UnityEngine.Events;
using NaughtyAttributes;

public class DynamicEvents : MonoBehaviour
{
    private static DynamicEvents instance = null;
    public static DynamicEvents Instance { get { return instance; } }

    [SerializeField]
    [Expandable]
    private StringArraySO eventNamesSO;
    public String[] EventNames { get { return eventNamesSO.Strings; } }

    public Dictionary<string, Action<float>> eventDict = new Dictionary<string, Action<float>>();

    private void Awake()
    {
        if(instance!=null) {
            Destroy(this);
            return;
        }
        instance = this;

        eventDict = new Dictionary<string, Action<float>>();

        foreach(string eventName in EventNames) {
            eventDict.Add(eventName, (float value) => { });
        }
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

    public void SubToEventByName(string eventName, Action<float> action)
    {
        if (!eventDict.ContainsKey(eventName)) return;
        eventDict[eventName] += action;
    }

    [Button]
    public void InvokeEvents()
    {
        foreach (var e in eventDict)
        {
            e.Value?.Invoke(1f);
        }
    }

    /// <summary>
    /// Get a string array of all event names
    /// </summary>
    /// <param name="currentNames">pass an array of current event names to only get new ones</param>
    /// <returns></returns>
    public String[] GetEventNames(string[] currentNames = null)
    {
        List<string> eventNames = EventNames.ToList();

        if (currentNames != null && currentNames.Length > 0) {
            foreach (var n in currentNames) {
                eventNames.Remove(n);
            }
        }

        return eventNames.ToArray();
    }
}
