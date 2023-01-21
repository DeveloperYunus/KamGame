using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecTrap : MonoBehaviour
{
    public float damage;
    public ParticleSystem explode;
    public static int isElectrapAmount;

    private void Start()
    {
        isElectrapAmount++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<EnemyHealth>()) other.GetComponent<EnemyHealth>().GetDamage(damage, 3);
            else other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 3);
            explode.Play();
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnDisable()
    {
        isElectrapAmount--;
    }
}
