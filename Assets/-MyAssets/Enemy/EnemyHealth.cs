using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float health;
    public float armour;    
    public Slider sl;
    public TextMeshProUGUI hpTxt;

    float percentArmour;
    float defaultDrag;

    [Header("Experinece")]
    [Tooltip("Enemy ölünce kazanacaðýmýz tecrübe puaný")]public int expValue;

    void Start()
    {
        sl.GetComponent<CanvasGroup>().DOFade(0, 0);
        defaultDrag = GetComponent<Rigidbody2D>().drag;
        percentArmour = armour * 0.01f;

        sl.maxValue = health;
        sl.value = health;
        hpTxt.text = health.ToString();
    }

    public void GetDamage(float damage, int dmgKing)                //bolt = 1, thunder = 2, elecTrap = 3
    {
        if (dmgKing == 3 && PlayerPrefs.GetInt("trap") == 5)
        {
            GetComponent<Rigidbody2D>().drag = 15;
            StartCoroutine(ResetDrag(3f));
        }

        float dmg = damage - damage * percentArmour;
        health -= dmg;
        ShowDmgTxt(-dmg);

        if (health <= 0)
        {
            health = 0;
            Die();
        }

        sl.GetComponent<CanvasGroup>().DOKill();
        sl.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        sl.GetComponent<CanvasGroup>().DOFade(0, 1.5f).SetDelay(5f);
        sl.value = health;
        hpTxt.text = health.ToString("0.##");
    }
    void ShowDmgTxt(float dmg)
    {
        float a = transform.position.x;
        float b = transform.position.y;
        FloatingHP.ShowsUp(new Vector2(Random.Range(a + 0.2f, a - 0.2f), Random.Range(b + 0.2f, b - 0.2f)), dmg, 1);
    }

    void Die() 
    {
        //Enemylerin içinden +7xp yazan text cýksýn
        KamHealth.instance.GainExp(expValue);

        Destroy(gameObject, 0.1f);
    }

    IEnumerator ResetDrag(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<Rigidbody2D>().drag = defaultDrag;
    }
}