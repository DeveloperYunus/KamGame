using DG.Tweening;
using System.Collections;
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
    float hpRegenTime;

    int exp;
    public static KamHealth instance;

    [Header("UI")]
    [Tooltip("Level ge�i� yada yenido�mak i�in")]public GameObject transitionPnl;
    [Tooltip("4 sn hi� bir�ey olmazsa deaktif olsun")] public GameObject hpUI;
    public Image screenBlood;
    
    public static bool dead;
    int dieTime; 
    float uiFadeTime, thirtyPrcnOfHp;

    void Start()
    {
        instance = this;
        dead = false;
        hpRegenTime = 0;
        dieTime = 0;

        anim = GetComponent<Animator>();
        kc = GetComponent<KamController>();
        exp = PlayerPrefs.GetInt("expValue");
        hpUI.GetComponent<CanvasGroup>().DOFade(0, 0);

        health = 100 + 15 * PlayerPrefs.GetInt("level", 1);
        armour = 10 + 2 * PlayerPrefs.GetInt("level", 1);
        thirtyPrcnOfHp = hpSl.maxValue * 0.3f;

        percentArmour = armour * 0.01f;
        hpTxt.text = health.ToString();
        hpSl.maxValue = health;
        hpSl.value = health;
    }
    private void Update()
    {
        if (Time.time > hpRegenTime && health < hpSl.maxValue)      //can yanilenmesi
        {
            hpRegenTime = Time.time + 1f;
            health += 1;

            if (health > hpSl.maxValue) 
                health = hpSl.maxValue;

            hpSl.value = health;
            hpTxt.text = health.ToString("0.##");

            if (health <= thirtyPrcnOfHp)                                  //ekrandaki screenBlood �n transparanl���n� art�r�r yada azalt�r
            {
                screenBlood.DOKill();
                screenBlood.DOFade((thirtyPrcnOfHp - health) / thirtyPrcnOfHp, 0.2f);
            }
        }

        if (uiFadeTime > 0)     //zamanla u� k�sm� g�zden kaybolur
        {
            uiFadeTime -= Time.deltaTime;
            if (uiFadeTime <= 0)
            {
                hpUI.GetComponent<CanvasGroup>().DOKill();
                hpUI.GetComponent<CanvasGroup>().DOFade(0, 2f);
            }
        }

        /*
        if (hpBelow)        //can %20'nin alt�ndam�
        {
            timer += Time.deltaTime;            //can her ilk defa %20 'nin alt�na indi�inde "timer" '� 1'e e�itle

            if (timer%2==1)
                float a = Mathf.Lerp();    //�nce iceri girsin
            else
                float a = Mathf.Lerp();    //sonra d��ar� ��ks�n
        }
        */


        if (Input.GetKeyUp(KeyCode.R))
        {
            PlayerPrefs.SetInt("expValue",0);                //xp de�erimi s�f�rlar art�r
            PlayerPrefs.SetInt("skillPoint", 0);             //yetenek puan�m� 1 art�r
            PlayerPrefs.SetInt("level", 0);
        }
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

            if (other.GetComponent<EnemySword>())
                GetDamage(other.GetComponent<EnemySword>().damage, other.GetComponent<EnemySword>().dmgKind);
        }

        if (other.CompareTag("EnmyBullet"))
        {
            other.GetComponent<Collider2D>().enabled = false;                                                       //merminin collider�n�n kapat�r
            GetDamage(other.GetComponent<FireBall>().damage, other.GetComponent<FireBall>().dmgKind);
        }
    }

    public void GetDamage(float damage, int dmgKind)
    {
        if (!GetComponent<KamAttack>().isBarrierActv)   //1 = melee & ranged firebal, 
        {
            if (dmgKind == 1)
            {
                CinemachineShake.instance.ShakeCamera(1f, 1f, 0.3f);
            }
            else if (dmgKind == 2)   //hasar t�r� 2 ise hit nimasyonunu �al��t�r
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.2f, 0.8f));

                CinemachineShake.instance.ShakeCamera(1, 1.5f, 0.4f);
            }
            else if (dmgKind == 3)   //hasar t�r� 3 ise hit nimasyonunu �al��t�r ve karakteri it
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.2f, 0.8f));

                CinemachineShake.instance.ShakeCamera(1.4f, 1.5f, 0.7f);

                int a;
                if (enmySwordPos.x > transform.position.x) a = -1;
                else a = 1;
                StartCoroutine(kc.StopWalkAndPush(0.5f, new Vector2(a, 0), pushStrong));
            }
            else if (dmgKind == 4)   //hasar t�r� 4 ise hit nimasyonunu �al��t�r ve karakteri it (mage boss)
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.4f, 1f));

                CinemachineShake.instance.ShakeCamera(1.4f, 1.5f, 1.1f);

                int a;
                if (enmySwordPos.x > transform.position.x) a = -1;
                else a = 1;
                StartCoroutine(kc.StopWalkAndPush(0.7f, new Vector2(a, 0.8f), pushStrong));
            }
            else if (dmgKind == 5)
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.2f, 1.3f));

                CinemachineShake.instance.ShakeCamera(2.4f, 1.5f, 1.2f);
            }
            else if (dmgKind == 6)                  //kaz�k ve d��en ta�lar tuzaklar�
            {
                CinemachineShake.instance.ShakeCamera(2.5f, 1f, 0.7f);
            }

            float dmg = damage - damage * percentArmour;               //hesaplama bir kez yap�ls�n diye bir de�i�kene atand�
            health -= dmg;
            ShowFloatTxt(-dmg, 1);

            if (health <= 0)
            {
                health = 0;
                StartCoroutine(Die());
            }
        }
        else if (PlayerPrefs.GetInt("barrier") == 5)
        {
            if (dmgKind == 5 || dmgKind == 6) return;                  //BOSS 'un hasarlar� can olarak eklenmesin 

            float dmg = damage - damage * percentArmour;               //hesaplama bir kez yap�ls�n diye bir de�i�kene atand�
            health += dmg;
            ShowFloatTxt(dmg ,2);

            if (health > hpSl.maxValue)
            {
                health = hpSl.maxValue;
            }
        }

        FadeUpHPUI();        
        hpSl.value = health;
        hpTxt.text = health.ToString("0.##");

        if (health <= thirtyPrcnOfHp)                                  //ekrandaki screenBlood �n transparanl���n� art�r�r yada azalt�r
        {
            screenBlood.DOKill();
            screenBlood.DOFade((thirtyPrcnOfHp - health) / thirtyPrcnOfHp, 0.2f);
        }
    }
    void ShowFloatTxt(float dmg, int type) //y�zen say�lar ile hasar� g�sterir ve xp'yi g�sterir
    {
        float a = transform.position.x;
        float b = transform.position.y;
        FloatingHP.ShowsUp(new Vector2(Random.Range(a + 0.3f, a - 0.3f), Random.Range(b + 0.2f, b - 0.2f)), dmg, type);
    }
    public void GainExp(int expValue)
    {
        if (PlayerPrefs.GetInt("level", 1) < 20)    //Kam maximum 20 sv olabilir
        {
            exp += expValue;
            PlayerPrefs.SetInt("expValue", exp);

            if (exp >= 100)
            {

                exp -= 100;

                PlayerPrefs.SetInt("expValue", exp);                                        //xp de�erimi s�f�rlar art�r
                PlayerPrefs.SetInt("skillPoint", PlayerPrefs.GetInt("skillPoint") + 1);     //yetenek puan�m� 1 art�r
                PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1) + 1);            //seviyemi 1 art�r

                health = 100 + 15 * PlayerPrefs.GetInt("level");
                armour = 10 + 2 * PlayerPrefs.GetInt("level");
            }

            ShowFloatTxt(expValue, 3);
        }
    }


    public void FadeUpHPUI()
    {
        hpUI.GetComponent<CanvasGroup>().DOKill();
        hpUI.GetComponent<CanvasGroup>().DOFade(1, 0.7f);
        uiFadeTime = 7;
    }
    IEnumerator Die()
    {
        Time.timeScale = 0.5f;
        dead = true;
        dieTime++;

        yield return new WaitForSeconds(0.5f);
        transitionPnl.GetComponent<RectTransform>().DOScale(1, 0);
        transitionPnl.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

        yield return new WaitForSeconds(0.6f);
        GetComponent<Transform>().position = CheckPntSys.checkPnt;
        GetComponent<KamAttack>().RebornBarrier(3);                         //�ld�kten sonra do�unca 3 sn hasar almas�n

        health = hpSl.maxValue;
        hpSl.value = health;
        hpTxt.text = health.ToString("0.##");
        screenBlood.DOFade(0, 0f);

        Time.timeScale = 1;
        yield return new WaitForSeconds(1f);
        transitionPnl.GetComponent<RectTransform>().DOScale(0, 0).SetDelay(0.5f);
        transitionPnl.GetComponent<CanvasGroup>().DOFade(0, 0.2f).SetDelay(0.3f);
    }
}
