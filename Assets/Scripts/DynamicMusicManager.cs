using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using NaughtyAttributes;

public class DynamicMusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    [MinMaxSlider(0f, 1f)]
    private Vector2 minMaxMusicVol = new Vector2(.4f, .8f);

    [SerializeField]
    private bool selfTimed;
    
    [SerializeField]
    private AudioSource[] tracks;
    private int currentTrack;
    private int fadingTrack;
    private bool fading = false;

    [SerializeField]
    private float crossfadeTime;
    private float crossfadeSpeed;

    [SerializeField]
    private float trackLength;
    private float timer;

    bool musicOn = false;

    void Start()
    {
        foreach(AudioSource track in tracks){
            track.Stop();
            track.volume = 0;
        }
        tracks[0].volume = 1f;

        crossfadeSpeed = 1f/crossfadeTime;

        DynamicEvents.Instance.eventDict[MUSIC_START] += MusicStart;

        foreach (var e in WAVE_START) {
            DynamicEvents.Instance.eventDict[e] += VolumeUp;
        }
        foreach (var e in WAVE_END) {
            DynamicEvents.Instance.eventDict[e] += VolumeDown;
        }
    }

    const string MUSIC_START = "musicStart";
    private string[] WAVE_START = new string[] {
        //"wave1start",
        "wave2start",
        "wave3start"
    };
    private string[] WAVE_END = new string[] {
        "wave1end",
        "wave2end",
        "wave3end"
    };

    private void OnEnable()
    {
        
    }

    private void MusicStart(float useless)
    {
        foreach (AudioSource track in tracks) {
            track.Play();
        }
    }

    private void VolumeUp(float ignore)
    {
        internalVolumMul = 1f;
    }
    private void VolumeDown(float ignore)
    {
        internalVolumMul = .3f;
    }

    private float internalVolumMul = .5f;

    void Update()
    {
        if(selfTimed){
            timer += Time.deltaTime;

            if(!fading && timer >= trackLength){
                IncreaseIntensity();
            }
        }
        
        if(fading){
            float fadeTimeDelta = Time.deltaTime * crossfadeSpeed;

            tracks[fadingTrack].volume = Mathf.Clamp01(tracks[fadingTrack].volume - fadeTimeDelta);

            float newVol = Mathf.Clamp01(tracks[currentTrack].volume + fadeTimeDelta);
            tracks[currentTrack].volume = newVol;

            if(newVol >= 1f)
            {
                fading = false;
                timer = 0;
                tracks[currentTrack].volume = 1f;
                tracks[fadingTrack].volume = 0f;
            }
        }

        //overall audio volume
        float volume = Mathf.Lerp(minMaxMusicVol.x, minMaxMusicVol.y, AsteroidGameManager.Instance.SampleDifficultyCurve(AsteroidGameManager.Instance.GameProgress-.1f));
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume * internalVolumMul) * 20);
    }

    public void IncreaseIntensity()
    {
        if(currentTrack < tracks.Length - 1){
            tracks[fadingTrack].volume = 0f;
            tracks[currentTrack].volume = 1f;

            fadingTrack = currentTrack;
            currentTrack++;
            fading = true;
        }
    }

    public void SetIntensity(int i)
    {
        if(i < tracks.Length){
            tracks[fadingTrack].volume = 0f;
            tracks[currentTrack].volume = 1f;

            fadingTrack = currentTrack;
            currentTrack = i;
            fading = true;
        }
    }
}
