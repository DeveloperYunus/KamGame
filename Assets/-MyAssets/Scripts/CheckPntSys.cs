using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPntSys : MonoBehaviour
{
    [SerializeField] public static Vector2 checkPnt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && checkPnt.x < gameObject.transform.position.x)
            checkPnt = gameObject.transform.position;
    }
}
