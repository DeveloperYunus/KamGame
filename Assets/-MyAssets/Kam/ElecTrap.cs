using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ElecTrap : MonoBehaviour
{
    public float damage;
    public ParticleSystem explode;

    [Header("")]
    public Collider2D box;
    public Collider2D circle;

    bool isActive;
    float time, lightMax, lightMin, lightNrml;
    Light2D lightt;

    private void Start()
    {
        lightt = GetComponent<Light2D>();

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.4f;
        lightMax = lightNrml + 0.4f;
        isActive = false;
    }
    private void Update()
    {
        if (isActive && time < Time.time)
        {
            time += 0.15f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.15f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isActive)          //aktif olduktan sonra hasar versin diye
        {
            other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 3);
        }

        if (other.CompareTag("Enemy") && !isActive)          //tuzaðý aktifleþtirmek için
        {
            box.enabled = false;
            circle.enabled = true;

            isActive = true;
            lightt.enabled = true;

            explode.Play();
            Destroy(gameObject, 0.5f);
        }        
    }
}