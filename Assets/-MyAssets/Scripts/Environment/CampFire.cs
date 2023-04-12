using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    [Tooltip("Her 0.5 sn de bir hasar alýr")]public int damage;

    public static List<AudioSource> sources = new();

    //Hasar sistemi için
    GameObject target;

    private void Start()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("soundLevel");
        sources.Add(GetComponent<AudioSource>());
    }
    public static void SetVolume()
    {
        int a = sources.Count;
        float b = PlayerPrefs.GetFloat("soundLevel");

        for (int i = 0; i < a; i++)
        {
            if (sources[i]) sources[i].volume = b;
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
            InvokeRepeating(nameof(GetBurnDamage), 0, 0.5f);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke(nameof(GetBurnDamage));
        }
    }

    void GetBurnDamage()
    {
        if (FirstOOP.FiftyChance())
            AudioManager.instance.PlaySound("FireBurn1");
        else
            AudioManager.instance.PlaySound("FireBurn2");
        target.GetComponent<KamHealth>().GetDamage(damage, 1);
    }
}
