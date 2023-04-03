using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ElkLight : MonoBehaviour
{
    float time, lightMax, lightMin, lightNrml;                      //ýþýk yanýp sönmesi için
    Light2D lightt;

    void Start()
    {
        if (!LevelFinished.isDarkLevels)
            Destroy(gameObject);

        lightt = GetComponent<Light2D>();       //yanýp sönen ýþýk ayarlarý
        lightt.enabled = true;

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.15f;
        lightMax = lightNrml + 0.15f;
    }

    void FixedUpdate()
    {
        if (time < Time.time)   //ýþýk parlamasý için
        {
            time += 0.07f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.05f);
        }
    }
}
