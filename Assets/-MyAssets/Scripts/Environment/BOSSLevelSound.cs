using UnityEngine;

public class BOSSLevelSound : MonoBehaviour
{
    public AudioClip trTalk, enTalk;
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

            BossFightTalk();
            Invoke(nameof(WarMusic), 3);
        }
    }

    void WarMusic()
    {
        AudioManager.instance.PlaySound("BossMusic");
    }

    void BossFightTalk()
    {
        if (PlayerPrefs.GetInt("language") == 0)
        {
            GetComponent<AudioSource>().clip = trTalk;
        }
        else
        {
            GetComponent<AudioSource>().clip = enTalk;
        }

        GetComponent<AudioSource>().PlayDelayed(5);
    }
}
