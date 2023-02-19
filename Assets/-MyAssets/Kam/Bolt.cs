using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bolt : MonoBehaviour
{   
    public GameObject boltExp;
    [HideInInspector] public float damage;
    public float destroyTime;

    float time, lightMax, lightMin, lightNrml;
    Light2D lightt;

    private void Start()
    {
        lightt = GetComponent<Light2D>();

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.4f;
        lightMax = lightNrml + 0.4f;

        Invoke(nameof(Explode), destroyTime - 0.4f);
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
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 1);
        }
        Explode();
    }

    public void Explode()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<ParticleSystem>().Stop();
        var trail = GetComponent<ParticleSystem>().trails;
        trail.enabled = false;
        boltExp.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 0.4f);
    }
}
