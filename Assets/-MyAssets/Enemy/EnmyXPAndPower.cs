using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnmyXPAndPower : MonoBehaviour
{
    [Header("XP Calculate")]
    public int mustGainXP;
    float ranged, melee;

    /*
    [Header("Konsrol")]
    public float allEnemy;
    public float XPperOneEnemy;

    public float totalMleeXP, totalRangedXP, totalXP;
    */

    [Header("Enemy Stats")]
    public float meleeHP;
    public float meleeArmour, meleeDmg;
    [Space(10)]
    public float rangedHP;
    public float rangedArmour, rangedDmg;

    int kamLevel;

    void Awake()
    {
        ranged = 0;
        melee = 0;
        kamLevel = PlayerPrefs.GetInt("level");

        int a = transform.childCount;
        for (int i = 0; i < a; i++)                                             //ranged ve melee enemy sayýlarýný bul
        {
            if (transform.GetChild(i).GetComponent<RangedAI>()) ranged++;  
            else if (transform.GetChild(i).GetComponent<MeleeAI>()) melee++;  
        }

        float allEnmy = ranged + melee * 1.2f;                                  //hesaplamalarý yap
        float oneEnmyXP = mustGainXP / allEnmy;

        for (int i = 0; i < a; i++)                                             //ranged ve melee enemyleri takrar bulup xp lerini yaz
        {
            if (transform.GetChild(i).GetComponent<RangedAI>())
            {
                transform.GetChild(i).GetComponent<EnemyHealth>().expValue = Mathf.RoundToInt(oneEnmyXP);
                transform.GetChild(i).GetComponent<EnemyHealth>().health = rangedHP * (1 + kamLevel * 0.15f);
                transform.GetChild(i).GetComponent<EnemyHealth>().armour = rangedArmour * (1 + kamLevel * 0.15f);
                transform.GetChild(i).GetComponent<RangedAI>().atkDamage = rangedDmg * (1 + kamLevel * 0.15f);
            }
            else if (transform.GetChild(i).GetComponent<MeleeAI>())
            {
                transform.GetChild(i).GetComponent<EnemyHealth>().expValue = Mathf.RoundToInt(oneEnmyXP * 1.2f);
                transform.GetChild(i).GetComponent<EnemyHealth>().health = meleeHP * (1 + kamLevel * 0.15f);
                transform.GetChild(i).GetComponent<EnemyHealth>().armour = meleeArmour * (1 + kamLevel * 0.15f);
                transform.GetChild(i).GetComponent<MeleeAI>().atkDamage = meleeDmg * (1 + kamLevel * 0.15f);
            }
        }
    }

    







    /*  XP kontrol kýsmý
    void Asd()
    {
        float allEnmy = ranged * 1.2f + melee;
        float oneEnmyXP = mustGainXP / allEnmy;

        allEnemy = ranged * 1.2f + melee;
        XPperOneEnemy = mustGainXP / allEnemy;

        totalMleeXP = XPperOneEnemy * melee;
        totalRangedXP = XPperOneEnemy * ranged * 1.2f;



        totalXP = totalRangedXP + totalMleeXP;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)) 
        {
            Asd();
        }
    }*/
}
