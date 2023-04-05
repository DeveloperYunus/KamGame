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
        if (other.CompareTag("Enemy") && isActive && other.GetComponentInParent<EnemyHealth>())          //aktif olduktan sonra hasar versin diye
        {
            other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 3);
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<BOSSHealth>().GetDamage(damage, 1);
        }

        if (other.CompareTag("Enemy") && !isActive)          //tuzaðý aktifleþtirmek için
        {
            box.enabled = false;
            circle.enabled = true;

            isActive = true;
            lightt.enabled = true;

            if (FirstOOP.FiftyChance())
                AudioManager.instance.PlaySound("ElkTrap1");
            else
                AudioManager.instance.PlaySound("ElkTrap2");

            if (Vector2.SqrMagnitude(GameObject.Find("Kam").transform.position - transform.position) < 121) //eðer kam ile aramda 11 in karesi kadar mesafe varsa ekran sarsýlmasýn
                CinemachineShake.instance.ShakeCamera(2f, 1f, 0.7f);

            explode.Play();
            Destroy(gameObject, 0.5f);
        }        
    }
}