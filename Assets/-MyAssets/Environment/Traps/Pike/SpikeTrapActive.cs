using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SpikeTrapActive : MonoBehaviour
{
    public SpikeDamage spikes;
    public float damage;
    public GameObject[] spikeShadows;

    bool onlyFirstTime;

    private void Start()
    {
        onlyFirstTime = true;
        spikes.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(onlyFirstTime && (other.CompareTag("Enemy") || other.CompareTag("Player")))
        {
            onlyFirstTime = false;
            StartCoroutine(TrapIsActivate());
        }
    }

    IEnumerator TrapIsActivate()
    {
        float b = PlayerPrefs.GetFloat("soundLevel");       //ses ayarý yapmak için
        spikes.triggered1.volume = b;
        spikes.pikeUp1.volume = b;
        spikes.pikeUp2.volume = b;

        spikes.triggered1.Play();

        spikes.GetComponent<Transform>().DOLocalMoveY(-0.6f, 0.2f);
 
        yield return new WaitForSeconds(Random.Range(0.4f, 0.8f));
        spikes.GetComponent<Transform>().DOLocalMoveY(0.03f, 0.3f);
        
        int aa = spikeShadows.Length;
        for (int i = 0; i < aa; i++)//mýzrak golgeleri ortaya cýkýyor
        {
            spikeShadows[i].GetComponent<SpriteRenderer>().DOFade(1, 0.15f);
        }

        if (FirstOOP.FiftyChance()) spikes.pikeUp1.Play();
        else spikes.pikeUp2.Play();

        spikes.GetComponent<BoxCollider2D>().enabled = true;
        
        yield return new WaitForSeconds(0.3f);
        spikes.IsDownRise = true;
    }
}
