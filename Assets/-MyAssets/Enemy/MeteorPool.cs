using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorPool : MonoBehaviour
{
    public GameObject meteor;
    public int objCountInPool;

    public static ObjectPooling meteorPool;

    void Start()
    {
        meteorPool = new ObjectPooling(meteor, transform);
        meteorPool.HavuzuDoldur(objCountInPool);
    }

    public static GameObject CallMeteor()
    {
        GameObject met = meteorPool.HavuzdanObjeCek();
        met.GetComponent<Meteor>().mP = meteorPool;
        return met;
    }
}
