using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [Tooltip("Enemy ölünce kazanacaðýmýz tecrübe puaný")]public float expValue;

    void Start()
    {
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
        sl.value = health;
        hpTxt.text = health.ToString("0.##");
    }
    void ShowDmgTxt(float dmg)
    {
        float a = transform.position.x;
        float b = transform.position.y;
        FloatingHP.ShowsUp(new Vector2(Random.Range(a + 0.2f, a - 0.2f), Random.Range(b + 0.2f, b - 0.2f)), dmg);
    }

    void Die() 
    {
        KamHealth.instance.ShowExp(expValue);

        Destroy(gameObject, 0.2f);
    }

    IEnumerator ResetDrag(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<Rigidbody2D>().drag = defaultDrag;
    }
}