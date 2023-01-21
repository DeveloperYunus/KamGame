using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEditor;

public class FloatingHP : MonoBehaviour
{
    public GameObject hpText;
    public int objCountInPool;

    public static ObjectPooling hpTextPool;

    void Start()
    {
        hpTextPool = new ObjectPooling(hpText);
        hpTextPool.HavuzuDoldur(objCountInPool);
    }


    public static void ShowsUp(Vector2 pos, float damageValue)
    {
        GameObject hpText = hpTextPool.HavuzdanObjeCek();
        hpText.GetComponent<HPTextSendPool>().fhp = hpTextPool;
        hpText.GetComponent<TextMeshPro>().DOFade(1, 0f);

        hpText.GetComponent<RectTransform>().position = pos;
        hpText.GetComponent<RectTransform>().DOMoveY(pos.y + 2f, 2f);

        hpText.GetComponent<TextMeshPro>().text = damageValue.ToString("0.##");
        hpText.GetComponent<TextMeshPro>().DOFade(0, 0.5f).SetDelay(0.95f);
    }
}
