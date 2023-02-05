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
            if (other.GetComponent<EnemyHealth>()) other.GetComponent<EnemyHealth>().GetDamage(damage, 3);  //if'li ifade olmasýnýn sebebi bazý collidelar enemyde bazýlarý enemy'nin chilýnda
            else other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 3);

            explode.Play();
            Destroy(gameObject, 0.5f);
        }
    }
}
