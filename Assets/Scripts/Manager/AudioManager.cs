using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public Sound[] sounds;

    void Awake()
    {
        if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        PlayGameBg();
    }

    public void PlayGameBg()
    {
        Play("GamePlay", true);
    }

    public void StopGameBg()
    {
        Stop("GamePlay");
    }

    void Stop(string name)
    {
        var sound = Array.Find(sounds, s => s.name == name);
        sound?.source.Stop();
    }

    public void TileSelect()
    {
        Play("TileSelect");
    }

    public void TileMove()
    {
        Play("TileMove");
    }

    public void Hint()
    {
        Play("Hint");
    }

    public void Shuffle()
    {
        Play("Shuffle");
    }

    public void ButtonClick()
    {
        Play("ButtonClick");
    }

    public void Victory()
    {
        Play("Victory");
    }

    public void Defeat()
    {
        Play("Defeat");
    }

    private void Play(string name, bool isMusic = false)
    {
        if (isMusic && PlayerPrefsHelper.instance.MusicOn == 0) return;
        if (!isMusic && PlayerPrefsHelper.instance.SoundOn == 0) return;
        var sound = Array.Find(sounds, s => s.name == name);
        sound?.source.Play();
    }
}