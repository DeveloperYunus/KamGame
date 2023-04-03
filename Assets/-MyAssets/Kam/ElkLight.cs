using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ElkLight : MonoBehaviour
{
    float time, lightMax, lightMin, lightNrml;                      //���k yan�p s�nmesi i�in
    Light2D lightt;

    void Start()
    {
        if (!LevelFinished.isDarkLevels)
            Destroy(gameObject);

        lightt = GetComponent<Light2D>();       //yan�p s�nen ���k ayarlar�
        lightt.enabled = true;

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.15f;
        lightMax = lightNrml + 0.15f;
    }

    void FixedUpdate()
    {
        if (time < Time.time)   //���k parlamas� i�in
        {
            time += 0.07f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.05f);
        }
    }
}
