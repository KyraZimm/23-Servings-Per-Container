using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
    private static AudioMixerGroup m_musicMixerGroup;
    private static AudioMixerGroup m_sfxMixerGroup;

    private static Transform m_poolRoot = null;
    private static Queue<AudioSource> m_pooledAudioSources = new Queue<AudioSource>();
    private static List<AudioTimer> m_activeSources = new List<AudioTimer>();

    private struct AudioTimer {
        public AudioSource PlayingSource;
        public float TimeClipStarted;
        public float ClipLength;
    }

    public static void Init(Config config) {
        m_musicMixerGroup = config.MusicMixerGroup;
        m_sfxMixerGroup = config.SFXMixerGroup;

        InitAudioPool(config.DefaultAudioSourcePoolSize);
    }

    public static void Update() {

        //check running audio sources to see if clips are done playing. return to pool when completed
        for (int i = m_activeSources.Count-1; i >= 0; i--) {
            if (Time.time - m_activeSources[i].TimeClipStarted >= m_activeSources[i].ClipLength) {
                ReturnSourceToPool(m_activeSources[i].PlayingSource);
                m_activeSources.RemoveAt(i);
            }
        }
    }

    private static void InitAudioPool(int startingPoolSize) {
        m_poolRoot = new GameObject("Pooled Audio Sources").transform;
        for (int i = 0; i < startingPoolSize; i++)
            CreatePooledAudioSource();
    }

    private static void CreatePooledAudioSource() {
        AudioSource source = new GameObject("Audio Source").AddComponent<AudioSource>();
        source.transform.SetParent(m_poolRoot);
        source.playOnAwake = false;

        ReturnSourceToPool(source);
    }

    private static void ReturnSourceToPool(AudioSource source) {
        source.Stop();
        source.clip = null;
        source.loop = false;

        m_pooledAudioSources.Enqueue(source);
    }

    #region Play Methods
    public static void PlayMusic(AudioClip clip) { Play(clip, m_musicMixerGroup, true); }
    public static void PlaySFX(AudioClip clip) { Play(clip, m_sfxMixerGroup, false); }
    public static void Play(AudioClip clip, AudioMixerGroup group) { Play(clip, group, false); }
    public static void Play(AudioClip clip, AudioMixerGroup group, bool loop) {
        if (m_pooledAudioSources.Count == 0)
            CreatePooledAudioSource();

        AudioSource source = m_pooledAudioSources.Dequeue();
        source.outputAudioMixerGroup = group;
        source.clip = clip;

        //if audio is meant to be looping, set audio source to loop
        if (loop) source.loop = true;

        //else, set a timer to check when the clip is done playing to return to pool
        else {
            AudioTimer timer = new AudioTimer();
            timer.PlayingSource = source;
            timer.ClipLength = clip.length;
            timer.TimeClipStarted = Time.time;
        }

        source.Play();
    }

    #endregion

}
