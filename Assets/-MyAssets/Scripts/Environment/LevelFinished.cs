using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class LevelFinished : MonoBehaviour
{
    public CanvasGroup levelEndImg;
    public float sceneOpenTime, sceneCloseTime;

    [Space(10)]
    public Tilemap caveTilemap;

    private void Start()
    {
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

    public void EndLevel(float waitTime)
    {
        PlayerPrefs.SetString("whichLevel", (int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());        //mevcut levelin 1 fazlas�n� kaydet

        levelEndImg.GetComponent<RectTransform>().DOScale(1, 0).SetUpdate(true).SetDelay(waitTime);
        levelEndImg.DOFade(1, sceneCloseTime).SetUpdate(true).SetDelay(waitTime).OnComplete(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("whichLevel"));                                                //ve mevcut levelin 1 fazlas�n� y�kle
        });
    }
}
