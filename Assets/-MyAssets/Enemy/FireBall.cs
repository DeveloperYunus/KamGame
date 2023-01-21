using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject fireExp;
    [HideInInspector] public float damage;
    [HideInInspector] public int dmgKind;
    public float destroyTime;

    [HideInInspector] public bool canDmg;

    private void Start()
    {
        canDmg = true;
        Invoke(nameof(Explode), destroyTime - 1f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("EnmySword"))
            Explode();
    }

    void Explode()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<ParticleSystem>().Stop();
        fireExp.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 1f);
    }
}
