using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;
using ScriptableObjects;

[RequireComponent(typeof(GenericObjPool))]
public class AudioManagerScript: MonoBehaviour, GenericObjPoolUserI
{
    private AudioSource musicSource;
    public SoundScripObj activeMusic;

    public static AudioManagerScript instance;
    public Sound[] sounds;

    [SerializeField]
    private GenericObjPool objPool;
    public GenericObjPool ObjPool => throw new NotImplementedException();

    // Start is called before the first frame update
    void Awake()
    {
        //SINGLETON
        //persist across scene loads
        if(instance==null) instance = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        //GENERATE AUDIO SOURCES As CHILDREN
        foreach(Sound s in sounds)
        {
            if (s.name.Length == 0) s.name = s.clip.name;

            GameObject temp = new GameObject(s.name);
            temp.transform.parent = transform;

            s.source = temp.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = false;

            s.source.outputAudioMixerGroup = s.audioMixGroup;
        }
    }

    private void Start()
    {
        if(objPool==null)
            objPool = GetComponent<GenericObjPool>();

        if (activeMusic != null) {
            musicSource = activeMusic.Play(createNew: true);
            musicSource.loop = true;
        }
    }

    public AudioSource GetAudioSource()
    {
        return objPool.SupplyObject<AudioSource>();
    }

    //PLAYS SOUND WITH MATCHING NAME
    //SEARCHES THROUGH ARRAY
    public void PlaySound(string sound)
    {
        Sound s = null;
        foreach (Sound temp in sounds) if (temp.name.Equals(sound)) s = temp;
        //Debug.Log("Playing Sound " + sound);
        if (s.source != null) s.source.Play();
        else Debug.LogError("cant find sound: " + sound);
        return;
    }

    public void PlaySoundRandomPitch(string sound, float pitchDif)
    {
        Sound s = null;
        foreach (Sound tempS in sounds) if (tempS.name.Equals(sound)) s = tempS;

        //null check
        if(s==null || s.source==null)
        {
            Debug.LogError("cant find sound: " + sound);
            return;
        }

        //create sound instance
        GameObject temp = new GameObject(s.name);
        temp.transform.parent = transform;

        AudioSource sInstance = temp.AddComponent<AudioSource>();
        sInstance.clip = s.clip;
        sInstance.volume = s.volume;
        sInstance.pitch = s.pitch + UnityEngine.Random.Range(-pitchDif, pitchDif);
        sInstance.playOnAwake = false;

        sInstance.outputAudioMixerGroup = s.audioMixGroup;

        if (sInstance != null) sInstance.Play();
        //delete instance
        GameObject.Destroy(temp, 1f);
    }

    public void StartSoundLoop(string sound, float time)
    {
        StartCoroutine(PlayLoopForSeconds(sound, time));
    }

    public IEnumerator PlayLoopForSeconds(string soundName, float time)
    {
        Sound s = null;
        foreach (Sound temp in sounds) if (temp.name.Equals(soundName)) s = temp;

        //null check
        if (s.source == null)
        {
            Debug.LogError("cant find sound: " + soundName);
            yield break;
        }

        s.source.loop = true;
        s.source.Play();

        yield return new WaitForSecondsRealtime(time);
        s.source.loop = false;
        s.source.Stop();

        yield break;
    }

    public void PlaySoundDelaySeconds(string soundName, float delayTime)
    {
        StartCoroutine(PlaySoundDelaySecondsCoroutine(soundName, delayTime)) ;
    }

    //PLAYS SOUNDS AFTER FLOAT SECONDS
    IEnumerator PlaySoundDelaySecondsCoroutine(string soundName, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        PlaySound(soundName);
    }

    public void ObjSetup(GameObject obj)
    {
        obj.AddComponent<AudioSource>();
    }

    public Transform GetObjectTransformFromPool()
    {
        return ObjPool.SupplyObject<Transform>();
    }
}
