using System.Drawing;
using UnityEngine;

public class EnvProducter : MonoBehaviour
{
    [Range(0,3)]
    public int envDensity;                      //cevre yoðunluðu-sýklýðý       GetComponent<SpriteRenderer>().bounds.size.x;
    public GameObject deneme;
    public LayerMask ground;

    [Header("Kind")]
    public bool isForCave;
    [Range(0, 1)]
    public int envDensityCave;                      //cevre yoðunluðu-sýklýðý       GetComponent<SpriteRenderer>().bounds.size.x;

    [Header("Scale")]
    public float flwrScale;
    public float rockScale;
    public float bushScale;
    public float lttlTreeScale;
    public float bgTreeScale;

    EnvHolder envHldr;
    float negativeRng, range;
    float grndX, grndY;

    //Yeþil environment için
    //0. tür : 1 çiçek 1 çalý
    //1. tür : 1 çalý 2 çiçek 1 taþ
    //2. tür : 1 kuçuk aðaç 2 çalý 2 çiçek 1 taþ 
    //3. tür : 1 büyük aðaç 2 küçük aðaç 2 çalý 3 çiçek 2 taþ

    //Maðaralar için
    //0. tür : 2 mantar
    //1. tür : 2 mantar 1 taþ 

    private void Start()
    {
        RaycastHit2D checkGround = Physics2D.Raycast(transform.position, Vector2.down, 4, ground);
        grndX = checkGround.point.x;
        grndY = checkGround.point.y;

        envHldr = GetComponentInParent<EnvHolder>();

        if (!isForCave)
        {
            range = (envDensity * 1.5f + 0.5f);
            negativeRng = -1 * range;

            if (checkGround)
                ProduceEnv();
        }
        else        //demekki bu obje maðarada
        {
            range = (envDensity + 0.5f);
            negativeRng = -1 * range;

            if (checkGround)
                ProduceCaveEnv();
        }
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) 
        {
            DeleteChild();
            print("silinecek");
        }
    }

    void ProduceEnv()
    {
        switch (envDensity)
        {
            case 0:
                InsBush(); 
                InsFlwrMshrm();
                break;

            case 1:
                InsBush(); 
                InsRock();
                for (int i = 0; i < 2; i++) 
                {
                    InsFlwrMshrm();
                }
                break;

            case 2:
                InsLittleTree();
                InsRock();
                for (int i = 0; i < 2; i++)
                {
                    InsFlwrMshrm();
                    InsBush();
                }
                break;

            case 3:
                InsBigTree();
                InsLittleTree();
                for (int i = 0; i < 2; i++)
                {
                    InsBush(); 
                    InsRock();
                }
                for (int i = 0; i < 3; i++)
                {
                    InsFlwrMshrm();
                }
                break;
        }
    }
    void ProduceCaveEnv()
    {
        switch (envDensityCave)
        {
            case 0:
                InsMshrmCave();
                break;

            case 1:
                InsRockCave();
                InsMshrmCave();
                break;
        }
    }
    GameObject InsEnv(Sprite sprt, float size)
    {
        GameObject a = Instantiate(deneme, transform.position, Quaternion.identity, transform);
        a.GetComponent<SpriteRenderer>().sprite = sprt;
        a.transform.position = new(grndX + Random.Range(range, negativeRng),
                                   grndY + 0.07f + a.GetComponent<SpriteRenderer>().sprite.bounds.size.y * size / 2, 0);

        a.GetComponent<SpriteRenderer>().sortingOrder = EnvHolder.envAmount;
        EnvHolder.envAmount++;

        GeySymmetry(a, size);
        return a;
    }
    void InsFlwrMshrm()
    {
        float b = Random.Range(flwrScale - 0.15f, flwrScale + 0.1f);
        GameObject a = InsEnv(envHldr.flwrMshrm[Random.Range(0, envHldr.flwrMshrm.Length)], b);
        a.GetComponent<SpriteRenderer>().sortingOrder = 500 + EnvHolder.envAmount;  //çýçekler ve mantarlar diðerlerinden önde olsun diye
        EnvHolder.envAmount++;

        IsObjectFront(a, b);
    }
    void InsRock()
    {
        float b = Random.Range(rockScale - 0.15f, rockScale + 0.1f);
        GameObject a = InsEnv(envHldr.rock[Random.Range(0, envHldr.rock.Length)], b);

        IsObjectFront(a, b);
    }
    void InsBush()
    {
        float b = Random.Range(bushScale - 0.15f, bushScale + 0.1f);
        InsEnv(envHldr.bush[Random.Range(0, envHldr.bush.Length)], b);
    }
    void InsLittleTree()
    {
        float b = Random.Range(lttlTreeScale - 0.15f, lttlTreeScale + 0.1f);
        InsEnv(envHldr.littleTree[Random.Range(0, envHldr.littleTree.Length)], b);
    }
    void InsBigTree()
    {
        float b = Random.Range(bgTreeScale - 0.15f, bgTreeScale + 0.1f);
        InsEnv(envHldr.bigTree[Random.Range(0, envHldr.bigTree.Length)], b);
    }

    void InsMshrmCave()
    {
        float b = Random.Range(flwrScale - 0.15f, flwrScale + 0.1f);
        GameObject a = InsEnv(envHldr.justMushroom[Random.Range(0, envHldr.justMushroom.Length)], b);
        a.GetComponent<SpriteRenderer>().sortingOrder = 500 + EnvHolder.envAmount;  //çýçekler ve mantarlar diðerlerinden önde olsun diye
        EnvHolder.envAmount++;

        a.transform.position = new(grndX + Random.Range(range, negativeRng),
                                   grndY + a.GetComponent<SpriteRenderer>().sprite.bounds.size.y * b / 2, 0);   //maðaradaki objeler biraz daha aþaðýda ve ground un arkasýnda olsun

        a.GetComponent<SpriteRenderer>().sortingLayerName = "CaveBG";
    }
    void InsRockCave()
    {
        float b = Random.Range(rockScale - 0.15f, rockScale + 0.1f);
        GameObject a = InsEnv(envHldr.rock[Random.Range(0, envHldr.rock.Length)], b);

        a.transform.position = new(grndX + Random.Range(range, negativeRng),
                                   grndY + a.GetComponent<SpriteRenderer>().sprite.bounds.size.y * b / 2, 0);   //maðaradaki objeler biraz daha aþaðýda ve ground un arkasýnda olsun

        a.GetComponent<SpriteRenderer>().sortingLayerName = "CaveBG";
    }

    void IsObjectFront(GameObject a, float scale)
    {
        int b = Random.Range(0, 5);
        if (b == 0)
        {
            a.transform.position = new(grndX + Random.Range(range, negativeRng),  grndY + (a.GetComponent<SpriteRenderer>().sprite.bounds.size.y * scale / 2) - 0.05f, 0);

            a.GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
            a.GetComponent<SpriteRenderer>().sortingOrder = 11 + EnvHolder.envAmount;
            EnvHolder.envAmount++;
        }
    }
    void GeySymmetry(GameObject a, float b)
    {
        if (FirstOOP.FiftyChance()) a.GetComponent<Transform>().localScale = new(b, b, b);              //rasgele olarak objeyi simetrik alýr
        else a.GetComponent<Transform>().localScale = new(-b, b, b);
    }

 
    private void OnDrawGizmosSelected()
    {
        if (!isForCave)
        {
            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawLine(transform.position - new Vector3(envDensity * 1.5f + 0.5f, 3, 0), transform.position + new Vector3(envDensity * 1.5f + 0.5f, -3, 0));
            Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, 3, 0));
        }
        else
        {
            Gizmos.color = UnityEngine.Color.magenta;
            Gizmos.DrawLine(transform.position - new Vector3(envDensityCave + 0.5f, 3, 0), transform.position + new Vector3(envDensityCave + 0.5f, -3, 0));
            Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, 3, 0));
        }
    }
    void DeleteChild()
    {
        int a = transform.childCount;
        for (int i = a-1; i > -1; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        ProduceEnv();
        ProduceCaveEnv();
    }
}