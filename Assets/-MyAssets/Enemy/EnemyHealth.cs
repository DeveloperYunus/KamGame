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

    void Start()
    {
        percentArmour = armour * 0.01f;

        sl.maxValue = health;
        sl.value = health;
        hpTxt.text = health.ToString();
    }

    public void GetDamage(float damage, int dmgKing)                //bolt = 1, thunder = 2, elecTrap = 3
    {
        float dmg = damage - damage * percentArmour;
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
    void ShowDmgTxt(float dmg)
    {
        float a = transform.position.x;
        float b = transform.position.y;
        FloatingHP.ShowsUp(new Vector2(Random.Range(a + 0.2f, a - 0.2f), Random.Range(b + 0.2f, b - 0.2f)), dmg);
    }

    void Die() 
    {
        Destroy(gameObject, 0.2f);
    }
}