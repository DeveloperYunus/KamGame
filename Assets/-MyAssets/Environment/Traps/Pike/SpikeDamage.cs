using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    [HideInInspector]
    public float damage;

    //yukarý cýkma iþlevi bitince bunu kaydeder cunku yukarý cýkarken yakalanýrsa daha cok hasar alýr.
    [HideInInspector]
    public bool IsDownRise;
    public static float playerAnimatorSpeed, playerNewSpeed;

    public AudioSource triggered1;
    public AudioSource slash1, slash2;
    public AudioSource pikeUp1, pikeUp2;

    private void Start()
    {
        IsDownRise = false;
        playerAnimatorSpeed = 1;
        playerNewSpeed = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int a = Random.Range(0, 2);
        float b = PlayerPrefs.GetFloat("soundLevel");

        slash1.volume = b;
        slash2.volume = b;
        pikeUp1.volume = b;
        pikeUp2.volume = b;
        triggered1.volume = b;

        if (other.CompareTag("Player"))
        {
            if (!IsDownRise)
            {
                other.GetComponent<KamHealth>().GetDamage(damage, 6);        //kam hasar alsýn ve yavaþlasýn;
                if (a == 0) slash1.Play();
                else slash2.Play();
            }
            else if (other.GetComponent<Rigidbody2D>().velocity.y < -0.05f)
            {
                other.GetComponent<KamHealth>().GetDamage(damage * 0.65f, 6);        //kam hasar alsýn ve yavaþlasýn;
                if (a == 0) slash1.Play();
                else slash2.Play();
            }
            other.GetComponent<KamController>().slow = 0.5f;
        }
        else if (other.CompareTag("Enemy"))
        {
            if (!IsDownRise)
            {
                other.GetComponentInParent<EnemyHealth>().GetDamage(damage, 6);    

                if (a == 0) slash1.Play();
                else slash2.Play();
            }
            else if (other.GetComponentInParent<Rigidbody2D>().velocity.y < -0.05f)
            {
                other.GetComponentInParent<EnemyHealth>().GetDamage(damage * 0.65f, 1);
                if (a == 0) slash1.Play();
                else slash2.Play();     
            }

            if (other.GetComponentInParent<MeleeAI>()) other.GetComponentInParent<MeleeAI>().slow = 0.65f;

            if (other.GetComponentInParent<MleBossAI>()) other.GetComponentInParent<MleBossAI>().slow = 0.85f;

            if (other.GetComponent<RangedAI>()) other.GetComponent<RangedAI>().slow = 0.65f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<KamController>().slow = 1f;
        }
        else if(other.CompareTag("Enemy"))
        {
            if (other.GetComponentInParent<MeleeAI>()) other.GetComponentInParent<MeleeAI>().slow = 1f;

            if (other.GetComponentInParent<MleBossAI>()) other.GetComponentInParent<MleBossAI>().slow = 1f;

            if (other.GetComponent<RangedAI>()) other.GetComponent<RangedAI>().slow = 1f;
        }
    }
}