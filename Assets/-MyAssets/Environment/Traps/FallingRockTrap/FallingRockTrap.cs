using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FallingRockTrap : MonoBehaviour
{
    public ParticleSystem destroyRockPS;
    public GameObject stonePiece;

    bool oneTime;

    [Header("Stones")]
    public float damageAmount;
    public float minRockAmount, maxRockAmount;
    public float minOran, maxOran;
    public float addForcePozitifX, addForceNegatifX;              //normalý 95 iyidirz
    public float liveTime;
    public Sprite[] stoneSprite;

    [Header("Sounds")]
    public AudioSource rockSmashing1;
    public AudioSource rockSmashing2;

    float oneVolume, secondVolume;

    private void Start()
    {
        oneTime = true;

        oneVolume = rockSmashing1.volume;
        secondVolume = rockSmashing2.volume;
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
        if (FirstOOP.FiftyChance())
        {
            rockSmashing1.volume = oneVolume * PlayerPrefs.GetFloat("soundLevel");
            rockSmashing1.Play();
        }
        else
        {
            rockSmashing2.volume = secondVolume* PlayerPrefs.GetFloat("soundLevel");
            rockSmashing2.Play(); 
        }

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
            a.GetComponent<Rigidbody2D>().AddTorque(Random.Range(2, -2));

            a.GetComponent<FallingRock>().damage = Mathf.Round(damageAmount * b);
            a.GetComponent<FallingRock>().liveTime = liveTime;
        }      
    }
}