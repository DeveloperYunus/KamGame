using DG.Tweening;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KamHealth : MonoBehaviour
{
    [Header("Health")]
    public float health;
    public float armour;
    public Slider hpSl;
    public TextMeshProUGUI hpTxt;

    Animator anim;
    KamController kc;
    Vector2 enmySwordPos;
    float pushStrong;
    float percentArmour;

    [Header("Experince")]
    public Slider expSl;
    public TextMeshProUGUI expTxt, skillPTxt, addedExp;

    float exp;
    public static KamHealth instance;

    void Start()
    {
        instance = this;

        anim = GetComponent<Animator>();
        kc = GetComponent<KamController>();
        percentArmour = armour * 0.01f;
        exp = PlayerPrefs.GetFloat("exp");

        hpSl.maxValue = health;
        hpSl.value = health;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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

    public void GetDamage(float damage, int dmgKind)
    {
        if (!GetComponent<KamAttack>().isBarrierActv)
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
            else if (dmgKind == 5)
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.1f, 1.5f));
            }

            float dmg = damage - damage * percentArmour;               //hesaplama bir kez yapýlsýn diye bir deðiþkene atandý
            health -= dmg;
            ShowDmgTxt(-dmg);

            if (health <= 0)
            {
                health = 0;
                Die();
            }
        }
        else if (PlayerPrefs.GetInt("barrier") ==5)
        {
            float dmg = damage - damage * percentArmour;               //hesaplama bir kez yapýlsýn diye bir deðiþkene atandý
            health += dmg;
            ShowDmgTxt(dmg);

            if (health > hpSl.maxValue)
            {
                health = hpSl.maxValue;
            }
        }
        hpSl.value = health;
        hpTxt.text = health.ToString("0.##");
    }
    void ShowDmgTxt(float dmg) //yüzen sayýlar ile haasarý gösterir
    {
        float a = transform.position.x;
        float b = transform.position.y;
        FloatingHP.ShowsUp(new Vector2(Random.Range(a + 0.3f, a - 0.3f), Random.Range(b + 0.2f, b - 0.2f)), dmg);
    }
    public void ShowExp(float expValue)
    {
        exp += expValue;

        if (exp > 100)
        {
            exp -= 100;
            PlayerPrefs.SetInt("skillPoint", PlayerPrefs.GetInt("skillPoint"));
        }

        expTxt.text = exp.ToString();
        skillPTxt.text = PlayerPrefs.GetInt("skillPoint").ToString();
        addedExp.text = "+ " + expValue.ToString();
        PlayerPrefs.SetFloat("expValue", exp);
        
        addedExp.GetComponent<RectTransform>().DOKill(false);
        addedExp.GetComponent<RectTransform>().DOScale(1.2f, 0.4f).SetEase(Ease.OutBack);
        addedExp.GetComponent<RectTransform>().DOScale(1f, 0.6f).SetDelay(0.5f);

        expSl.GetComponent<CanvasGroup>().DOKill(false);
        expSl.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
        expSl.GetComponent<CanvasGroup>().DOFade(0, 2f).SetDelay(4f);
    }

    void Die()
    {
        Destroy(gameObject, 0.1f);
    }
}
