using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecTrap : MonoBehaviour
{
    public float damage;
    public ParticleSystem explode;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 3);

            explode.Play();
            Destroy(gameObject, 0.5f);
        }
    }
}
