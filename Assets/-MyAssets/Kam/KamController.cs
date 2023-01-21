using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;
    public int extraJump;
    public float checkRadius;
    public Transform groundCheck;
    public LayerMask whatIsGround;

    Rigidbody2D rb;
    float moveInput;
    [HideInInspector] public float slow;                                                        //hasar alýnca yavaþlamasý için
    [HideInInspector] public int facingRight;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool canMove;
    int goRight, goLeft;
    int extraJumpValue;

    Animator anim;
    [HideInInspector] public float animSlow;                                                                       //saldýrýp hasar alýrken yavaþla.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        canMove = true;
        slow = 1;
        facingRight = 1;
        extraJumpValue = extraJump;
        animSlow = 1;
    }
    void Update()
    {
        if (isGrounded)
        {
            extraJump = extraJumpValue;

            /*
            if (Mathf.Abs(rb.velocity.x) > 0.15f)
            {
                if (footStepTimer <= 0.28f)
                    footStepTimer += Time.deltaTime;
                else
                {
                    int a = Random.Range(0, 2);
                    if (a == 0)
                        audioManager.playSound("footStep1");
                    else
                        audioManager.playSound("footStep2");

                    footStepTimer = 0;
                }
            }
            
            if (oneFallSound)
            {
                audioManager.playSound("fallSoil1");
                oneFallSound = false;
            }*/
        }
        //else if (rb.velocity.y < -0.05f) oneFallSound = true;


        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            goLeft = -1;
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            goLeft = 0;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            goRight = 1;
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            goRight = 0;

        if (canMove)
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                Jump();
        
    }
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);              //yere deðiyormu kontrolu yapar

        if (canMove)
        {
            if (facingRight == -1 && moveInput > 0.1)       FlipFace();
            else if (facingRight == 1 && moveInput < -0.1)  FlipFace();

            if (goRight == 1)       moveInput = Mathf.Lerp(moveInput, 1, 0.25f);
            else if (goLeft == -1)  moveInput = Mathf.Lerp(moveInput, -1, 0.25f);
            else                    moveInput = Mathf.Lerp(moveInput, 0, 0.25f);
       
            rb.velocity = new Vector2(moveInput * speed * animSlow * slow, rb.velocity.y);

            //anim.speed = 1 * animSlow;                                                                            //animasyonlarýn hýzýný ayarlar
            anim.SetFloat("speedx", Mathf.Abs(rb.velocity.x));
            anim.SetFloat("speedy", rb.velocity.y);
        }        
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = jumpForce * Vector2.up * slow;
        }
        else if (extraJump > 0)
        {
            rb.velocity = jumpForce * Vector2.up * slow;
            extraJump--;
        }
    }
    public void FlipFace()
    {
        if (GetComponent<KamAttack>().animTransition <= 0)
        {
            facingRight *= -1;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;

            //cmvc.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2f;     //x damping yada ofset böyle deðiþtirilir
            //Invoke(nameof(CamOfsettSmooth),1.1f);
        }
    }

    public void GoRight(bool a)
    {
        if (a) goLeft = -1;
        else goLeft = 0;
    }
    public void GoLeft(bool a)
    {
        if (a) goRight = 1;
        else goRight = 0;
    }
    public void WhichBtnPress(GameObject a)
    {
        a.GetComponent<Animator>().SetBool("up", false);
        a.GetComponent<Animator>().SetBool("press", true);
    }
    public void WhichBtnUp(GameObject a)
    {
        a.GetComponent<Animator>().SetBool("press", false);
        a.GetComponent<Animator>().SetBool("up", true);
    }  

    public IEnumerator SetSlow(float slowValue, float time)       //slow için ise true, deðilde itme için ise false olacak
    {
        slow = slowValue;
        yield return new WaitForSeconds(time);
        slow = 1;
    }

    public IEnumerator StopWalkAndPush(float time, Vector2 direction, float strong)
    {
        canMove= false;
        FirstOOP.Push(rb, direction, strong);
        anim.SetFloat("speedx", 0);
        anim.SetFloat("speedy", 0);
        yield return new WaitForSeconds(time);
        canMove = true;
        StartCoroutine(SetSlow(0.6f, 0.4f));
    }
}
