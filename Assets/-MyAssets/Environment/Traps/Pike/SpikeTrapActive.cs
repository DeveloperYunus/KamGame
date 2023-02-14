using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SpikeTrapActive : MonoBehaviour
{
    public GameObject spikes;
    public float damage;
    public GameObject[] spikeShadows;

    bool onlyFirstTime;

    private void Start()
    {
        onlyFirstTime = true;
        spikes.GetComponent<SpikeDamage>().damage = damage;
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
        if (FirstOOP.FiftyChance()) spikes.GetComponent<SpikeDamage>().triggered1.Play();
        else spikes.GetComponent<SpikeDamage>().triggered2.Play();

        spikes.GetComponent<Transform>().DOLocalMoveY(-0.6f, 0.2f);
 
        yield return new WaitForSeconds(Random.Range(0.4f, 0.8f));
        spikes.GetComponent<Transform>().DOLocalMoveY(0.03f, 0.3f);
        
        int aa = spikeShadows.Length;
        for (int i = 0; i < aa; i++)//mýzrak golgeleri ortaya cýkýyor
        {
            spikeShadows[i].GetComponent<SpriteRenderer>().DOFade(1, 0.15f);
        }

        int b = Random.Range(0, 2);
        if (b == 0) spikes.GetComponent<SpikeDamage>().pikeUp1.Play();
        else spikes.GetComponent<SpikeDamage>().pikeUp2.Play();

        spikes.GetComponent<BoxCollider2D>().enabled = true;
        
        yield return new WaitForSeconds(0.3f);
        spikes.GetComponent<SpikeDamage>().IsDownRise = true;
    }
}
