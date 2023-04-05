using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bolt : MonoBehaviour
{   
    public GameObject boltExp;
    [HideInInspector] public float damage;
    public float destroyTime;

    float time, lightMax, lightMin, lightNrml;
    Light2D lightt;

    bool canSoundable;                      //boltlar cok uzakta patlayýnca ses cýkamalarý tuhaf oluyor diye bu var

    private void Start()
    {
        lightt = GetComponent<Light2D>();

        time = 0;
        canSoundable = true;

        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.4f;
        lightMax = lightNrml + 0.4f;

        Invoke(nameof(StartExp), destroyTime - 0.4f);
    }
    private void Update()
    {
        if (time < Time.time)
        {
            time += 0.15f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.15f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.GetComponentInParent<EnemyHealth>())
        {
            other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 1);
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<BOSSHealth>().GetDamage(damage, 1);
        }
        Explode();
    }

    void StartExp()
    {
        canSoundable = false;
        Explode();
    }

    public void Explode()
    {
        if (canSoundable)
        {
            int a = Random.Range(1, 3);
            if (a == 0)
                AudioManager.instance.PlaySound("BoltExp1");
            else if (a == 1)
                AudioManager.instance.PlaySound("BoltExp2");
            else
                AudioManager.instance.PlaySound("BoltExp3");
        }

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<ParticleSystem>().Stop();
        var trail = GetComponent<ParticleSystem>().trails;
        trail.enabled = false;
        boltExp.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 0.4f);
    }
}
