using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSHeadIns : MonoBehaviour
{
    public GameObject bossHead;

    void Start()
    {
        Instantiate(bossHead, transform.position, Quaternion.identity);
    }
}
