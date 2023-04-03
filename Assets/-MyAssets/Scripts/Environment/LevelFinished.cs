using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class LevelFinished : MonoBehaviour
{
    public CanvasGroup levelEndImg;
    public float sceneOpenTime, sceneCloseTime;

    [Space(10)]
    public int bossCount;               //5,10 ve 15. ölümlerde bosslar var ve bosslar ölünce diðer level'e geçiyoruz. Bu deðiþken levelde kaç boss olduðunu söylüyor
                                        //tümm bosslarý öldürmemiz lazým
    int deadBossCount;

    [Space(10)]
    public Tilemap caveTilemap;

    [Space(10)]
    public bool isDarkLevel;
    public static bool isDarkLevels;


    private void Awake()
    {
        isDarkLevels = isDarkLevel;         //eðer karanlýk level ise falling leaf kapansýn ateþ külleri gözüksün ve kam'ýn asasý elektrik ile yansýn
    }
    private void Start()
    {
        deadBossCount = 0;
        caveTilemap.color = Color.white;

        levelEndImg.DOFade(1, 0).SetUpdate(true);
        levelEndImg.GetComponent<RectTransform>().DOScale(1, 0).SetUpdate(true);

        levelEndImg.DOFade(0, sceneOpenTime).SetUpdate(true);
        levelEndImg.GetComponent<RectTransform>().DOScale(0, 0).SetDelay(sceneOpenTime).SetUpdate(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EndLevel(0);
        }
    }

    void EndLevel(float waitTime)
    {
        PlayerPrefs.SetString("whichLevel", (int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());        //mevcut levelin 1 fazlasýný kaydet

        levelEndImg.GetComponent<RectTransform>().DOScale(1, 0).SetUpdate(true).SetDelay(waitTime);
        levelEndImg.DOFade(1, sceneCloseTime).SetUpdate(true).SetDelay(waitTime).OnComplete(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("whichLevel"));                                                //ve mevcut levelin 1 fazlasýný yükle
        });
    }

    public void EndLevelFromBoss(float waitTime)
    {
        deadBossCount++;

        if (deadBossCount < bossCount)
            return;


        PlayerPrefs.SetString("whichLevel", (int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());        //mevcut levelin 1 fazlasýný kaydet

        levelEndImg.GetComponent<RectTransform>().DOScale(1, 0).SetUpdate(true).SetDelay(waitTime);
        levelEndImg.DOFade(1, sceneCloseTime).SetUpdate(true).SetDelay(waitTime).OnComplete(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("whichLevel"));                                                //ve mevcut levelin 1 fazlasýný yükle
        });
    }
}
