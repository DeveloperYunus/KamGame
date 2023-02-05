using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamAttack : MonoBehaviour
{
    KamController kc;
    Animator anim;
    [HideInInspector] public float animTransition;                                                                        //cok fazla animasyon olunca diðerleri bunun bitmesini bekleyecekler

    [Header("Bolt")]
    public GameObject bolt;
    public float boltSpeed;
    public float boltDmg;
    public Transform staffMuzzle;
    Vector3 worldPos;

    [Header("Thunder")]
    public GameObject thunder;
    public float thunderDmg;
    public CircleCollider2D colliderr;

    [Header("Trap")]
    public GameObject trap;
    public float trapDmg;
    GameObject groundedTrap;

    [Header("Barrier")]
    public ParticleSystem barrier;
    public float barrierDuration;
    public bool isBarrierActv;

    void Start()
    {
        groundedTrap = null;
        anim = GetComponent<Animator>();
        kc = GetComponent<KamController>();
        animTransition = 0;
        isBarrierActv = false;
    }
    void Update()
    {
        if (animTransition <= 0)
        {
            if (kc.animSlow != 1)
                kc.animSlow = 1;

            if (Input.GetMouseButton(0))
            {
                worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);                 //masuse positionu screen posisyonu old. için bunu yaptýk
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
            if (Input.GetKeyDown(KeyCode.E) && kc.isGrounded)
            {
                colliderr.enabled = true;
                colliderr.GetComponent<ThunderRadius>().collIsActive = true;
                Thunder();
            }
            if (Input.GetKeyDown(KeyCode.Q))
                Barrier();
            if (Input.GetKeyDown(KeyCode.S) && kc.isGrounded)
                Trap();
        }
        else animTransition -= Time.deltaTime;
    }

    void NormalAttack()
    {
        anim.SetTrigger("atk");
        animTransition = 0.75f;
        kc.animSlow = 0.3f;
    }
    void Thunder()
    {
        anim.SetTrigger("thunder");
        animTransition = 0.5f;
        kc.animSlow = 0.2f;
    }
    void Barrier()
    {
        anim.SetTrigger("barrier");
        animTransition = 1f;
        kc.animSlow = 0.2f;
    }
    void Trap()
    {
        anim.SetTrigger("trap");
        animTransition = 1f;
        kc.animSlow = 0.2f;
    }

    void NormalAttackEvent()//animasyonun içerisindeki event için
    {
        Vector3 difference = worldPos - staffMuzzle.position;                                                   
        float distance = difference.magnitude;
        Vector2 direction = difference / distance;
        direction.Normalize();

        GameObject a = Instantiate(bolt, staffMuzzle.position, Quaternion.identity);
        a.GetComponent<Rigidbody2D>().velocity = direction * boltSpeed;
        a.GetComponent<Bolt>().damage = boltDmg * PlayerPrefs.GetInt("bolt");
    }
    void NormalThunderEvent()
    {
        /*Vector2 targetPos = colliderr.GetComponent<ThunderRadius>().ClosestEnemy();

        GameObject a = Instantiate(thunder, targetPos, Quaternion.Euler(0, 0, 0));
        a.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder");*/

        List<GameObject> enemies = colliderr.GetComponent<ThunderRadius>().ClosestEnemy();

        if (enemies.Count != 0)//icerde adam varsa ona hasar ver
        {
            if (PlayerPrefs.GetInt("thunder") != 5)
            {

                GameObject b = Instantiate(thunder, enemies[0].transform.position, Quaternion.Euler(0, 0, 0));
                b.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder");
                colliderr.GetComponent<ThunderRadius>().enemy.Clear();//burada sýfýrlýyoz ki sonraki yýldýrýmlarda önceki verilerde bulunan adamlarý baz almasýn
                return;
            }
            int a = enemies.Count;
            for (int i = 0; i < a; i++)
            {

                GameObject b = Instantiate(thunder, enemies[i].transform.position, Quaternion.Euler(0, 0, 0));
                b.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder");
            }
        }
        else//icerde adam yoksa baktýðýn yöne vur
        {
            GameObject a = Instantiate(thunder, new Vector2(transform.position.x + (2.5f * kc.facingRight), transform.position.y - 1.6f), Quaternion.Euler(0, 0, 0));
            a.GetComponent<Thunder>().damage = thunderDmg * PlayerPrefs.GetInt("thunder");
        }
        colliderr.GetComponent<ThunderRadius>().enemy.Clear();//burada sýfýrlýyoz ki sonraki yýldýrýmlarda önceki verilerde bulunan adamlarý baz almasýn
    }
    void NormalTrapEvent()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(3 * kc.facingRight, -2), 5, kc.whatIsGround);
        bool ground = Physics2D.Raycast(transform.position, new Vector2(3 * kc.facingRight, -2), 5, kc.whatIsGround);

        if (!ground) return;
        if (groundedTrap) Destroy(groundedTrap, 0.2f);
        
        GameObject a = Instantiate(trap, hit.point, Quaternion.identity);
        a.GetComponent<ElecTrap>().damage = trapDmg * PlayerPrefs.GetInt("trap");
        groundedTrap = a;
    }
    void NormalBarrier()
    {
        isBarrierActv = true;
        barrier.Play();
        StartCoroutine(BarrierDuration());
    }

    IEnumerator BarrierDuration()
    {
        yield return new WaitForSeconds(barrierDuration * PlayerPrefs.GetInt("barrier"));
        isBarrierActv = false;
        barrier.Stop();
    }
}