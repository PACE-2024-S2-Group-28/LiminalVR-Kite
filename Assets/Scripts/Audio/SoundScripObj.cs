using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace ScriptableObjects 
{

    [CreateAssetMenu(fileName = "NewSoundEffect", menuName = "Audio/New Sound Effect")]
    public class SoundScripObj : ScriptableObject
    {
        private static readonly float SEMITONES_CONVERSION = 1.05946f;
        public enum PlayOrder { in_order, reverse, random };

        #region config

        public AudioMixerGroup mixGroup;
        public AudioClip[] clips;
        
        [SerializeField]
        [MinMaxSlider(0f, 1f)]
        private Vector2 volumeSlider = Vector2.one*.5f;

        
        private int playIndex;
        public PlayOrder playOrder = PlayOrder.in_order;

        public bool useSemitones;

        [HideIf("useSemitones")]
        [SerializeField]
        [MinMaxSlider(.1f, 3f)]
        private Vector2 pitchSlider = Vector2.one;

        [Range(-10, 10)] 
        [ShowIf("useSemitones")]
        public int semitoneMin;
        [Range(-10, 10)] 
        [ShowIf("useSemitones")]
        public int semitoneMax;

        public AudioRolloffMode rolloffMode;
        
        [MinMaxSlider(1f, 1000f)]
        public Vector2 rolloffDistance = new Vector2(1,500);

        #endregion

    #region PreviewCode
    #if UNITY_EDITOR
        private AudioSource previewer;

        private void OnEnable()
        {
            previewer = EditorUtility
                .CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave,
                    typeof(AudioSource)).GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            DestroyImmediate(previewer.gameObject);
        }

        public void PlayPreview()
        {
            Play(audioSourceParam: previewer);
        }

        public void StopPreview()
        {
            previewer.Stop();
        }

     #endif
     #endregion

        public void SyncPitchAndSemitones()
        {
            if (useSemitones)
            {
                pitchSlider.x = Mathf.Pow(SEMITONES_CONVERSION, semitoneMin);
                pitchSlider.y = Mathf.Pow(SEMITONES_CONVERSION, semitoneMax);
            }
            else
            {
                semitoneMin = Mathf.RoundToInt(Mathf.Log10(pitchSlider.x)/Mathf.Log10(SEMITONES_CONVERSION));
                semitoneMax = Mathf.RoundToInt(Mathf.Log10(pitchSlider.y) / Mathf.Log10(SEMITONES_CONVERSION));
            }
        }

        public AudioClip GetClip()
        {
            var clip = clips[playIndex >= clips.Length ? 0 : playIndex];

            switch (playOrder)
            {
                case PlayOrder.in_order:
                    playIndex = (++playIndex) % clips.Length;
                    break;
                case PlayOrder.reverse:
                    playIndex = (--playIndex) % clips.Length;
                    break;
                case PlayOrder.random:
                    playIndex = Random.Range(0, clips.Length);
                    break;
            }

            return clip;
        }

        public AudioSource Play(Transform sParent = null, Vector3? wPos = null, AudioSource audioSourceParam = null, float volumeMul = 1, bool createNew = false)
        {
            //Debug.Log("Trying to play sound: " + this);
#if UNITY_EDITOR
            if (!Application.isPlaying) audioSourceParam = previewer;
#endif

            //null check error
            if(clips.Length == 0)
            {
                Debug.LogError(this + " is missing any sound clips");
                return null;
            }

            bool destroySource = false;
            AudioSource source;
            //try parameter audio source
            if (audioSourceParam != null) {
                source = audioSourceParam;
                //destroySource = true;
            }
            //try audio manager obj pool audio source
            else if (!createNew && (source = AudioManagerScript.instance.GetAudioSource()) != null);
            //create new gameobject if all else fails
            else
            {
                var _obj = new GameObject(this.ToString(), typeof(AudioSource));
                _obj.transform.parent = AudioManagerScript.instance.transform;
                source = _obj.GetComponent<AudioSource>();
                destroySource = true;
            }

            //set config
            if(mixGroup) source.outputAudioMixerGroup = mixGroup;
            source.clip = GetClip();
            source.volume = Random.Range(volumeSlider.x, volumeSlider.y)*volumeMul;
            source.pitch = useSemitones
                ? Mathf.Pow(SEMITONES_CONVERSION, Random.Range(semitoneMin, semitoneMax))
                : Random.Range(pitchSlider.x, pitchSlider.y);

            //position section
            if (sParent)
            {
                source.transform.parent = sParent;
                source.transform.localPosition = Vector3.zero;
                source.spatialBlend = 1f;
            }
            else if (wPos.HasValue)
            {
                source.transform.position = wPos.Value;
                source.spatialBlend = 1f;
            }
            else source.spatialBlend = 0f;

            source.rolloffMode = rolloffMode;

            source.minDistance = rolloffDistance.x;
            source.maxDistance = rolloffDistance.y;

            source.Play();

            if (destroySource)
                Destroy(source.gameObject, source.clip.length / source.pitch);

            return source;
        }

        private void OnValidate()
        {
            SyncPitchAndSemitones();
        }
    }


}
