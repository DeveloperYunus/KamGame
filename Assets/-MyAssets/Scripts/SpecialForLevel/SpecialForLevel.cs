using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpecialForLevel : MonoBehaviour
{
    public Light2D lightt;
    float time, lightMax, lightMin, lightNrml;

    bool onlyOneTime;

    private void Start()
    {
        onlyOneTime = true;

        time = 0;
        lightNrml = lightt.intensity;
        lightMin = lightNrml - 0.2f;
        lightMax = lightNrml + 0.2f;
    }
    private void Update()
    {
        if (time < Time.time)
        {
            time += 0.01f;
            FirstOOP.LightSparkling(lightt, lightMax, lightMin, lightNrml, 0.02f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && onlyOneTime)
        {
            onlyOneTime = false;
            AudioManager.instance.PlaySound("BossAppear");
        }
    }
}
