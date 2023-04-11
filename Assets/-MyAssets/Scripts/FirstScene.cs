using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstScene : MonoBehaviour
{
    public Light2D lightt;
    public CanvasGroup sceneFade, levelBtnsHldr;
    public Transform levelBtnsPnl;


    float time, lightMax, lightMin, lightNrml;
    bool levelPnlOpen;

    [Space(10)]
    public TextMeshProUGUI resetTxt;
    public TextMeshProUGUI yesTxt, levelsTxt;


    private void Start()
    {
        levelPnlOpen = false;
        time = 0;

        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.22f;
        lightMax = lightNrml + 0.22f;

        yesTxt.transform.parent.GetComponent<RectTransform>().DOScale(0, 0f);

        sceneFade.gameObject.SetActive(true);
        sceneFade.GetComponent<RectTransform>().DOScale(1, 0);
        sceneFade.DOFade(1, 0f);
        sceneFade.DOFade(0, 2f).OnComplete(() => sceneFade.GetComponent<RectTransform>().DOScale(0, 0));

        levelBtnsHldr.DOFade(0, 0);
        levelBtnsHldr.GetComponent<RectTransform>().DOScale(0, 0);

        int a = int.Parse(PlayerPrefs.GetString("whichLevel"));

        for (int i = 0; i < a; i++)
        {
            levelBtnsPnl.GetChild(i).GetComponent<Button>().interactable = true;
        }

        if (PlayerPrefs.GetInt("language", 0) == 0)     // 0 = türkçe
        {
            resetTxt.text = "   Oyunu Sýfýrla";
            yesTxt.text = "Evet !";
            levelsTxt.text = "Bölümler...";
        }
        else
        {
            resetTxt.text = "   Reset The Game";
            yesTxt.text = "Yes !";
            levelsTxt.text = "Levels...";
        }
    }
    private void Update()
    {
        if (time < Time.time)
        {
            time += 0.01f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.02f);
        }

        if (Input.anyKeyDown && !levelPnlOpen)   //herhangi bir tuþa basýnca burasý çalýþýr
            FirstLevel();
    }

    public void GoLevels(string index)
    {
        AudioManager.instance.PlaySound("WoodBtn");

        sceneFade.GetComponent<RectTransform>().DOScale(1, 0);
        sceneFade.DOFade(1, 2f).OnComplete(() =>                        //sahneyi kapatan siyah ekran açýldýktan sonra
        {
            Destroy(AudioManager.instance);
            SceneManager.LoadScene(index);
        });
    }

    public void FirstLevel()
    {
        if (PlayerPrefs.GetInt("firsOpen") == 0)     //oyun ilk kez açýlýyorsa Cinematic sahnesine gitsin
        {
            PlayerPrefs.SetFloat("soundLevel", 0.5f);       //ses ayarlansýn
            PlayerPrefs.SetInt("bolt", 1);
            Application.targetFrameRate = 55;

            sceneFade.DOKill();

            sceneFade.GetComponent<RectTransform>().DOScale(1, 0);
            sceneFade.DOFade(1, 2f).OnComplete(() =>                        //sahneyi kapatan siyah ekran açýldýktan sonra
            {
                PlayerPrefs.SetInt("firsOpen", 1);
                PlayerPrefs.SetInt("language", 1);      //dili ingilizceye ayarla
                SceneManager.LoadScene("Cinematic");
                               
            });
        }
        else
        {
            levelBtnsHldr.GetComponent<RectTransform>().DOScale(1, 0);
            levelBtnsHldr.DOFade(1, 1f);
            levelPnlOpen = true;
        }     
    }

    public void ResetGame()
    {
        AudioManager.instance.PlaySound("WoodBtn");

        yesTxt.transform.parent.GetComponent<RectTransform>().DOKill();
        yesTxt.transform.parent.GetComponent<RectTransform>().DOScale(1, 0.8f).SetEase(Ease.OutBack);
        yesTxt.transform.parent.GetComponent<RectTransform>().DOScale(0, 0.3f).SetEase(Ease.InBack).SetDelay(2f);
    }

    public void AbsouleteResetGame()
    {
        AudioManager.instance.PlaySound("WoodBtn");

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("FirstLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
