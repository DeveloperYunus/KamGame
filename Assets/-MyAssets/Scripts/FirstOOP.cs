using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    public static bool FiftyChance()
    {
        if (Random.Range(0, 2) == 0) return false;
        else                         return true;
    }

    /// <param name="lightt">Kuvvet uygulanacak rigidbody </param>
    /// <param name="diff">Iþýkdaki artýþ ve azalýþýn miktarý </param>

    public static void LightSparkling(Light2D lightt, float lMax, float lMin, float lNrml, float diff)
    {
        if (FirstOOP.FiftyChance())
        {
            lightt.intensity += diff;
        }
        else
        {
            lightt.intensity -= diff;
        }

        if (lightt.intensity > lMax || lightt.intensity < lMin)
            lightt.intensity = lNrml;
    }
}