using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class RangedAI : MonoBehaviour
{
    [Header("Movement")]
    Transform target;
    public float speed;
    public float atkRange;
    public float nextWaypointDistance;              //gideceðimiz noktaya ne kadar yakýn olunca duralým ayný zamanda saldýrý mesafesi
    public float seekTime;
    public float seeRange;
    [HideInInspector] public float slow;                              //hasar alýnca yada spike içindeyken yavaþlamasý için

    Path path;
    Seeker seeker;
    Rigidbody2D rb;
    Transform enemyBody;
    Vector3 startPos;
    int currentWaypoint = 0;                        //mevcut gideceðimiz nokta
    float atkRngSquare, seRngSquare, bodyScale;
    int mask;                                       //linecast'teki layer ignore için 


    [Header("Jump")]
    public float jumpHeight;
    public float jumpForce;
    public Transform groundCheckPos;
    public LayerMask whatIsGround;

    bool isGrounded;
    bool jumpEnabled;


    [Header("Animation/Attack")]
    public GameObject fireBall;
    public Transform muzzle;
    public float atkDamage;
    public float atkSpeed;

    Animator anim;
    bool canAtk;
    bool chaseCase;

    void Start()
    {
        mask = (1 << 8) | (1 << 7) | (1 << 2) | (1 << 1);     //enemy layer ýný kaydeder    (enemy, transparanFX, dontClose, ignore raycast)
        mask = ~mask;                                         // "~" ifadesi ile tersini alýr (bu olmasa linecast sadece "8" nolu katmaný arar. Bu ifade ("~") varken sadece "8" nolu katmaný yoksayar)

        slow = 1;
        speed *= 100;
        jumpForce *= 100;
        jumpEnabled = false;
        canAtk = true;
        chaseCase = true;
        atkRngSquare = atkRange * atkRange;
        seRngSquare = seeRange * seeRange;

        target = GameObject.Find("Kam").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        startPos = transform.position;
        enemyBody = transform.GetChild(0);
        bodyScale = enemyBody.transform.localScale.x;

        Invoke(nameof(JumpReset), 1.5f);
        InvokeRepeating(nameof(UpdatePath), 0f, seekTime);
    }
    void UpdatePath()
    {
        if (target)
        {
            if (TargetInAtkDist() && CanISeeTarget())
            {
                chaseCase = false;
                if (canAtk) Attack();
            }
            else if (seeker.IsDone() && TargetInSeeDist())
            {
                chaseCase = true;
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
        anim.speed = slow;

        if (!target) 
            return;

        if (path == null || Vector2.Distance(target.position, rb.position) < nextWaypointDistance)      //yol yoksa yada hedef ile aradaki mesafe küçükse kovalama
            return;

        if (currentWaypoint >= path.vectorPath.Count)        
            return;

        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, 0.2f, whatIsGround);
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;       //bizim hedefimizin hangi yönde old. belirler

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);       //kare kök almaya gerek yok
        if (distance < nextWaypointDistance)
            currentWaypoint++;

        if (chaseCase)//kovalýyosam koþ ve zýpla
        {
            rb.AddForce(new Vector2(speed * slow * Time.deltaTime * direction.x, 0));

            if (isGrounded && jumpEnabled && direction.y > jumpHeight)
            {
                rb.AddForce(jumpForce * slow * Vector2.up);
                jumpEnabled = false;
                Invoke(nameof(JumpReset), 1.5f);
            }

            if (rb.velocity.x > 0.08f) enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
            else if (rb.velocity.x < -0.08f) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
        }        
    }


    void Attack()
    {
        if (target.position.x - transform.position.x > 0) enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
        else enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);

        Vector3 difference = target.position - transform.position;
        float distance = difference.magnitude;
        Vector2 direction = difference / distance;
        direction.Normalize();

        GameObject a = Instantiate(fireBall, muzzle.position, Quaternion.identity);
        a.GetComponent<Rigidbody2D>().velocity = direction * 7;
        a.GetComponent<FireBall>().damage = atkDamage;
        a.GetComponent<FireBall>().dmgKind = 1;

        canAtk = false;
        anim.SetTrigger("attack");
        Invoke(nameof(AttackReset), atkSpeed);
    }
    void JumpReset()
    {
        jumpEnabled = true;
    }
    void AttackReset()
    {
        canAtk = true;
    }

    bool TargetInAtkDist()
    {
        return Vector2.SqrMagnitude(transform.position - target.position) < atkRngSquare;        
    }
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
    }
}