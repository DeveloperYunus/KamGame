using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FirstOOP : MonoBehaviour
{
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


    /// <param name="lightt">Light2D componenti</param>
    /// <param name="diff">Iþýkdaki artýþ ve azalýþýn miktarý </param>
    public static void LightSparkling(Light2D lightt, float lMax, float lMin, float lNrml, float diff)
    {
        if (FiftyChance())
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


/*      LineCast ve Raycast sýrasýnda layerlarý yoksaymak veya hedeflemek için

var layer1 = 3;
var layer2 = 5;
var layermask1 = 1 << layer1;
var layermask2 = 1 << layer2;
var finalmask = ~(layermask1 | layermask2); // Or, (1 << layer1) | (1 << layer2)

RaycastHit2D checkWall = Physics2D.Linecast(transform.position, target.position + new Vector3(0, 0.3f, 0), finalmask);


        Yumuþak geçiþ için

currentValue = Mathf.Lerp(currentValue, targetValue, transitionSpeed);






*/