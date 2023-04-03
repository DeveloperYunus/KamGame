using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class LevelFinished : MonoBehaviour
{
    public CanvasGroup levelEndImg;
    public float sceneOpenTime, sceneCloseTime;

    [Space(10)]
    public int bossCount;               //5,10 ve 15. �l�mlerde bosslar var ve bosslar �l�nce di�er level'e ge�iyoruz. Bu de�i�ken levelde ka� boss oldu�unu s�yl�yor
                                        //t�mm bosslar� �ld�rmemiz laz�m
    int deadBossCount;

    [Space(10)]
    public Tilemap caveTilemap;

    [Space(10)]
    public bool isDarkLevel;
    public static bool isDarkLevels;


    private void Awake()
    {
        isDarkLevels = isDarkLevel;         //e�er karanl�k level ise falling leaf kapans�n ate� k�lleri g�z�ks�n ve kam'�n asas� elektrik ile yans�n
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
        PlayerPrefs.SetString("whichLevel", (int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());        //mevcut levelin 1 fazlas�n� kaydet

        levelEndImg.GetComponent<RectTransform>().DOScale(1, 0).SetUpdate(true).SetDelay(waitTime);
        levelEndImg.DOFade(1, sceneCloseTime).SetUpdate(true).SetDelay(waitTime).OnComplete(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("whichLevel"));                                                //ve mevcut levelin 1 fazlas�n� y�kle
        });
    }

    public void EndLevelFromBoss(float waitTime)
    {
        deadBossCount++;

        if (deadBossCount < bossCount)
            return;


        PlayerPrefs.SetString("whichLevel", (int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());        //mevcut levelin 1 fazlas�n� kaydet

        levelEndImg.GetComponent<RectTransform>().DOScale(1, 0).SetUpdate(true).SetDelay(waitTime);
        levelEndImg.DOFade(1, sceneCloseTime).SetUpdate(true).SetDelay(waitTime).OnComplete(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("whichLevel"));                                                //ve mevcut levelin 1 fazlas�n� y�kle
        });
    }
}
