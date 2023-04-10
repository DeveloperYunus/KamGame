using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour
{
    public GameObject fadeImage;
    public GameObject picture1;
    public GameObject picture2;
    public GameObject picture3;

    [Space(10)]
    public CanvasGroup UIpanel;
    public TextMeshProUGUI devamTxt;

    [Space(10)]
    public AudioSource clipSource;
    public AudioClip trLore, enLore;

    private void Start()
    {
        UIpanel.DOFade(0, 0f);
        UIpanel.DOFade(1, 1f).OnComplete(() => UIpanel.interactable = true);
    }

    public void LanguageBtn(int index)
    {
        PlayerPrefs.SetInt("language", index);
        AudioManager.instance.PlaySound("WoodBtn");

        if (index == 0) devamTxt.text = "Devam...";
        else            devamTxt.text = "Continue...";
    }
    public void Continue()
    {
        AudioManager.instance.PlaySound("WoodBtn");

        UIpanel.DOFade(0, 1f);

        fadeImage.GetComponent<CanvasGroup>().blocksRaycasts = false;

        fadeImage.GetComponent<CanvasGroup>().DOFade(0, 3f).SetDelay(2f);
        StartCoroutine(MovePictue());
    }

    IEnumerator MovePictue()
    {
        yield return new WaitForSeconds(2);
        if (PlayerPrefs.GetInt("language") == 0)        //dile göre lore'u seç ve sonra anlatmaya baþla
            clipSource.clip = trLore;
        else
            clipSource.clip = enLore;

        clipSource.Play();

        yield return new WaitForSeconds(40);
        fadeImage.GetComponent<CanvasGroup>().DOFade(1, 3f);
        picture2.GetComponent<CanvasGroup>().DOFade(1, 0).SetDelay(3f);
        fadeImage.GetComponent<CanvasGroup>().DOFade(0, 3f).SetDelay(3.1f);

        yield return new WaitForSeconds(15);
        fadeImage.GetComponent<CanvasGroup>().DOFade(1, 3f);
        picture3.GetComponent<CanvasGroup>().DOFade(1, 0).SetDelay(3f);
        fadeImage.GetComponent<CanvasGroup>().DOFade(0, 3f).SetDelay(3.1f);

        yield return new WaitForSeconds(20);
        fadeImage.GetComponent<CanvasGroup>().DOFade(1, 3f).OnComplete(() => SceneManager.LoadScene("1"));
    }

    public void PassScene()
    {
        fadeImage.GetComponent<CanvasGroup>().DOFade(1, 1.5f).OnComplete(() => SceneManager.LoadScene("1"));
    }
}
