using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    [HideInInspector] public float currentVolume;       //bu her ses ayarýnda güncellensin

    public static bool inCave;                          //magaranýn içindemiyiz dýþýndamýyýz
    float bgTimer, inCaveTimer;                         //background müziklerinin timer kýsmý ve magaraya girince hesaplamalarý ksýtlý süreliðine yapmasý için süre sayacý
    float inCaveSound;
    string currentBGMelody;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        currentVolume = PlayerPrefs.GetFloat("soundLevel");
        inCaveSound = 0;
        inCaveTimer = 0;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * currentVolume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    private void Update()
    {
        if (bgTimer < 0)
        {
            if (FirstOOP.FiftyChance())
            {
                //bgTimer = clip'in uzunluðu + 2;
                PlaySound("forestBG1");
                currentBGMelody = "forestBG1";
            }
            else
            {
                //bgTimer = clip'in uzunluðu + 2;
                PlaySound("forestBG2");
                currentBGMelody = "forestBG2";
            }
        }
        else bgTimer -= Time.deltaTime;

        if (inCaveTimer > 0)
        {
            inCaveTimer -= Time.deltaTime;

            if (inCave)
            {
                inCaveTimer = 1.5f;

                inCaveSound = Mathf.Lerp(inCaveSound, 1, 0.1f);
                PlaySound("caveBG");

                SetSound(currentBGMelody, 1 - inCaveSound);             //mevcut bg yi sýfýra doðru götür
                SetSound("caveBG", inCaveSound);
            }
            else
            {
                inCaveSound = Mathf.Lerp(inCaveSound, 0, 0.1f);

                SetSound(currentBGMelody, 1 - inCaveSound);
                SetSound("caveBG", inCaveSound);
            }
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);       
        if (s == null) return;                                                      //eðer o isimde bir ses dosyasý yoksa return et
        SetSound(name, currentVolume);
        s.source.Play();
    }
    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);                  
        if (s == null) return;                                                      
        s.source.Stop();
    }
    public void SetSound(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.volume = volume * s.volume;
    }

    public void SetGV(float volume)                     //Level select menu'deki SetGlobalVolume() fontksiyonu için yazýldý
    {
        PlaySound("slider");
        currentVolume = volume;
        SetSound("bgMusic", volume);
    }
}



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