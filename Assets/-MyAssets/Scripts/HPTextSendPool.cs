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
        yield return new WaitForSeconds(2.1f);              //2.1 ��nk� 2 sn de ancak tam yerine ule��yor (FloatingHP scripti)
        fhp.HavuzaObjeEkle(gameObject);
    }
}
