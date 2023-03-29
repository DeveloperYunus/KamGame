using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider sl;                                   //soundslider
    public bool isBurning;                              //Yanan levelleri gelince bunu etkinleþtirecez ve yanma bg sesi çalacak
    public Sound[] sounds;
    public static AudioManager instance;
    [HideInInspector] public float currentVolume;       //bu her ses ayarýnda güncellensin

    public static bool inCave;                          //magaranýn içindemiyiz dýþýndamýyýz
    [HideInInspector] public float inCaveTimer;         //magaraya girince hesaplamalarý ksýtlý süreliðine yapmasý için süre sayacý
    float bgTimer;                                      //background müziklerinin timer kýsmý sayacý 
    float inCaveSound;
    string currentBGMelody;

    bool isSlFirst;                                     //Bu olmazsa oyun açýldýðýnda slider sesi çalýyor

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

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * currentVolume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        isSlFirst = true;

        currentVolume = PlayerPrefs.GetFloat("soundLevel");
        sl.value = currentVolume * 10;

        inCaveSound = 0;
        inCaveTimer = 0;
    }
    private void Update()
    {
        if (bgTimer < 0)
        {
            if (!isBurning)
            {
                if (FirstOOP.FiftyChance())
                {
                    bgTimer = GetClip("Forest1").length;
                    PlaySound("Forest1");
                    currentBGMelody = "Forest1";
                }
                else
                {
                    bgTimer = GetClip("Forest2").length;
                    PlaySound("Forest2");
                    currentBGMelody = "Forest2";
                }
            }
        }
        else 
            bgTimer -= Time.deltaTime;

        if (inCaveTimer > 0)
        {
            inCaveTimer -= Time.deltaTime;

            if (inCave)
            {
                inCaveSound = Mathf.Lerp(inCaveSound, 1, 0.06f);
                PlaySoundOne("Cave");

                SetSound(currentBGMelody, (1 - inCaveSound) * currentVolume);             //mevcut bg yi sýfýra doðru götür
                SetSound("Cave", inCaveSound * currentVolume);
            }
            else
            {
                inCaveSound = Mathf.Lerp(inCaveSound, 0, 0.06f);

                SetSound(currentBGMelody, (1 - inCaveSound) * currentVolume);
                SetSound("Cave", inCaveSound * currentVolume);
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
    /// <summary>   Hali hazýrda çalmakta olan seslei caldýrmaz.   </summary>
    public void PlaySoundOne(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;

        if (!s.source.isPlaying)                                                    //eðer bu ses çalmýyorsa bu sesi çal
        {   
            SetSound(name, currentVolume);
            s.source.Play();
        }
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
    public AudioClip GetClip(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return null;
        return s.clip;
    }

    public void SetGV()                     //Level select menu'deki SetGlobalVolume() fontksiyonu için yazýldý
    {
        if (!isSlFirst)
            PlaySound("SoundSlider");

        isSlFirst = false;

        currentVolume = sl.value * 0.1f;
        PlayerPrefs.SetFloat("soundLevel", currentVolume);

        SetSound(currentBGMelody, (1 - inCaveSound) * currentVolume);             //mevcut bg yi sýfýra doðru götür
        SetSound("Cave", inCaveSound * currentVolume);
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