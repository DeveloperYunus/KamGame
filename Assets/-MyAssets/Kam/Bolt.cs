using UnityEngine;

public class Bolt : MonoBehaviour
{   
    public GameObject boltExp;
    [HideInInspector] public float damage;
    public float destroyTime;

    private void Start()
    {
        Invoke(nameof(Explode), destroyTime - 0.4f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<EnemyHealth>()) other.GetComponent<EnemyHealth>().GetDamage(damage, 1);    //collider kendisindede parent objesindede olabiliyor 
            else other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 1);
        }
        Explode();
    }

    void Explode()
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
