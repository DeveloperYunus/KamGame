using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class KamAttack : MonoBehaviour
{
    KamController kc;
    Animator anim;
    [HideInInspector] public float animTransition;                                                                        //cok fazla animasyon olunca di�erleri bunun bitmesini bekleyecekler

    [Header("Bolt")]
    public GameObject bolt;
    public float boltSpeed;
    public float boltDmg;
    public Transform staffMuzzle;

    Vector3 worldPos;
    [HideInInspector] public bool muzzleInGround;

    [Header("Thunder")]
    public GameObject thunder;
    public float thunderDmg, cooldown1;
    public CircleCollider2D colliderr;
    public Image cldwnThunder;

    [Header("Trap")]
    public GameObject trap;
    public float trapDmg, cooldown2;
    GameObject groundedTrap;
    public Image cldwnTrap;

    [Header("Barrier")]
    public ParticleSystem barrier;
    public float barrierDuration, cooldown3;
    public Image cldwnBarrier;

    [HideInInspector] public bool isBarrierActv;

    Light2D lightt;
    float time, lightMax, lightMin, lightNrml;

    void Start()
    {
        groundedTrap = null;
        isBarrierActv = false;
        muzzleInGround = false;
        animTransition = 0;

        anim = GetComponent<Animator>();
        kc = GetComponent<KamController>();
        lightt = barrier.GetComponent<Light2D>();

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.2f;
        lightMax = lightNrml + 0.2f;
    }
    void Update()
    {
        if (animTransition <= 0 && KamController.canMove)
        {
            if (kc.animSlow != 1)
                kc.animSlow = 1;

            if (Input.GetMouseButton(0))
            {
                worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);                 //masuse positionu screen posisyonu old. i�in bunu yapt�k
                Vector3 difference = worldPos - staffMuzzle.position;
                float distance = difference.magnitude;
                Vector2 direction = difference / distance;
                direction.Normalize();

                if (Mathf.Abs(direction.x) > 0.5f && Mathf.Abs(worldPos.x-transform.position.x) >= 2.3f)
                {
                    if (transform.localScale.x > 0 && direction.x < 0)      kc.FlipFace();
                    else if (transform.localScale.x < 0 && direction.x > 0) kc.FlipFace();                   
                    NormalAttack();
                }
            }
            if (Input.GetKeyDown(KeyCode.E) && kc.isGrounded && cldwnThunder.fillAmount == 0 && PlayerPrefs.GetInt("thunder") > 0)
            {
                colliderr.enabled = true;
                colliderr.GetComponent<ThunderRadius>().collIsActive = true;
                Thunder();
            }
            if (Input.GetKeyDown(KeyCode.Q) && cldwnBarrier.fillAmount == 0 && PlayerPrefs.GetInt("barrier") > 0)
                Barrier();
            if (Input.GetKeyDown(KeyCode.S) && kc.isGrounded && cldwnTrap.fillAmount == 0 && PlayerPrefs.GetInt("trap") > 0)
                Trap();
        }
        else animTransition -= Time.deltaTime;


        if (isBarrierActv && time < Time.time)
        {
            time += 0.15f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.05f);
        }

        if (cldwnThunder.fillAmount > 0)
        {
            cldwnThunder.fillAmount -= 1 / cooldown1 * Time.deltaTime;
        }
        if (cldwnTrap.fillAmount > 0)
        {
            cldwnTrap.fillAmount -= 1 / cooldown2 * Time.deltaTime;
        }
        if (cldwnBarrier.fillAmount > 0)
        {
            cldwnBarrier.fillAmount -= 1 / cooldown3 * Time.deltaTime;
        }
    }

    void NormalAttack()
    {
        anim.SetTrigger("atk");
        animTransition = 0.6f;
        kc.animSlow = 0.3f;

        if (FirstOOP.FiftyChance())
            AudioManager.instance.PlaySound("StaffSwo1");
        else
            AudioManager.instance.PlaySound("StaffSwo2");
        KamHealth.instance.FadeUpHPUI();
    }
    void Thunder()
    {
        anim.SetTrigger("thunder");
        animTransition = 0.5f;
        kc.animSlow = 0.2f;

        cldwnThunder.fillAmount = 1;
        KamHealth.instance.FadeUpHPUI();
    }
    void Barrier()
    {
        anim.SetTrigger("barrier");
        animTransition = 1f;
        kc.animSlow = 0.2f;

        cldwnBarrier.fillAmount = 1;
        KamHealth.instance.FadeUpHPUI();
    }
    void Trap()
    {
        anim.SetTrigger("trap");
        animTransition = 1f;
        kc.animSlow = 0.2f;

        cldwnTrap.fillAmount = 1;
        KamHealth.instance.FadeUpHPUI();       
    }

    void NormalAttackEvent()//animasyonun i�erisindeki event i�in
    {
        GameObject a = Instantiate(bolt, staffMuzzle.position, Quaternion.identity);

        if (!muzzleInGround)
        {
            Vector3 difference = worldPos - staffMuzzle.position;
            float distance = difference.magnitude;
            Vector2 direction = difference / distance;
            direction.Normalize();

            a.GetComponent<Rigidbody2D>().velocity = direction * boltSpeed;
            a.GetComponent<Bolt>().damage = boltDmg * PlayerPrefs.GetInt("bolt", 1);
        }
        else
        {
            a.GetComponent<Bolt>().Explode();
        }
    }
    void NormalThunderEvent()
    {
        /*Vector2 targetPos = colliderr.GetComponent<ThunderRadius>().ClosestEnemy();

        GameObject a = Instantiate(thunder, targetPos, Quaternion.Euler(0, 0, 0));
        a.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder");*/

        List<GameObject> enemies = colliderr.GetComponent<ThunderRadius>().ClosestEnemy();
        AudioManager.instance.PlaySound("Thunder");

        if (enemies.Count != 0)//icerde adam varsa ona hasar ver
        {
            if (PlayerPrefs.GetInt("thunder") != 5)
            {
                GameObject b = Instantiate(thunder, enemies[0].transform.position, Quaternion.Euler(0, 0, 0));
                b.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder");
                colliderr.GetComponent<ThunderRadius>().enemy.Clear();//burada s�f�rl�yoz ki sonraki y�ld�r�mlarda �nceki verilerde bulunan adamlar� baz almas�n
                return;
            }
            int a = enemies.Count;
            for (int i = 0; i < a; i++)
            {

                GameObject b = Instantiate(thunder, enemies[i].transform.position, Quaternion.Euler(0, 0, 0));
                b.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder") * 0.5f;
            }
        }
        else//icerde adam yoksa bakt���n y�ne vur
        {
            GameObject a = Instantiate(thunder, new Vector2(transform.position.x + (2.5f * kc.facingRight), transform.position.y - 1.6f), Quaternion.Euler(0, 0, 0));
            a.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder");
        }
        colliderr.GetComponent<ThunderRadius>().enemy.Clear();//burada s�f�rl�yoz ki sonraki y�ld�r�mlarda �nceki verilerde bulunan adamlar� baz almas�n
    }
    void NormalTrapEvent()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(3 * kc.facingRight, -2), 5, kc.whatIsGround);
        bool ground = Physics2D.Raycast(transform.position, new Vector2(3 * kc.facingRight, -2), 5, kc.whatIsGround);

        if (!ground) //e�er raycast ucunda zemin yoksa electrap spawn etme ve cooldown'u s�f�rla
        {
            cldwnTrap.fillAmount = 0;
            return;
        }
        if (groundedTrap) Destroy(groundedTrap, 0.2f);
        
        GameObject a = Instantiate(trap, hit.point, Quaternion.identity);
        a.GetComponent<ElecTrap>().damage = trapDmg * PlayerPrefs.GetInt("trap");
        groundedTrap = a;
    }
    void NormalBarrier()
    {
        isBarrierActv = true;
        lightt.enabled = true;
        barrier.Play();
        
        AudioManager.instance.PlaySound("ElkBarrier");

        StartCoroutine(BarrierDuration(barrierDuration * PlayerPrefs.GetInt("barrier")));
    }
    public void RebornBarrier(float time)
    {
        isBarrierActv = true;
        lightt.enabled = true;
        barrier.Play();

        AudioManager.instance.PlaySound("ElkBarrier");

        StartCoroutine(BarrierDuration(time));
    }

    public IEnumerator BarrierDuration(float time)
    {
        yield return new WaitForSeconds(time);
        isBarrierActv = false;
        lightt.enabled = false;
        barrier.Stop();

        AudioManager.instance.StopSound("ElkBarrier");      //bariyer kapan�nca ses dekapans�n
    }
}