using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class BOSSAI : MonoBehaviour
{
    [Header("Movement")]
    Transform target;
    public float speed;
    public float /*meleeRange,*/ rangedRange;
    public float nextWaypointDistance;                          //gideceðimiz noktaya ne kadar yakýn olunca duralým
    public float seekTime;
    public float seeRange;

    Path path;
    Seeker seeker;
    Rigidbody2D rb;
    Transform enemyBody;
    Vector3 startPos;
    int currentWaypoint = 0;                                    //mevcut gideceðimiz nokta
    float rangedRngS, meleeRngS, seRngSquare, bodyScale;        //menzilli ve yakýncý saldýrý menzillerinin karesi
    float lineerDrag;

    [Header("Stats")]
    public GameObject swordColl;                                //MeleeAI saldýrý için
    public GameObject fireBall;                                 //ranged saldýrý için
    public Transform muzzle;
    public float atkDamage;
    public float atkSpeed;

    EnemyHealth ehp;
    Animator anim;
    bool canAtk;
    bool chaseCase;

    int bossPhase, whichAtk;                                    //whichAtk = normal saldýrýmý yoksa meteor düþmesi mi


    [Header("Falling Meteors")]
    public GameObject meteor;
    public float meteorDamage;
    public float yatayRng;                                      //meteorlarýn yataydaki düþecekleri aralýk
    public int numOfMeteor;                                     //(+-2) tek seferde düþecek meteor sayýsý.
    public float speedX;

    Vector2 originPos;                                          //meteorlarýn düþmeye baþlayacaðý merkez nokta eskiden biz belirliyoduk þimdi belli bir yükseklikte sabit

    void Start()
    {
        speed *= 100;
        bossPhase = 0;
        whichAtk = 0;   
        canAtk = true;
        chaseCase = true;
        rangedRngS = rangedRange * rangedRange;
        //meleeRngS = meleeRange * meleeRange;
        seRngSquare = seeRange * seeRange;
        originPos = transform.position + new Vector3(0, 10, 0);

        target = GameObject.Find("Kam").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        ehp = GetComponent<EnemyHealth>();
        anim = GetComponentInChildren<Animator>();
        swordColl.GetComponent<EnemySword>().damage = atkDamage * 3;
        swordColl.GetComponent<EnemySword>().dmgKind = 5;                       //1= normal saldýrý, 2= stan atn saldýrý,  5 = BOSS 

        startPos = transform.position;
        enemyBody = transform.GetChild(0);
        bodyScale = enemyBody.transform.localScale.x;
        lineerDrag = rb.drag;
        rb.drag = 0;

        InvokeRepeating(nameof(UpdatePath), 0.5f, seekTime);
    }
    void UpdatePath()
    {
        if (bossPhase != 1  && ehp.health <= (ehp.sl.maxValue * 0.4f))
        {
            bossPhase = 1;
            //bir ses gelsin ve faz deðiþikliði olduðunu hissettir
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
                        if (whichAtk < 3)//bu kod ile her 4 saldýrýda 1 meteor düþecek (her 4 * saldýrý hýzý süresinde 1 kez)
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
                        if (whichAtk < 4)
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
                seeker.StartPath(rb.position, target.position, OnPathComplete);        //yolu hesaplýyorsa bir daha hesaplamasýn
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

    void FixedUpdate()
    {
        if (!target) 
            return;

        if (path == null || Vector2.Distance(target.position, rb.position) < nextWaypointDistance)      //yol yoksa yada hedef ile aradaki mesafe küçükse kovalama
            return;

        if (currentWaypoint >= path.vectorPath.Count)        
            return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;       //bizim hedefimizin hangi yönde old. belirler

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);       //kare kök almaya gerek yok
        if (distance < nextWaypointDistance)
            currentWaypoint++;

        if (chaseCase)//kovalýyosam koþ
        {
            float ab = speed * Time.deltaTime;
            rb.AddForce(new Vector2(ab * direction.x, 0));

            if (rb.velocity.x > 0.08f) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
            else if (rb.velocity.x < -0.08f) enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
        }
        anim.SetFloat("speedx", rb.velocity.x);
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
        a.GetComponent<FireBall>().dmgKind = 1;

        anim.SetTrigger("rngAtk");
        canAtk = false;
        Invoke(nameof(AttackReset), atkSpeed);
    }
    void MeleeAttack()
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
        anim.SetTrigger("mleAtk");
        Invoke(nameof(AttackReset), atkSpeed);
        Invoke(nameof(CallTheKam), 0.7f);
    }
    void CallTheKam()
    {
        if (target.position.x - transform.position.x > 0)
        {
            target.position = new Vector2(transform.position.x + 3, transform.position.y - 1);
            StartCoroutine(target.GetComponent<KamController>().SetSlow(0.3f, 1));
        }
        else
        {
            target.position = new Vector2(transform.position.x - 3, transform.position.y - 1);
        }
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

        int a = Random.Range(numOfMeteor - 2, numOfMeteor + 2);             //verilen sayýnýn +-2 aralýðýnda olacak
        for (int i = 0; i < a; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            float X = Random.Range(originPos.x - yatayRng, originPos.x + yatayRng);

            GameObject met = MeteorPool.CallMeteor();
            met.GetComponent<Transform>().position = new Vector2(X, originPos.y);
            met.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-speedX, speedX), 0);
            met.GetComponent<Meteor>().damage = meteorDamage;
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
    /*bool TrgtInMleAtkDist()
    {
        return Vector2.SqrMagnitude(enemyBody.position - target.position) < meleeRngS;
    }*/
    bool TargetInSeeDist()
    {
        return Vector2.SqrMagnitude(enemyBody.position - target.position) < seRngSquare;
    }
    bool CanISeeTarget()
    {
        RaycastHit2D checkWall = Physics2D.Linecast(transform.position, target.position + new Vector3(0, 0.3f, 0));
        return target.name == checkWall.rigidbody.name;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seeRange);

        Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, target.position + new Vector3(0, 0.3f, 0));

        Gizmos.color = Color.yellow;        //meteorlar için
        Gizmos.DrawLine(new(transform.position.x - yatayRng, transform.position.y + 10), new(transform.position.x + yatayRng, transform.position.y + 10));
    }
}