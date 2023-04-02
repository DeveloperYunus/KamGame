using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FallingRock : MonoBehaviour
{
    [HideInInspector] public bool isGiveDamage;
    [HideInInspector] public float damage;
    [HideInInspector] public float liveTime;

    public AudioSource toFlesh1, toFlesh2;
    public AudioSource toGround1, toGround2;

    bool oneTime;


    private void Start()
    {
        oneTime = true;
        isGiveDamage = true;

        StartCoroutine(DieTimer());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        float b = PlayerPrefs.GetFloat("soundLevel");       //ses ayarýný yapmak için

        toFlesh1.volume = b;
        toFlesh2.volume = b;
        toGround1.volume = b;
        toGround2.volume = b; 

        if (isGiveDamage && !other.CompareTag("Meteor"))
        {
            isGiveDamage = false;
            StartCoroutine(SetLayer());

            if (other.CompareTag("Player"))
            {
                other.GetComponent<KamHealth>().GetDamage(damage, 6);

                if (FirstOOP.FiftyChance())
                    toFlesh1.Play();
                else
                    toFlesh2.Play();
            }

            if(other.CompareTag("Enemy"))
            {
                other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 6);

                if (FirstOOP.FiftyChance())
                    toFlesh1.Play();
                else
                    toFlesh2.Play();
            }

            if (oneTime && other.CompareTag("Ground"))
            {
                oneTime = false;
                isGiveDamage = false;
                gameObject.layer = 10;  //FallRock

                if (FirstOOP.FiftyChance())
                    toGround1.Play();               
                else
                    toGround2.Play();               
            }
        }
    }

    IEnumerator DieTimer()
    {
        yield return new WaitForSeconds(liveTime + Random.Range(1f, -1f));
        GetComponent<SpriteRenderer>().DOFade(0, 1f);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    IEnumerator SetLayer()
    {
        yield return new WaitForSeconds(0.4f);
        gameObject.layer = 10;  //FallRock
    }
}
