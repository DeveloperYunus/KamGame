using System.Collections;
using UnityEngine;
using Pathfinding;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Seeker))]
public class BOSSAI : MonoBehaviour
{
    [Header("Movement")]
    Transform target;
    public float speed;
    public float /*meleeRange,*/ rangedRange;
    public float nextWaypointDistance;                          //gidece�imiz noktaya ne kadar yak�n olunca dural�m
    public float seekTime;
    public float seeRange;

    Path path;
    Seeker seeker;
    Rigidbody2D rb;
    Transform enemyBody;
    Vector3 startPos;
    int currentWaypoint = 0;                                    //mevcut gidece�imiz nokta
    float rangedRngS, meleeRngS, seRngSquare, bodyScale;        //menzilli ve yak�nc� sald�r� menzillerinin karesi
    float lineerDrag;
    int mask;                                                  //linecast'teki layer ignore i�in 


    [Header("Stats")]
    public GameObject swordColl;                                //MeleeAI sald�r� i�in
    public GameObject fireBall;                                 //ranged sald�r� i�in
    public Transform muzzle;
    [Tooltip("FireBall = 1x damage, claw = 3x damage")]public float atkDamage;
    public float atkSpeed;

    EnemyHealth eHp;
    Animator anim;
    bool canAtk;
    bool chaseCase;

    int bossPhase, whichAtk;                                    //whichAtk = normal sald�r�m� yoksa meteor d��mesi mi
    float hpRegenTime;                                          //Bu k�s�m BOSS un can�n�n yenilenmesi i�in

    [Header("Falling Meteors")]
    public GameObject meteor;
    public Vector2 originPos;                                   //meteorlar�n d��meye ba�layaca�� merkez nokta eskiden biz belirliyoduk �imdi belli bir y�kseklikte sabit
    public float meteorDamage;
    public float horizontalRng;                                 //meteorlar�n yataydaki d��ecekleri aral�k
    public int numOfMeteor;                                     //(+-2) tek seferde d��ecek meteor say�s�.
    public float speedX;


    [Header("Cinemachine")]
    public CinemachineVirtualCamera cmCam;                      //cinamachine camera
    public float transSpeed;                                    //Kamera ortho size ve zaman degerlerinin ne kadar h�zl� de�i�ece�ini belirler

    int mleAtkPhase;                                            //BOSS yak�n sald�r�ya ba�lay�nca zaman yava�las�n ve kamera yaklas��n
    float dfltOrthSize;                                         //dfltOrthSize = cameralar�n defaultOrthoSize'�
    bool mleAtkFix;                                             //Melee sald�r� bittikden sonra zaman� ve camera ayarlar�n� sadece bir kez s�f�rlas�n diye (start'da true ba�lar)


    float time, lightMax, lightMin, lightNrml;                  //���k yan�p s�nmesi i�in
    Light2D lightt;

    void Start()
    {
        mask = (1 << 8) | (1 << 7) | (1 << 2) | (1 << 1);     //enemy layer �n� kaydeder    (enemy, transparanFX, dontClose, ignore raycast)
        mask = ~mask;                                         // "~" ifadesi ile tersini al�r (bu olmasa linecast sadece "8" nolu katman� arar. Bu ifade ("~") varken sadece "8" nolu katman� yoksayar)

        lightt = GetComponent<Light2D>();       //yan�p s�nen ���k ayarlar�

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.45f;
        lightMax = lightNrml + 0.45f;

        speed *= 100;
        bossPhase = 0;
        whichAtk = 0;
        mleAtkPhase = 0;
        dfltOrthSize = cmCam.m_Lens.OrthographicSize;
        canAtk = true;
        chaseCase = true;
        mleAtkFix = true;
        rangedRngS = rangedRange * rangedRange;
        //meleeRngS = meleeRange * meleeRange;
        seRngSquare = seeRange * seeRange;

        target = GameObject.Find("Kam").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        eHp = GetComponent<EnemyHealth>();
        anim = GetComponentInChildren<Animator>();
        swordColl.GetComponent<EnemySword>().damage = atkDamage * 3;
        swordColl.GetComponent<EnemySword>().dmgKind = 5;                       //1= normal sald�r�, 2= stan atn sald�r�,  5 = BOSS 
        eHp = GetComponent<EnemyHealth>();

        startPos = transform.position;
        enemyBody = transform.GetChild(0);
        bodyScale = enemyBody.transform.localScale.x;
        lineerDrag = rb.drag;
        rb.drag = 0;

        InvokeRepeating(nameof(UpdatePath), 0.5f, seekTime);
    }
    void FixedUpdate()
    {
        if (Time.time > hpRegenTime && eHp.health < eHp.sl.maxValue)        //can yenilenmesi kodu
        {
            hpRegenTime = Time.time + 1f;
            eHp.health += 1;

            if (eHp.health > eHp.sl.maxValue)
                eHp.health = eHp.sl.maxValue;

            eHp.sl.value = eHp.health;
            eHp.hpTxt.text = eHp.health.ToString("0.#");
        }

        if (time < Time.time)   //���k parlamas� i�in
        {
            time += 0.15f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.22f);
        }

        if (mleAtkPhase == 1)           //sald�r� ba�lad� zaman yava�las�n
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.6f, transSpeed);
            cmCam.m_Lens.OrthographicSize = Mathf.Lerp(cmCam.m_Lens.OrthographicSize, dfltOrthSize - 1.3f, transSpeed);
        }
        else if (mleAtkPhase == 2)      //sald�r� bitti zaman normalle�sin
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, transSpeed);
            cmCam.m_Lens.OrthographicSize = Mathf.Lerp(cmCam.m_Lens.OrthographicSize, dfltOrthSize, transSpeed);
        }
        else if (!mleAtkFix)                            //ne 1 nede 2 de�ilse zaman� ve kameralar� normal hallerine getir
        {
            mleAtkFix = true;
            Time.timeScale = 1;

            cmCam.m_Lens.OrthographicSize = dfltOrthSize;
        }

        if (!target)        //bu sat�rdan sonraki kodlar yol bulma k�sm�n� ilgilendirdiriyor. Yani yol bulma i�lemleri tamamlanmad�ysa alt sat�rlara ge�me
            return;

        if (path == null || Vector2.Distance(target.position, rb.position) < nextWaypointDistance)      //yol yoksa yada hedef ile aradaki mesafe k���kse kovalama
            return;

        if (currentWaypoint >= path.vectorPath.Count)
            return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;       //bizim hedefimizin hangi y�nde old. belirler

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);       //kare k�k almaya gerek yok
        if (distance < nextWaypointDistance)
            currentWaypoint++;

        if (chaseCase)//koval�yosam ko�
        {
            float ab = speed * Time.deltaTime;
            rb.AddForce(new Vector2(ab * direction.x, 0));

            if (rb.velocity.x > 0.08f) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
            else if (rb.velocity.x < -0.08f) enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
        }
        anim.SetFloat("speedx", rb.velocity.x);
    }

    void UpdatePath()
    {
        if (bossPhase != 1  && eHp.health <= (eHp.sl.maxValue * 0.4f))
        {
            bossPhase = 1;
            //bir ses gelsin ve faz de�i�ikli�i oldu�unu hissettir
        }

        if (target)
        {
            if (TargetInRngAtkDist() && CanISeeTarget())
            {
                chaseCase = false;
                rb.drag = lineerDrag;

                if (canAtk)
                {
                    if (bossPhase == 0)
                    {
                        if (whichAtk < 4)   //bu kod ile her 5 sald�r�da 1 meteor d��ecek (her 4 * sald�r� h�z� s�resinde 1 kez)
                        {
                            whichAtk++;
                            RangedAttack();
                        }
                        else
                        {
                            whichAtk = 0;
                            StartCoroutine(Meteor());
                        }
                    }
                    else
                    {
                        if (whichAtk < 3)   //bu kod ile her 4 sald�r�da 1 Kam'� yak�n�na ���nlay�p hasar verecek (her 4 * sald�r� h�z� s�resinde 1 kez)
                        {
                            whichAtk++;
                            StartCoroutine(Meteor());
                        }
                        else
                        {
                            whichAtk = 0;
                            MeleeAttack();
                        }
                    }
                }
            }
            else if (seeker.IsDone() && TargetInSeeDist())
            {
                if (canAtk)
                {
                    whichAtk++;
                    StartCoroutine(Meteor());
                }

                chaseCase = true;
                rb.drag = 1f; 
                seeker.StartPath(rb.position, target.position, OnPathComplete);        //yolu hesapl�yorsa bir daha hesaplamas�n
            }
            else if (seeker.IsDone())
                seeker.StartPath(rb.position, startPos, OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }


    void RangedAttack()
    {
        if (target.position.x - transform.position.x > 0) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
        else enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);

        Vector3 difference = target.position - transform.position;
        float distance = difference.magnitude;
        Vector2 direction = difference / distance;
        direction.Normalize();

        GameObject a = Instantiate(fireBall, muzzle.position, Quaternion.identity);
        a.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y - 0.1f) * 7;                  //merminin hedef yeri
        a.GetComponent<FireBall>().damage = atkDamage;
        a.GetComponent<FireBall>().dmgKind = 6;                         //BOSS's meteor ve fireball

        anim.SetTrigger("rngAtk");
        canAtk = false;
        Invoke(nameof(AttackReset), atkSpeed);
    }
    void MeleeAttack()
    {
        target.GetComponent<KamController>().canMove = false;
        target.GetComponent<KamController>().StopRun();

        if (target.position.x - transform.position.x > 0)
        {
            enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);   //BOSS kama baks�n           
            if (target.localScale.x > 0) target.GetComponent<KamController>().FlipFace();
        }
        else
        {
            enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
            if (target.localScale.x < 0) target.GetComponent<KamController>().FlipFace();
        }

        mleAtkPhase = 1;        //zaman yava�las�n
        mleAtkFix = false;

        canAtk = false;
        anim.SetTrigger("mleAtk");
        Invoke(nameof(AttackReset), atkSpeed);
        Invoke(nameof(CallKam), 0.7f);
        Invoke(nameof(ReleaseKam), 1.5f);
    }
    void CallKam()
    {
        if (target.position.x - transform.position.x > 0)        
            target.DOMove(new Vector2(transform.position.x + 3.2f, transform.position.y - 0.5f), 0.8f).SetEase(Ease.OutBack);     //KAM h�zla BOSS un �n�ne gelsin        
        else        
            target.DOMove(new Vector2(transform.position.x - 3.2f, transform.position.y - 0.5f), 0.8f).SetEase(Ease.OutBack);        
    }
    void ReleaseKam()
    {
        mleAtkPhase = 2;    //zaman normale d�ns�n
        target.GetComponent<KamController>().canMove = true;        
    }
    public IEnumerator Meteor()
    {
        if (target.position.x - transform.position.x > 0)
        {
            enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
        }
        else
        {
            enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
        }

        canAtk = false;
        anim.SetTrigger("fallMeteor");
        Invoke(nameof(AttackReset), atkSpeed * 1.5f);

        int a = Random.Range(numOfMeteor - 2, numOfMeteor + 2);             //verilen say�n�n +-2 aral���nda olacak
        for (int i = 0; i < a; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            float X = Random.Range(originPos.x - horizontalRng, originPos.x + horizontalRng);

            GameObject met = MeteorPool.CallMeteor();
            met.GetComponent<Transform>().position = new Vector2(X, originPos.y);
            met.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-speedX, speedX), 0);
            met.GetComponent<Meteor>().damage = meteorDamage;
            met.GetComponent<CircleCollider2D>().enabled = true;
        }
    }
    void AttackReset()
    {
        canAtk = true;
    }

    bool TargetInRngAtkDist()
    {
        return Vector2.SqrMagnitude(transform.position - target.position) < rangedRngS;        
    }
    /*bool TrgtInMleAtkDist()           //bunun i�in gerek yok ama ilerde eklenebilir diye dursun
    {
        return Vector2.SqrMagnitude(enemyBody.position - target.position) < meleeRngS;
    }*/
    bool TargetInSeeDist()
    {
        return Vector2.SqrMagnitude(enemyBody.position - target.position) < seRngSquare;
    }
    bool CanISeeTarget()
    {
        RaycastHit2D checkWall = Physics2D.Linecast(transform.position, target.position + new Vector3(0, 0.3f, 0), mask);
        return target.name == checkWall.transform.name;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seeRange);

        Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, target.position + new Vector3(0, 0.3f, 0));

        Gizmos.color = Color.yellow;        //meteorlar i�in
        Gizmos.DrawLine(new(originPos.x - horizontalRng, originPos.y), new(originPos.x + horizontalRng, originPos.y));
    }
}