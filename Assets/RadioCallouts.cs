using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

public class RadioCallouts : MonoBehaviour
{
    private static RadioCallouts instance;
    public static RadioCallouts Instance => instance;

    [SerializeField]
    private AudioSource radioSource;

    [SerializeField]
    private AudioClip voiceStart;

    [SerializeField]
    private SoundScripObj sfxRadioStatic;

    private void Awake()
    {
        if(instance!=null) {
            GameObject.Destroy(this);
            return;
        }
        instance = this;
    }

    void Start()
    {
        PlayLine(voiceStart);
    }

    public void PlayLine(AudioClip clip) {
        StopAllCoroutines();
        StartCoroutine(PlayVoiceline(clip));
    }

    public static void PlayVO(AudioClip line)
    {
        instance.PlayLine(line);
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
