using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FallingRock : MonoBehaviour
{
    public bool isGiveDamage;
    public float damage;
    
    bool oneTime;
    float zaman;

    [HideInInspector] public float liveTime;

    private void Start()
    {
        oneTime = true;
        isGiveDamage = true;
        StartCoroutine(DieTimer());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGiveDamage && !other.CompareTag("Meteor"))
        {
            isGiveDamage = false;
            StartCoroutine(SetLayer());
            int a = Random.Range(0, 3);

            if (other.CompareTag("Player"))
            {
                other.GetComponent<KamHealth>().GetDamage(damage, 6);
                //ete çarpma ses kodu
            }

            if(other.CompareTag("Enemy"))
            {
                other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 6);
                //ete çarpma ses kodu
            }

            if (oneTime && other.CompareTag("Ground"))
            {
                oneTime = false;
                isGiveDamage = false;
                gameObject.layer = 10;  //FallRock
                //yere çarpma ses kodu
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
