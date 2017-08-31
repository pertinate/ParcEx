using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SourceType
{
    Sfx,
    Music
}
public enum PlayType
{
    Repeat,
    Once,
    RepeatOver
}

public class AudioManager : MonoBehaviour {
    private AudioListener playerListener()
    {
        return Drone.instance.gameObject.GetComponent<AudioListener>();
    }

    public static AudioManager instance;

    public bool useTestClip = false;
    private bool finishedLoading = false;

    public AudioClip[] allClips;

    public AudioClip testClip;

    public Slider global, sfx, music;
    public Toggle mute;

    public static List<AudioSource> allSources = new List<AudioSource>();
    public static List<AudioSource> musicSources = new List<AudioSource>();
    public static List<AudioSource> sfxSources = new List<AudioSource>();

    private void Awake()
    {
        instance = this;
        global.onValueChanged.AddListener((float f) =>
        {
            AudioListener.volume = f;
            GPServices.m_saveSystem.CurrentSave.globalAudio = f;
            if (finishedLoading)
                mute.isOn = false;
        });
        mute.onValueChanged.AddListener((bool b) =>
        {
            Mute(b);
            GPServices.m_saveSystem.CurrentSave.muted = b;
        });
        sfx.onValueChanged.AddListener((float f) =>
        {
            GPServices.m_saveSystem.CurrentSave.sfxAudio = f;
            if (finishedLoading)
                mute.isOn = false;
        });
        music.onValueChanged.AddListener((float f) =>
        {
            GPServices.m_saveSystem.CurrentSave.musicAudio = f;
            if (finishedLoading)
                mute.isOn = false;
        });
        global.value = GPServices.m_saveSystem.CurrentSave.globalAudio;
        global.onValueChanged.Invoke(GPServices.m_saveSystem.CurrentSave.globalAudio);
        music.value = GPServices.m_saveSystem.CurrentSave.musicAudio;
        music.onValueChanged.Invoke(GPServices.m_saveSystem.CurrentSave.musicAudio);
        sfx.value = GPServices.m_saveSystem.CurrentSave.sfxAudio;
        sfx.onValueChanged.Invoke(GPServices.m_saveSystem.CurrentSave.sfxAudio);
        mute.isOn = GPServices.m_saveSystem.CurrentSave.muted;
        mute.onValueChanged.Invoke(GPServices.m_saveSystem.CurrentSave.muted);
        PostLoad();
    }

    private void Start()
    {
        
    }
    private void PostLoad()
    {
        finishedLoading = true;
        if (useTestClip)
        {
            AddSource(SourceType.Music, testClip, PlayType.Repeat);
        }
    }

    public static void AddSource(SourceType type, AudioClip clip, PlayType ptype = PlayType.Once, int replay = 0)
    {
        GameObject go = new GameObject();
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        allSources.Add(source);

        switch (type)
        {
            case SourceType.Sfx:

                if (!instance.mute.isOn)
                    source.volume = GPServices.m_saveSystem.CurrentSave.sfxAudio;
                else
                    source.volume = 0;
                instance.sfx.onValueChanged.AddListener((float f) => source.volume = f);
                sfxSources.Add(source);
                break;
            case SourceType.Music:
                if (!instance.mute.isOn)
                    source.volume = GPServices.m_saveSystem.CurrentSave.musicAudio;
                else
                    source.volume = 0;
                instance.music.onValueChanged.AddListener((float f) => source.volume = f);
                musicSources.Add(source);
                break;
        }
        switch (ptype)
        {
            case PlayType.Once:
                source.loop = false;
                source.Play();
                break;
            case PlayType.Repeat:
                source.loop = true;
                source.Play();
                break;
        }
    }

    public void Mute(bool _mute)
    {
        if (_mute)
        {
            foreach(AudioSource s in allSources)
            {
                s.volume = 0f;
            }
        }
        else
        {
            foreach(AudioSource s in musicSources)
            {
                s.volume = GPServices.m_saveSystem.CurrentSave.musicAudio;
            }
            foreach(AudioSource s in sfxSources)
            {
                s.volume = GPServices.m_saveSystem.CurrentSave.sfxAudio;
            }
        }
    }
}

public class FloatEvent : UnityEvent<float> { }
