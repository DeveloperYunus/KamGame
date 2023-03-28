using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireBall : MonoBehaviour
{
    public GameObject fireExp;
    [HideInInspector] public float damage;
    [HideInInspector] public int dmgKind;
    public float destroyTime;

    [HideInInspector] public bool canDmg;

    float time, lightMax, lightMin, lightNrml;                      //ýþýk yanýp sönmesi için
    Light2D lightt;

    private void Start()
    {
        if (FirstOOP.FiftyChance())
            AudioManager.instance.PlaySound("FireBallGo1");
        else
            AudioManager.instance.PlaySound("FireBallGo2");

        lightt = GetComponent<Light2D>();

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.6f;
        lightMax = lightNrml + 0.6f;

        canDmg = true;
        Invoke(nameof(Explode), destroyTime - 1f);
    }
    private void Update()
    {
        if (time < Time.time)
        {
            time += 0.15f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.3f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("EnmySword") && !other.CompareTag("EnmyBullet"))       
            Explode();       
    }

    void Explode()
    {
        if (FirstOOP.FiftyChance())
            AudioManager.instance.PlaySound("FireBallExp1");
        else
            AudioManager.instance.PlaySound("FireBallExp2");


        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<ParticleSystem>().Stop();
        fireExp.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 1f);
    }
}
