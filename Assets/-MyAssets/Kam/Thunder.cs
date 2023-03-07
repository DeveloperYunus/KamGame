using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Thunder : MonoBehaviour
{
    public GameObject thunderExp;
    public LayerMask ground;
    [HideInInspector] public float damage;

    float time, lightMax, lightMin, lightNrml;
    Light2D lightt;

    private void Start()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10, ground);      //bulunduðum yerden, aþaðý doðru 5 birim ýþýn at ve ""ground" a çarparsa bana söyle
        //if (hit) transform.position = hit.point;

        lightt = GetComponent<Light2D>();

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.4f;
        lightMax = lightNrml + 0.4f;

        Destroy(gameObject, 0.5f);
    }
    private void Update()
    {
        if (time < Time.time)
        {
            time += 0.15f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.25f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 2);
        }
        GetComponent<Collider2D>().enabled = false;
        thunderExp.GetComponent<ParticleSystem>().Play();

        CinemachineShake.instance.ShakeCamera(2f, 1f, 0.7f);
    }
}
