using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class RngdBossAI : MonoBehaviour
{
    [Header("Movement")]
    Transform target;
    public float speed;
    public float meleeRange, rangedRange;
    public float nextWaypointDistance;              //gideceðimiz noktaya ne kadar yakýn olunca duralým
    public float seekTime;
    public float seeRange;

    Path path;
    Seeker seeker;
    Rigidbody2D rb;
    Transform enemyBody;
    Vector3 startPos;
    int currentWaypoint = 0;                        //mevcut gideceðimiz nokta
    float rangedRngS, meleeRngS, seRngSquare, bodyScale;            //menzilli ve yakýncý saldýrý menzillerinin karesi
    float lineerDrag;

    [Header("Jump")]
    public float jumpHeight;
    public float jumpForce;
    public Transform groundCheckPos;
    public LayerMask whatIsGround;

    [Header("Stats")]
    public GameObject swordColl;                //MeleeAI saldýrý için
    public GameObject fireBall;                 //ranged saldýrý için
    public Transform muzzle;
    public float atkDamage;
    public float atkSpeed, pushStrong;

    Animator anim;
    bool canAtk;
    bool chaseCase;

    void Start()
    {
        speed *= 100;
        jumpForce *= 100;
        canAtk = true;
        chaseCase = true;
        rangedRngS = rangedRange * rangedRange;
        meleeRngS = meleeRange * meleeRange;
        seRngSquare = seeRange * seeRange;

        target = GameObject.Find("Kam").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        swordColl.GetComponent<EnemySword>().damage = atkDamage * 2;
        swordColl.GetComponent<EnemySword>().dmgKind = 4;                       //1= normal saldýrý, 2= stan atn saldýrý
        swordColl.GetComponent<EnemySword>().pushStrong = pushStrong;                       //1= normal saldýrý, 2= stan atn saldýrý
        startPos = transform.position;
        enemyBody = transform.GetChild(0);
        bodyScale = enemyBody.transform.localScale.x;
        lineerDrag = rb.drag;
        rb.drag = 0;

        InvokeRepeating(nameof(UpdatePath), 0f, seekTime);
    }
    void UpdatePath()
    {
        if (target)
        {
            if (TargetInRngAtkDist() && CanISeeTarget())
            {
                chaseCase = false;
                rb.drag = lineerDrag;

                if (canAtk)
                {
                    if (TrgtInMleAtkDist())
                        MeleeAttack();
                    else
                        RangedAttack();
                }              
            }
            else if (seeker.IsDone() && TargetInSeeDist())
            {
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
            rb.AddForce(new Vector2(ab * direction.x, ab * direction.y * 2));

            if (rb.velocity.x > 0.08f) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
            else if (rb.velocity.x < -0.08f) enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
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
        a.GetComponent<FireBall>().dmgKind = 1;

        canAtk = false;
        anim.SetTrigger("rangedAtk");
        Invoke(nameof(AttackReset), atkSpeed);
    }
    void MeleeAttack()
    {
        if (target.position.x - transform.position.x > 0) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
        else enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);

        canAtk = false;
        anim.SetTrigger("meleeAtk");
        Invoke(nameof(AttackReset), atkSpeed);
    }
    void AttackReset()
    {
        canAtk = true;
    }

    bool TargetInRngAtkDist()
    {
        return Vector2.SqrMagnitude(transform.position - target.position) < rangedRngS;        
    }
    bool TrgtInMleAtkDist()
    {
        return Vector2.SqrMagnitude(enemyBody.position - target.position) < meleeRngS;
    }
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
    }
}