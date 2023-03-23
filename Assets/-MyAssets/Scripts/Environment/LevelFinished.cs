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

        levelEndImg.DOFade(1, 0);
        levelEndImg.GetComponent<RectTransform>().DOScale(1, 0);

        levelEndImg.DOFade(0, sceneOpenTime);
        levelEndImg.GetComponent<RectTransform>().DOScale(0, 0).SetDelay(sceneOpenTime);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            levelEndImg.DOFade(1, sceneCloseTime);
            levelEndImg.GetComponent<RectTransform>().DOScale(1, 0);

            PlayerPrefs.SetInt("whichLevel", int.Parse(SceneManager.GetActiveScene().name) + 1);        //mevcut levelin 1 fazlasýný kaydet
        }
    }
}
