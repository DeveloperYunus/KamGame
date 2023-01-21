using UnityEngine;

public class Thunder : MonoBehaviour
{
    public GameObject thunderExp;
    public LayerMask ground;
    [HideInInspector] public float damage;


    private void Start()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10, ground);      //bulunduðum yerden, aþaðý doðru 5 birim ýþýn at ve ""ground" a çarparsa bana söyle
        //if (hit) transform.position = hit.point;

        Destroy(gameObject, 0.5f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<EnemyHealth>()) other.GetComponent<EnemyHealth>().GetDamage(damage, 2);
            else other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 2);
        }
        GetComponent<Collider2D>().enabled = false;
        thunderExp.GetComponent<ParticleSystem>().Play();
    }
}
