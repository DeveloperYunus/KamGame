using Cinemachine;
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
    bool canMeleeDmg;                                           //enemySword lar pe�pese hasar vermesin diye (ayn� zamanda bazen bir k�l�� 2 kez hasar veriyordu)

    int exp;
    public static KamHealth instance;

    [Header("UI")]
    [Tooltip("Level ge�i� yada yenido�mak i�in")]public GameObject transitionPnl;
    [Tooltip("4 sn hi� bir�ey olmazsa deaktif olsun")] public GameObject hpUI;
    public float warSoundVlm;
    public Image screenBlood;
    public CinemachineVirtualCamera cmVC;
    
    public static bool dead;
    //int dieTime;                                              //kam 3 kez �l�nce level tekrar ba�lat�labilir
    float uiFadeTime, thirtyPrcnOfHp;                           //kam genel UI k�sm� zamanla ortadan kaybolsun diye ve bloodyScreen i�in
    float warSoundTime;                                         //Kam sava�tan ��k�nca sava� muzi�i sussun diye

    bool hpBelow;
    float timer, camOrthSize;                                   //zamanlay�c� ve cameran�n ba�lang��taki ortho boyutu

    void Start()
    {
        instance = this;
        dead = false;
        hpBelow = false;
        canMeleeDmg = true;

        timer = 0;
        hpRegenTime = 0;
        camOrthSize = cmVC.m_Lens.OrthographicSize;
        //dieTime = 0;

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
            hpTxt.text = health.ToString("0.#");

            screenBlood.DOKill();
            if (health <= thirtyPrcnOfHp)                                  //ekrandaki screenBlood �n transparanl���n� art�r�r yada azalt�r
                screenBlood.DOFade((thirtyPrcnOfHp - health) / thirtyPrcnOfHp, 0.2f);
            else
                screenBlood.DOFade(0, 0.5f);

            if (health > hpSl.maxValue * 0.2f) hpBelow = false;             //can %20 den b�y�kse camera i�eri d��ar� yapmas�n
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

        if (warSoundTime > 0)     //zamanla warSound s�ns�n 
        {
            warSoundTime -= Time.deltaTime;
            if (warSoundTime <= 0)  
            {
                hpUI.GetComponent<AudioSource>().DOKill();
                hpUI.GetComponent<AudioSource>().DOFade(0f * PlayerPrefs.GetFloat("soundLevel"), 3f).OnComplete(() =>
                {
                    hpUI.GetComponent<AudioSource>().Stop();
                });
            }
        }


        if (hpBelow)        //can %20'nin alt�ndam� (o zaman kalp at��� animasyonunun ba�lat)
        {
            timer += Time.deltaTime;            

            if (timer < 0.45)
            {
                cmVC.m_Lens.OrthographicSize = Mathf.Lerp(cmVC.m_Lens.OrthographicSize, 4.65f, 0.15f);    //�nce iceri girsin
            }
            else
            {
                cmVC.m_Lens.OrthographicSize = Mathf.Lerp(cmVC.m_Lens.OrthographicSize, camOrthSize, 0.15f);    //sonra d��ar� ��ks�n

                if (timer > 0.9f) timer = 0.01f;
            }
        }
        else if(timer > 0)
        {
            cmVC.m_Lens.OrthographicSize = Mathf.Lerp(cmVC.m_Lens.OrthographicSize, camOrthSize, 0.05f);    

            if (cmVC.m_Lens.OrthographicSize > 4.95f)
            {
                cmVC.m_Lens.OrthographicSize = 5;
                timer = 0;
                AudioManager.instance.StopSound("HeartBeat");               //can artt� kalp at�� i�lemi dursun
            }
        }



        if (Input.GetKeyUp(KeyCode.R))      //build al�rken s�f�rlanacak
        {
            PlayerPrefs.SetInt("expValue",0);                //xp de�erimi s�f�rlar art�r
            PlayerPrefs.SetInt("skillPoint", 0);             //yetenek puan�m� 1 art�r
            PlayerPrefs.SetInt("level", 1);
            exp = 0;
            print("level s�f�rla, (bu k�s�m silinecek)");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnmySword") && canMeleeDmg)
        {
            canMeleeDmg = false;
            Invoke(nameof(ResetMeleDmg), 0.25f);

            if (other.name == "Blade")
            {
                enmySwordPos = other.GetComponent<Transform>().position;
                pushStrong = other.GetComponent<EnemySword>().pushStrong;
            }

            if (other.GetComponent<EnemySword>())
            {
                if (other.GetComponent<EnemySword>().dmgKind == 1)  //demekki normal melee enemy
                {
                    if (FirstOOP.FiftyChance())
                        AudioManager.instance.PlaySound("Slash1");
                    else
                        AudioManager.instance.PlaySound("Slash2");
                }

                GetDamage(other.GetComponent<EnemySword>().damage, other.GetComponent<EnemySword>().dmgKind);
            }
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
            else if (dmgKind == 2)   //hasar t�r� 2 ise hit animasyonunu �al��t�r - Mle Boss ilk sald�r�
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.2f, 0.8f));

                AudioManager.instance.PlaySound("MleSlash1");
                CinemachineShake.instance.ShakeCamera(1, 1.5f, 0.4f);
            }
            else if (dmgKind == 3)   //hasar t�r� 3 ise hit nimasyonunu �al��t�r ve karakteri it - Mle Boss 2. sald�r� (stan atan sald�r�)
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.2f, 0.8f));

                AudioManager.instance.PlaySound("MleSlash2");
                CinemachineShake.instance.ShakeCamera(1.4f, 1.5f, 0.7f);

                int a;
                if (enmySwordPos.x > transform.position.x) a = -1;
                else a = 1;
                StartCoroutine(kc.StopWalkAndPush(0.5f, new Vector2(a, 0), pushStrong));
            }
            else if (dmgKind == 4)   //hasar t�r� 4 ise hit nimasyonunu �al��t�r ve karakteri it - Rng Boss
            {
                anim.SetTrigger("hit");
                StartCoroutine(kc.SetSlow(0.4f, 1f));

                AudioManager.instance.PlaySound("RngSlash");
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
        WarSoundFade();

        hpSl.value = health;
        hpTxt.text = health.ToString("0.#");

        if (health <= thirtyPrcnOfHp)                                  //ekrandaki screenBlood �n transparanl���n� art�r�r yada azalt�r
        {
            screenBlood.DOKill();
            screenBlood.DOFade((thirtyPrcnOfHp - health) / thirtyPrcnOfHp, 0.2f);
        }

        if (health < hpSl.maxValue * 0.2f)      //cameran�n kalp at��� i�in
        {
            AudioManager.instance.PlaySoundOne("HeartBeat");
            hpBelow = true;
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

            if (exp >= 100)     //level'� 1 art�r
            {
                AudioManager.instance.PlaySound("LevelUp");
                kc.levelUpPS.Play();
                exp -= 100;

                PlayerPrefs.SetInt("expValue", exp);                                        //xp de�erimi s�f�rlar art�r
                PlayerPrefs.SetInt("skillPoint", PlayerPrefs.GetInt("skillPoint") + 1);     //yetenek puan�m� 1 art�r
                PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1) + 1);            //seviyemi 1 art�r

                health = 100 + 15 * PlayerPrefs.GetInt("level");
                armour = 10 + 2 * PlayerPrefs.GetInt("level");

                percentArmour = armour * 0.01f;                 //can ve z�rh g�ncellendikten sonra UI ve hesaplama k�s�mlar�da g�ncellensin
                hpTxt.text = health.ToString("0.#");
                hpSl.maxValue = health;
                hpSl.value = health;
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
    public void WarSoundFade()
    {
        warSoundTime = 4f;

        if (!hpUI.GetComponent<AudioSource>().isPlaying)
            hpUI.GetComponent<AudioSource>().Play();       //m�zik �alm�yorsa �almaya ba�las�n

        hpUI.GetComponent<AudioSource>().DOKill();
        hpUI.GetComponent<AudioSource>().DOFade(warSoundVlm * PlayerPrefs.GetFloat("soundLevel"), 2f);
    }

    IEnumerator Die()
    {
        AudioManager.instance.PlaySound("KamDie");
        Time.timeScale = 0.5f;
        dead = true;
        //dieTime++;

        yield return new WaitForSeconds(0.5f);
        transitionPnl.GetComponent<RectTransform>().DOScale(1, 0);
        transitionPnl.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

        yield return new WaitForSeconds(0.6f);
        GetComponent<Transform>().position = CheckPntSys.checkPnt;
        GetComponent<KamAttack>().RebornBarrier(3);                         //�ld�kten sonra do�unca 3 sn hasar almas�n

        health = hpSl.maxValue;
        hpBelow = false;
        hpSl.value = health;
        hpTxt.text = health.ToString("0.#");

        Time.timeScale = 1;
        yield return new WaitForSeconds(1f);
        screenBlood.DOFade(0, 0f);
        transitionPnl.GetComponent<RectTransform>().DOScale(0, 0).SetDelay(0.5f);
        transitionPnl.GetComponent<CanvasGroup>().DOFade(0, 0.2f).SetDelay(0.3f);
    }

    void ResetMeleDmg()
    {
        canMeleeDmg = true;
    }
}
