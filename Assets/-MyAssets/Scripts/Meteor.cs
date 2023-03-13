using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public ParticleSystem smoke;
    public ParticleSystem explode;

    [HideInInspector] public float damage;
    [HideInInspector] public ObjectPooling mP;      //meteorPool

    bool canExp;                //sadece 1 kez patlamasý için

    private void OnEnable()
    {
        canExp = true;
        smoke.Play(false);
        GetComponent<Rigidbody2D>().gravityScale = 1;
        GetComponent<SpriteRenderer>().enabled = true;

        //Invoke(nameof(SendPool),3f);                  //bu aktif olunca gereksiz yere meteorlarý aktif halde iken takrar ceðýrýyor
        explode.Stop();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canExp)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<KamHealth>().GetDamage(damage, 6);        //6 = BOSS's meteor
            }

            if (!other.CompareTag("Meteor"))
            {
                Explode();
            }
        }
    }

    void Explode()
    {
        canExp = false;
        smoke.Stop(false);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<SpriteRenderer>().enabled = false;

        explode.Play();

        Invoke(nameof(SendPool), 1.2f);
    }

    void SendPool()
    {
        mP.HavuzaObjeEkle(gameObject);
    }
}