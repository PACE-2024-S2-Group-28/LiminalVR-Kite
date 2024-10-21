﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

public class RadioCallouts : MonoBehaviour
{
    [SerializeField]
    private AudioSource radioSource;

    [SerializeField]
    private AudioClip voiceStart;

    [SerializeField]
    private SoundScripObj sfxRadioStatic;

    void Start()
    {
        StartCoroutine(PlayVoiceline(voiceStart));
    }

    public void PlayLine(AudioClip clip) {
        StartCoroutine(PlayVoiceline(clip));
    }

    private IEnumerator PlayVoiceline(AudioClip clip)
    {
        radioSource.Stop();
        yield return new WaitForSeconds(.2f);

        radioSource.clip = clip;

        sfxRadioStatic?.Play();
        yield return new WaitForSeconds(0.2f);
        radioSource.Play();
        yield return new WaitForSeconds(clip.length);
        sfxRadioStatic?.Play();
    }
}
