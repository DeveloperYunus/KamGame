using DG.Tweening;
using System;
using System.Collections;
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
    public GameObject camTarget;

    [Header("Particles")]
    public ParticleSystem soilParticle;                 //soil particle
    public ParticleSystem.EmissionModule soilPSEmis;                   //Soil particlenin emission modülü

    public ParticleSystem jumpPS;
    public ParticleSystem levelUpPS;

    [HideInInspector] public float slow;                                                        //hasar alýnca yavaþlamasý için (spike slow buna dahil)
    [HideInInspector] public int facingRight;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool canMove;
    Rigidbody2D rb;
    float moveInput;
    float soilTime;                         //ayaklardan çýkan toprak için
    int goRight, goLeft;
    int extraJumpValue;
    bool oneFallSound;                      //yere düþünce ses çýkmasý için

    Animator anim;
    [HideInInspector] public float animSlow;                                                    //saldýrýken yavaþlamasý için

    [Header("Text Target")]
    public Transform kamText;
    public Transform textTarget;
    public float flwSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        soilPSEmis = soilParticle.emission;

        canMove = true;
        oneFallSound = false;

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
                    if (FirstOOP.FiftyChance()) audioManager.playSound("footStep1");
                    else audioManager.playSound("footStep2");

                    footStepTimer = 0;
                }
            }*/

            if (oneFallSound)
            {
                //audioManager.playSound("fallSoil1");
                oneFallSound = false;
                jumpPS.Play();
            }
        }
        else if (rb.velocity.y < -0.05f) 
            oneFallSound = true;


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

        kamText.position = Vector3.Slerp(kamText.position, textTarget.position, flwSpeed);              //kam'ýn düþünce textininin kendini takip etmesini saðlar


        if (canMove)
        {
            //if (facingRight == -1 && moveInput > 0.1)       FlipFace();
            //else if (facingRight == 1 && moveInput < -0.1)  FlipFace();
            if (facingRight == -1 && rb.velocity.x > 0.1)       FlipFace();     //ateþ ederken yön deðiþtirsin diye bu 2. satýrlarý kullandýk
            else if (facingRight == 1 && rb.velocity.x < -0.1)  FlipFace();

            if (goRight == 1)       moveInput = Mathf.Lerp(moveInput, 1, 0.15f);
            else if (goLeft == -1)  moveInput = Mathf.Lerp(moveInput, -1, 0.15f);
            else                    moveInput = Mathf.Lerp(moveInput, 0, 0.2f);

            if (isGrounded)
            {
                rb.velocity = new Vector2(moveInput * speed * animSlow * slow, rb.velocity.y);           //bunun sayesinde hýz her zamana sýfý normal fizik sstemlerini bozuyor o yüzden alttakini kullandýk
                rb.drag = 0;                
            }
            else
            {
                rb.AddForce(new Vector2(moveInput * speed * 60 * Time.deltaTime * animSlow * slow, 0));
                rb.drag = 0.8f;
            }

            if (Time.time > soilTime)       //ayaðýmýzdan çýkan toprak için
            {
                soilTime = Time.time + 0.2f;
                if (isGrounded && Mathf.Abs(rb.velocity.x) > 0.15f)
                    soilPSEmis.rateOverTime = 10;
                else
                    soilPSEmis.rateOverTime = 0;
            }

            anim.speed = slow;                                                               //animasyonlarýn hýzýný ayarlar
            anim.SetFloat("speedx", Mathf.Abs(rb.velocity.x));
            anim.SetFloat("speedy", rb.velocity.y);
        }        
    }

    void Jump()
    {
        if (isGrounded)
        {
            jumpPS.Play();
            rb.velocity = new Vector2 (rb.velocity.x, jumpForce* slow);
            //rb.AddForce(jumpForce * Vector2.up * slow);
        }
        else if (extraJump > 0)
        {
            jumpPS.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * slow);
            //rb.AddForce(jumpForce * Vector2.up * slow);
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

            camTarget.transform.DOKill();
            camTarget.transform.localPosition = new(-camTarget.transform.localPosition.x, 0, 0);
            camTarget.transform.DOLocalMoveX(2.5f, 2f);
        }
    }
    public void StopRun()                       //dýþarýdan kam'ýn hýzýný sýfýrlamak ve animasyonunu durdurmak için
    {
        if (isGrounded)
        {
            anim.SetFloat("speedx", 0);
            rb.velocity = Vector2.zero;
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
    public IEnumerator StopWalkAndPush(float time, Vector2 direction, float strong)     //sýnýf bosslarýnýn bizi itmesi için
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