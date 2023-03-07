using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvHolder : MonoBehaviour
{
    public Sprite[] bigTree;
    public Sprite[] littleTree;
    public Sprite[] flwrMshrm;
    public Sprite[] rock;
    public Sprite[] bush;

    public static int envAmount;    //camera noise ile sallanýnca objeler birbirinin içinden geçiyor geçmesin diye herbirinin z eksenindeki deðerini burada güncelliyoz
}
