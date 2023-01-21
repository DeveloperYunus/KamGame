using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling
{
    private GameObject prefab;
    private Stack<GameObject> objeHavuzu = new Stack<GameObject>();

    public ObjectPooling(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public void HavuzuDoldur(int miktar)
    {
        for (int i = 0; i < miktar; i++)
        {
            GameObject obje = Object.Instantiate(prefab);
            HavuzaObjeEkle(obje);
        }
        //Debug.Log(objeHavuzu.Count);
    }

    public GameObject HavuzdanObjeCek()
    {
        if (objeHavuzu.Count > 0)
        {
            GameObject obje = objeHavuzu.Pop();
            obje.SetActive(true);

            return obje;
        }

        return Object.Instantiate(prefab);
    }

    public void HavuzaObjeEkle(GameObject obje)
    {
        obje.SetActive(false);
        objeHavuzu.Push(obje);
    }
}