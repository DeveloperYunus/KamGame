using DG.Tweening;
using TMPro;
using UnityEngine;

public class KamTalkColl : MonoBehaviour
{
    public TextMeshPro kamText;
    [Tooltip("Kam'ýn buraya gelince söyleyeceði söz (0 = tr, 1 = en...)")]
    public string[] think;
    public float readTime;

    bool firstTime;

    private void Start()
    {
        kamText.DOFade(0, 0f);
        kamText.text = null;
        firstTime = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && firstTime)
        {
            firstTime = false;

            kamText.DOKill();
            if (PlayerPrefs.GetInt("language") == 0)
            {
                kamText.DOFade(1, 0.5f).OnComplete(() => kamText.DOFade(0, 1f).SetDelay(readTime));
                kamText.text = think[0];
            }
            else
            {
                kamText.DOFade(1, 0.5f).OnComplete(() => kamText.DOFade(0, 1f).SetDelay(readTime));
                kamText.text = think[1];
            }
        }
    }
}
