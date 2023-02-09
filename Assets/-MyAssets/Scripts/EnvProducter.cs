using UnityEngine;

public class EnvProducter : MonoBehaviour
{
    [Range(0,3)]
    public int envDenisty;                      //cevre yo�unlu�u-s�kl���       GetComponent<SpriteRenderer>().bounds.size.x;
    public GameObject deneme;
    public LayerMask ground;

    [Header("Scale")]
    public float flwrScale;
    public float rockScale;
    public float bushScale;
    public float lttlTreeScale;
    public float bgTreeScale;

    EnvHolder envHldr;
    float negativeRng, range;
    float grndX, grndY;

    //0. t�r : 1 �i�ek 1 �al�
    //1. t�r : 1 �al� 2 �i�ek 1 ta�
    //2. t�r : 1 ku�uk a�a� 2 �al� 2 �i�ek 1 ta� 
    //3. t�r : 1 b�y�k a�a� 2 k���k a�a� 2 �al� 3 �i�ek 2 ta�

    private void Start()
    {
        range = (envDenisty * 1.5f + 1);
        negativeRng = -1 * range;
        envHldr = GetComponentInParent<EnvHolder>();

        RaycastHit2D checkGround = Physics2D.Raycast(transform.position, Vector2.down, 4, ground);
        grndX = checkGround.point.x;
        grndY = checkGround.point.y;

        if (checkGround)
            ProduceEnv();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) 
        {
            DeleteChild();
        }
    }

    void ProduceEnv()
    {
        switch (envDenisty)
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
    GameObject InsEnv(Sprite sprt, float size)
    {
        GameObject a = Instantiate(deneme, transform.position, Quaternion.identity, transform);
        a.GetComponent<SpriteRenderer>().sprite = sprt;
        a.transform.position = new(grndX + Random.Range(range, negativeRng),
                                   grndY + 0.07f + a.GetComponent<SpriteRenderer>().sprite.bounds.size.y * size / 2, 0);

        GeySymmetry(a, size);
        return a;
    }
    void InsFlwrMshrm()
    {
        float b = Random.Range(flwrScale - 0.15f, flwrScale + 0.1f);
        GameObject a = InsEnv(envHldr.flwrMshrm[Random.Range(0, envHldr.flwrMshrm.Length)], b);
        a.GetComponent<SpriteRenderer>().sortingOrder = 11;  //���ekler ve mantarlar di�erlerinden �nde olsun diye

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

    void IsObjectFront(GameObject a, float scale)
    {
        int b = Random.Range(0, 5);
        if (b == 0)
        {
            a.transform.position = new(grndX + Random.Range(range, negativeRng), grndY + (a.GetComponent<SpriteRenderer>().sprite.bounds.size.y * scale / 2) - 0.05f, 0);
            a.GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
            a.GetComponent<SpriteRenderer>().sortingOrder = 11;
        }
    }
    void GeySymmetry(GameObject a, float b)
    {
        if (FirstOOP.FiftyChance()) a.GetComponent<Transform>().localScale = new(b, b, b);              //rasgele olarak objeyi simetrik al�r
        else a.GetComponent<Transform>().localScale = new(-b, b, b);
    }

 
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - new Vector3(envDenisty * 1.5f + 1, 3, 0), transform.position + new Vector3(envDenisty * 1.5f + 1, -3, 0));
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, 3, 0));
    }
    void DeleteChild()
    {
        int a = transform.childCount;
        for (int i = a-1; i > -1; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        ProduceEnv();
    }
}