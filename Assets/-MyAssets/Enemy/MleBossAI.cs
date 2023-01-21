using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class MleBossAI : MonoBehaviour
{
    [Header("Movement")]
    Transform target;
    public float speed;
    public float nextWaypointDistance;              //gidece�imiz noktaya ne kadar yak�n olunca dural�m
    public float seekTime;
    public float seeRange;

    Path path;
    Seeker seeker;
    Rigidbody2D rb;
    Transform enemyBody;
    Vector3 startPos;
    int currentWaypoint = 0;                        //mevcut gidece�imiz nokta
    float NWPDSquare, seRngSquare, bodyScale;

    [Header("Jump")]
    public float jumpHeight;
    public float jumpForce;
    public Transform groundCheckPos;
    public LayerMask whatIsGround;

    bool isGrounded;
    bool jumpEnabled;

    [Header("Stats")]                   //animations - attack - defence
    public GameObject swordColl;
    public float atkDamage, atkSpeed, pushStrong, atkSprint;        //atkSprint 2. sald�r�da ne kadar ileri at�lacak
    [HideInInspector]public int atkKind;                        //MeleeBoss i�in, vuru�u stanm� yoksa gari itmemi onu belirler

    Animator anim;
    bool canAtk;

    void Start()
    {
        speed *= 100;
        jumpForce *= 100;
        jumpEnabled = false;
        canAtk = true;
        NWPDSquare = nextWaypointDistance * nextWaypointDistance;
        seRngSquare = seeRange * seeRange;

        target = GameObject.Find("Kam").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        swordColl.GetComponent<EnemySword>().damage = atkDamage;
        swordColl.GetComponent<EnemySword>().dmgKind = 2;                       //1= normal sald�r�, 2= stan atn sald�r�
        swordColl.GetComponent<EnemySword>().pushStrong = pushStrong;                       //1= normal sald�r�, 2= stan atn sald�r�
        startPos = transform.position;
        enemyBody = transform.GetChild(0);
        bodyScale = enemyBody.transform.localScale.x;

        Invoke(nameof(JumpReset), 1.5f);
        InvokeRepeating(nameof(UpdatePath), 0f, seekTime);
    }
    void UpdatePath()
    {
        if (seeker.IsDone() && target && TargetInSeeDist()) 
            seeker.StartPath(rb.position, target.position, OnPathComplete);        //yolu hesapl�yorsa bir daha hesaplamas�n
        else if (seeker.IsDone())
            seeker.StartPath(rb.position, startPos, OnPathComplete);        //yolu hesapl�yorsa bir daha hesaplamas�n
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
        anim.SetFloat("speedx", Mathf.Abs(rb.velocity.x));

        if (!target) 
            return;

        if (canAtk && TargetInAtkDist())
            Attack();

        if (path == null || Vector2.Distance(target.position, rb.position) < nextWaypointDistance)      //yol yoksa yada hedef ile aradaki mesafe k���kse kovalama
            return;

        if (currentWaypoint >= path.vectorPath.Count)        
            return;

        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, 0.2f, whatIsGround);
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;       //bizim hedefimizin hangi y�nde old. belirler

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);       //kare k�k almaya gerek yok
        if (distance < nextWaypointDistance)
            currentWaypoint++;


        rb.AddForce(new Vector2(speed * Time.deltaTime * direction.x, 0));

        if (isGrounded && jumpEnabled && direction.y > jumpHeight)
        {
            rb.AddForce(Vector2.up * jumpForce);
            jumpEnabled = false;
            Invoke(nameof(JumpReset), 1.5f);
        }

        if (rb.velocity.x > 0.08f) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
        else if (rb.velocity.x < -0.08f) enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);
    }


    void Attack()
    {
        if (target.position.x - transform.position.x > 0) enemyBody.localScale = new Vector3(-bodyScale, bodyScale, 1);
        else enemyBody.localScale = new Vector3(bodyScale, bodyScale, 1);

        anim.SetTrigger("attack");
        canAtk = false;
        Invoke(nameof(AttackReset), atkSpeed);
        Invoke(nameof(SecAttackSprint), 1.6f);
    }
    void SecAttackSprint()
    {
        int a;
        if (enemyBody.localScale.x > 0)
            a = -1;
        else
            a = 1;
        FirstOOP.Push(rb, new Vector2(a, 0), atkSprint);
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
        return Vector2.SqrMagnitude(transform.position - target.position) < NWPDSquare;        
    }
    bool TargetInSeeDist()
    {
        return Vector2.SqrMagnitude(enemyBody.position - target.position) < seRngSquare;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seeRange);
    }
}