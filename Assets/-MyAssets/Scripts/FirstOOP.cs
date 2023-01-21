using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FirstOOP : MonoBehaviour
{
    /// <summary> 
    /// Yüzü çevir. (Obje, ölçek)
    /// </summary>
    public static void FlipFace(Transform objectt, float scale)
    {
        objectt.localScale = new Vector3(scale, 1, 1);
    }

    public static void Destroyy(Object obj, [DefaultValue("0.0F")] float t)
    {

    }

    /// <summary> 
    /// Bir objeye kuvvet uygula.
    /// </summary>
    /// <param name="rb2">Kuvvet uygulanacak rigidbody </param>
    /// <param name="direction">Yön </param>
    /// <param name="strong">Kuvvet </param>
    public static void Push(Rigidbody2D rb2, Vector3 direction, float strong)
    {
        rb2.velocity = strong * direction;
    }






}