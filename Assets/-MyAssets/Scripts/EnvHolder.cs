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

    public static int envAmount;    //camera noise ile sallan�nca objeler birbirinin i�inden ge�iyor ge�mesin diye herbirinin z eksenindeki de�erini burada g�ncelliyoz
}
