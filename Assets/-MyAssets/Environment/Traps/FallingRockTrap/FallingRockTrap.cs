using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FallingRockTrap : MonoBehaviour
{
    public ParticleSystem destroyRockPS;
    public GameObject stonePiece;

    [Header("Stones")]
    public float damageAmount;
    public float minRockAmount, maxRockAmount;
    public float minOran, maxOran;
    public float addForcePozitifX, addForceNegatifX;              //normalý 95 iyidirz
    public float liveTime;
    public Sprite[] stoneSprite;

    /*[Header("Sounds")]
    public AudioSource rope;
    public AudioSource rockSmashing1, rockSmashing2;*/

    bool oneTime;

    private void Start()
    {
        oneTime = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (oneTime  && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            oneTime = false;
            StartCoroutine(Rock());    
        }
    }

    IEnumerator Rock()
    {
        yield return new WaitForSeconds(0.4f);
        destroyRockPS.Play();
        int aa = Random.Range(0,2);
        /*if (aa == 0) rockSmashing1.Play();
        else rockSmashing2.Play();*/

        yield return new WaitForSeconds(0.2f);
        float aaa = Random.Range(minRockAmount, maxRockAmount + 1);
        for (int i = 0; i < aaa; i++) 
        {
            GameObject a = Instantiate(stonePiece, new Vector3(destroyRockPS.transform.position.x, destroyRockPS.transform.position.y + Random.Range(0.6f, 0.1f),0), 
                Quaternion.identity, transform);

            float b = Random.Range(minOran, maxOran);
            a.GetComponent<Transform>().DOScale(b, 0);
            a.GetComponent<SpriteRenderer>().sprite = stoneSprite[Random.Range(0, stoneSprite.Length)];
            a.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(addForcePozitifX, addForceNegatifX), Random.Range(40f, 25f)));
            a.GetComponent<Rigidbody2D>().AddTorque(Random.Range(10, -10));

            a.GetComponent<FallingRock>().damage = Mathf.Round(damageAmount * b);
            a.GetComponent<FallingRock>().liveTime = liveTime;
        }      
    }
}