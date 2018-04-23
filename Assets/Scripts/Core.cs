using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ammo
{
    public TileType tile;
    public int value;
    public UnityEngine.UI.Text counter;

    public void AddValue(int val)
    {
        value += val;
        if (counter != null) counter.text = value.ToString();
    }

    public bool SubValue(int val)
    {
        if (value < val) return false;
        value -= val;
        if (counter != null) counter.text = value.ToString();
        return true;
    }

    public void SetValue(int val)
    {
        value = val;
        if (counter != null) counter.text = value.ToString();
    }
}

public class Core : MonoBehaviour {
    
    public static Core instance;
    public int Coins;
    public Ammo[] ammoValues;

    public Animator zeusAnimator;

    public UnityEngine.UI.Image zeusHPFill;

    public float zeusHP = 0.0f;

    public static Dictionary<TileType, Ammo> Ammo;

    void Awake()
    {
        instance = this;
        Ammo = new Dictionary<TileType, Ammo>();
        for(int i=0;i<ammoValues.Length;i++)
        {
            Ammo.Add(ammoValues[i].tile, ammoValues[i]);
        }
    }

	// Use this for initialization
	void Start () {
        MusicPlayer.PlayTrack(MusicTrack.Defend, true);
	}

    public static void TriggerZeus()
    {
        SoundPlayer.PlaySound(Sound.ZeusHurt, true);
        instance.zeusAnimator.SetTrigger("hit");
        instance.zeusHP += 0.2f;
        instance.zeusHPFill.fillAmount = instance.zeusHP;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
