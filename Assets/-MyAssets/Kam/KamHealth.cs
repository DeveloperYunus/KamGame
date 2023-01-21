using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KamHealth : MonoBehaviour
{
    [Header("Health")]
    public float health;
    public float armour;

    Animator anim;
    KamController kc;
    Vector2 enmySwordPos;
    float pushStrong;
    float percentArmour;

    public Slider sl;
    public TextMeshProUGUI hpTxt;

    void Start()
    {
        anim = GetComponent<Animator>();
        kc = GetComponent<KamController>();
        percentArmour = armour * 0.01f;

        sl.maxValue = health;
        sl.value = health;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print(other.name);
        if (other.CompareTag("EnmySword"))
        {
            if (other.name == "Blade")
            {
                enmySwordPos = other.GetComponent<Transform>().position;
                pushStrong = other.GetComponent<EnemySword>().pushStrong;
            }
            GetDamage(other.GetComponent<EnemySword>().damage, other.GetComponent<EnemySword>().dmgKind);
        }

        if (other.CompareTag("EnmyBullet"))
        {
            other.GetComponent<Collider2D>().enabled = false;
            GetDamage(other.GetComponent<FireBall>().damage, other.GetComponent<FireBall>().dmgKind);
        }
    }

    void GetDamage(float damage, int dmgKind)
    {
        if (dmgKind == 2)   //hasar türü 2 ise hit nimasyonunu çalýþtýr
        {
            anim.SetTrigger("hit");
            StartCoroutine(kc.SetSlow(0.2f, 0.8f));
        }
        else if (dmgKind == 3)   //hasar türü 3 ise hit nimasyonunu çalýþtýr ve karakteri it
        {
            anim.SetTrigger("hit");
            StartCoroutine(kc.SetSlow(0.2f, 0.8f));

            int a;
            if (enmySwordPos.x > transform.position.x) a = -1;
            else a = 1;
            StartCoroutine(kc.StopWalkAndPush(0.5f, new Vector2(a, 0), pushStrong));
        }
        else if (dmgKind == 4)   //hasar türü 4 ise hit nimasyonunu çalýþtýr ve karakteri it (mage boss)
        {
            anim.SetTrigger("hit");
            StartCoroutine(kc.SetSlow(0.4f, 1f));

            int a;
            if (enmySwordPos.x > transform.position.x) a = -1;
            else a = 1;
            StartCoroutine(kc.StopWalkAndPush(0.7f, new Vector2(a, 0.8f), pushStrong));
        }

        float dmg = damage - damage * percentArmour;               //hesaplama bir kez yapýlsýn diye bir deðiþkene atandý
        health -= dmg;
        ShowDmgTxt(dmg);

        if (health <= 0)
        {
            health = 0;
            Die();
        }
        sl.value = health;
        hpTxt.text = health.ToString("0.##");
    }
    void ShowDmgTxt(float dmg) //yüzen sayýlar ile haasarý gösterir
    {
        float a = transform.position.x;
        float b = transform.position.y;
        FloatingHP.ShowsUp(new Vector2(Random.Range(a + 0.2f, a - 0.2f), Random.Range(b + 0.2f, b - 0.2f)), dmg);
    }

    void Die()
    {
        Destroy(gameObject, 0.1f);
    }
}
