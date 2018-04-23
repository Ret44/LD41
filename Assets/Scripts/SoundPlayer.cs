using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    Click,
    CyclopsDeath,
    CyclopsHit,
    Match3,
    MenuScroll,
    PlaceTower,
    SwapTokens,
    TokensHit,
    TowerAttacks,
    ZeusHurt
}

public class SoundPlayer : MonoBehaviour
{

    public static SoundPlayer instance;

    public AudioSource click;
    public AudioSource cyclopsDeath;
    public AudioSource cyclopsHit;
    public AudioSource match3;
    public AudioSource menuScroll;
    public AudioSource placeTower;
    public AudioSource swapTokens;
    public AudioSource tokensHit;
    public AudioSource towerAttacks;
    public AudioSource zeusHurt;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static void PlaySound(Sound sound, bool randomPitch)
    {
        AudioSource source = null;
        switch(sound)
        {
            case Sound.Click: source = instance.click; break;
            case Sound.CyclopsDeath: source = instance.cyclopsDeath; break;
            case Sound.CyclopsHit: source = instance.cyclopsHit; break;
            case Sound.Match3: source = instance.match3; break;
            case Sound.MenuScroll: source = instance.menuScroll; break;
            case Sound.PlaceTower: source = instance.placeTower; break;
            case Sound.SwapTokens: source = instance.swapTokens; break;
            case Sound.TokensHit: source = instance.tokensHit; break;
            case Sound.TowerAttacks: source = instance.towerAttacks; break;
            case Sound.ZeusHurt: source = instance.zeusHurt; break;
        }
        if(source!=null)
        {
            if (randomPitch)
                source.pitch = Random.Range(0.85f, 1.25f);
            else
                source.pitch = 1f;
            source.Play();
        }
    }
}
