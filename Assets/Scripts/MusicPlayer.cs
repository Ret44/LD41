using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicTrack
{
    Title,
    Intro,
    Build,
    Defend,
    Win,
    Lose
}

public class MusicPlayer : MonoBehaviour {

    public static MusicPlayer instance;

    public AudioSource audioSource;

    public AudioClip titleTrack;
    public AudioClip introTrack;
    public AudioClip buildTrack;
    public AudioClip defendTrack;
    public AudioClip winTrack;
    public AudioClip loseTrack;

    [SerializeField]
    private MusicTrack _currentTrack;
    public static MusicTrack CurrentTrack
    {
        get { return instance._currentTrack; }
    }

    public void Awake()
    {
        instance = this;
    }

    public static void PlayTrack(MusicTrack track, bool loop)
    {
        AudioClip trackToPlay = null;
        switch(track)
        {
            case MusicTrack.Title: trackToPlay = instance.titleTrack; break;
            case MusicTrack.Intro: trackToPlay = instance.introTrack; break;
            case MusicTrack.Build: trackToPlay = instance.buildTrack; break;
            case MusicTrack.Defend: trackToPlay = instance.defendTrack; break;
            case MusicTrack.Win: trackToPlay = instance.winTrack; break;
            case MusicTrack.Lose: trackToPlay = instance.loseTrack; break;
        }
        if(trackToPlay!=null)
        {
            instance.audioSource.Stop();
            instance.audioSource.loop = loop;
            instance.audioSource.clip = trackToPlay;
            instance.audioSource.Play();
        }
    }
}
