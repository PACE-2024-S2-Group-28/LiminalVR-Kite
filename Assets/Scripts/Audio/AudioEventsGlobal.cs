using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Events;

public class AudioEventsGlobal : MonoBehaviour
{
    public static AudioEventsGlobal instance;

    public AudioPositionalEvent positionTest = new AudioPositionalEvent();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        else GameObject.Destroy(this);
    }
}
