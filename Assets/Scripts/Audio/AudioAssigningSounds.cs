using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Specialized;

using System;
using System.Text.RegularExpressions;

[RequireComponent(typeof(AudioEventsGlobal))]
public class AudioAssigningSounds : MonoBehaviour, GenericObjPoolUserI
{
    [SerializeField]
    private AudioEventsGlobal eventSource;

    #region Object Pool Setup
    [SerializeField]
    private GenericObjPool objPool;
    public GenericObjPool ObjPool { get { return objPool; } }

    public void ObjSetup(GameObject obj)
    {
        var source = obj.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
    }
    #endregion


    //[SerializeField]
    //private SoundScripObj defaultSound;

    [SerializeField]
    private List<EventSoundGroup> eventGroups = new List<EventSoundGroup>();

    private Dictionary<string, EventSoundGroup> eventsDict;

    [Serializable]
    public class EventSoundGroup
    {
        public string eventName;
        public SoundScripObj soundObj;
        public AudioPositionalEvent eventRef;
        public UnityAction<Transform, Vector3?> soundDelegate;

        public EventSoundGroup(string name)
        {
            eventName = name;
        }
    }

    private void OnEnable()
    {
        eventSource = GetComponent<AudioEventsGlobal>();
        objPool = GetComponent<GenericObjPool>();

        //instantiate dictionary
        eventsDict = new Dictionary<string, EventSoundGroup>();
        foreach (var group in eventGroups)
            eventsDict.Add(group.eventName, group);

        //finds references for active scene
        GetEvents();

        //adds the sound playing to the events in the dict
        //done through delegate to keep reference so it can be unsubbed
        foreach (var entry in eventsDict)
        {
            var val = entry.Value;
            if (val.eventRef == null || val.soundObj == null) continue;

            val.soundDelegate = (parent, wPos) => { val.soundObj.Play(parent, wPos, ObjPool.SupplyObject<AudioSource>()); };
            val.eventRef.AddListener(val.soundDelegate);
        }
    }

    private void Start()
    {
        //test at start
        //AudioEventsGlobal.instance.positionTest.OptInvoke(wPos: new Vector3(20,0,0));
    }

    private void OnDisable()
    {
        //to be written
        //unsub listeners from all events
        foreach (var entry in eventsDict)
        {
            var val = entry.Value;
            if (val.eventRef == null || val.soundObj == null) continue;

            val.eventRef.RemoveListener(val.soundDelegate);
        }
    }

    //retrive names of all AudioPostionalEvent events from AudioEventsGlobal
    public void GetEventNames()
    {
        FieldInfo[] fields = eventSource.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            //if it exists and is not already in list
            if (typeof(AudioPositionalEvent).IsAssignableFrom(field.FieldType) && !EventGroupListContains(eventGroups, field.Name))
            {
                eventGroups.Add(new EventSoundGroup(field.Name));
            }
        }

        //debug
        Debug.Log("Number of events found: " + eventGroups.Count);
    }

    //checks whether an event is already in list to no redundantly add it multiple times to list
    public bool EventGroupListContains(List<EventSoundGroup> list, string name)
    {
        foreach (var group in list)
            if (name == group.eventName) return true;

        return false;
    }

    //gets all events at on runtime start using the previously saved names
    //unwieldly but grabbing the events directly through reflection outside of runtime doesnt seem to work
    public void GetEvents()
    {
        FieldInfo[] fields = eventSource.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (eventsDict.ContainsKey(field.Name))
                eventsDict[field.Name].eventRef = field.GetValue(eventSource) as AudioPositionalEvent;
        }
    }

    public void ClearEvents()
    {
        eventGroups.Clear();
    }

    public Transform GetObjectTransformFromPool()
    {
        return ObjPool.SupplyObject<Transform>();
    }
}