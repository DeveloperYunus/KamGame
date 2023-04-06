using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutroLevel : MonoBehaviour
{
    public Image darkBG;
    public CanvasGroup text;

    [Space(10)]
    public AudioClip trOutro;
    public AudioClip enOutro;

    private void Start()
    {
        darkBG.DOFade(1, 0);
        darkBG.DOFade(0, 3);

        if (PlayerPrefs.GetInt("language") == 0)
        {
            GetComponent<AudioSource>().clip = trOutro;

            darkBG.DOFade(1, 3).SetDelay(40);
            text.DOFade(1, 2).SetDelay(44);
            Invoke(nameof(GoLevelMenu), 50);
        }
        else
        {
            GetComponent<AudioSource>().clip = enOutro;

            darkBG.DOFade(1, 3).SetDelay(45);
            text.DOFade(1, 2).SetDelay(49);
            Invoke(nameof(GoLevelMenu), 55);
        }

        GetComponent<AudioSource>().PlayDelayed(4f);
    }

    void GoLevelMenu()
    {
        SceneManager.LoadScene("FirstLevel");
    }
}
