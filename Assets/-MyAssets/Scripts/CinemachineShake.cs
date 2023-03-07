using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake instance;

    CinemachineVirtualCamera cmVrtlCmr;                     //kameran�n kendisi
    CinemachineBasicMultiChannelPerlin cmBMCP;              //bilmiyorum

    float ampDefault, freDefault;                                       //cameran�n normal amplitude ve fraquency �zelliklerini tutmak i�in
    float startingInt, startingFre;                                      //fonksiyonda istenen sallanma g�c�
    float shakeTimer;
    float totalShakeTime;                                   //fonksiyonda istenen sallanma s�resi

    private void Awake()
    {
        instance = this;
        cmVrtlCmr = GetComponent<CinemachineVirtualCamera>();
        cmBMCP = cmVrtlCmr.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        ampDefault = cmBMCP.m_AmplitudeGain;
        freDefault = cmBMCP.m_FrequencyGain;
    }
    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            float a = 1 - shakeTimer * totalShakeTime;
            cmBMCP.m_AmplitudeGain = Mathf.Lerp(startingInt, ampDefault, a);
            cmBMCP.m_FrequencyGain = Mathf.Lerp(startingFre, freDefault, a);           
        }
    }

    public void ShakeCamera(float amplitude, float frequency, float time)       //genlik, geni�lik - s�kl�k, frekans
    {
        cmBMCP.m_AmplitudeGain = amplitude;
        cmBMCP.m_FrequencyGain = frequency;
        shakeTimer = time;

        startingInt = amplitude;
        startingFre = frequency;
        totalShakeTime = 1 / time;
    }
}
