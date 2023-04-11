using System.Collections;
using UnityEngine;

public class HPTextSendPool : MonoBehaviour
{
    public ObjectPooling fhp;


    private void OnEnable()
    {
        StartCoroutine(SendPool());   
    }

    IEnumerator SendPool()
    {
        yield return new WaitForSeconds(2.1f);              //2.1 çünkü 2 sn de ancak tam yerine uleþýyor (FloatingHP scripti)
        fhp.HavuzaObjeEkle(gameObject);
    }
}
