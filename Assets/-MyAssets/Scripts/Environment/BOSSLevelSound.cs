using UnityEngine;

public class BOSSLevelSound : MonoBehaviour
{
    bool firstTime;

    private void Start()
    {
        firstTime = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && firstTime)
        {
            firstTime = false;
            AudioManager.instance.PlaySound("WolfHowling");

            Invoke(nameof(WarMusic), 3);
        }
    }

    void WarMusic()
    {
        AudioManager.instance.PlaySound("BossMusic");
    }
}
