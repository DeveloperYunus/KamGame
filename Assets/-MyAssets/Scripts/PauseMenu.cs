using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Drawing;

public class PauseMenu : MonoBehaviour
{
    public TextMeshProUGUI[] skillsLevel;
    public TextMeshProUGUI skillDesc, skillPoint, upgradeTxt, quitTxt, restartTxt;

    KamAttack ka;
    string skillHolder, returnTxt;                                            //skill ID holder
    int skillID;
    bool isPaused;

    private void Start()
    {
        isPaused = false;
        skillHolder = null;

        ka = GameObject.Find("Kam").GetComponent<KamAttack>();

        gameObject.GetComponent<RectTransform>().DOScale(0f, 0f);
        gameObject.GetComponent<CanvasGroup>().DOFade(0, 0f);

        RegenTexts();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) 
        {
            gameObject.GetComponent<RectTransform>().DOKill();
            gameObject.GetComponent<CanvasGroup>().DOKill();

            if (!isPaused)      //�u anda kapal� o zaman a��ls�n
            {
                gameObject.GetComponent<RectTransform>().DOScale(1f, 0f);
                gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.2f).SetUpdate(true);
            }
            else      //�u anda a��k o zaman kapans�n
            {
                gameObject.GetComponent<RectTransform>().DOScale(0f, 0f).SetDelay(1f).SetUpdate(true);
                gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            }

            isPaused = !isPaused;

            if (isPaused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }
    }

    public void SelectSkill(string skill)                 //0 = bolt, 1 = thunder, 2 = trap, 3 = barrier
    {
        skillHolder = skill;

        switch (skill)
        {
            case "bolt":
                skillID = 0;
                break;

            case "thunder":
                skillID = 1;
                break;

            case "trap":
                skillID = 2;
                break;

            case "barrier":
                skillID = 3;
                break;
        }

        RegenTexts(skillID);
    }
    public void UpgradeSkill()
    {
        if (PlayerPrefs.GetInt("skillPoint") > 0 && PlayerPrefs.GetInt(skillHolder) < 5)
        {
            PlayerPrefs.SetInt(skillHolder, PlayerPrefs.GetInt(skillHolder) + 1);
            PlayerPrefs.SetInt("skillPoint", PlayerPrefs.GetInt("skillPoint") - 1);

            RegenTexts(skillID);
        }
    }
    public void RegenTexts(int skillID = -1)
    {
        skillsLevel[0].text = PlayerPrefs.GetInt("bolt").ToString();
        skillsLevel[1].text = PlayerPrefs.GetInt("thunder").ToString();
        skillsLevel[2].text = PlayerPrefs.GetInt("trap").ToString();
        skillsLevel[3].text = PlayerPrefs.GetInt("barrier").ToString();

        skillPoint.text = PlayerPrefs.GetInt("skillPoint").ToString();

        if (skillID == -1)      //skill yok tavsiye ver veya hikaye anlat
            skillDesc.text = RegenTips();
        else                    //demekki bir skill var a��klamas�n� yaz
            skillDesc.text = ReturnSkillTxt(skillID);
    }
    public string ReturnSkillTxt(int skillID)       //<color=#53ecec>a sd</color>
    {
        if (PlayerPrefs.GetInt("language") == 0)
        {
            switch (skillID)//tr
            {
                case 0:
                    returnTxt = "<color=#53ecec>��M�EK TOPU</color> (Sol Fare Tu�u)\n\n   Bu temel b�y� fare simgesinin oldu�u yere do�ru ate�lenir\n\nHasar  ";

                    if (PlayerPrefs.GetInt("bolt") != 5)
                        returnTxt += ka.boltDmg * PlayerPrefs.GetInt("bolt") + "  >>  " + ka.boltDmg * (PlayerPrefs.GetInt("bolt") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.boltDmg * PlayerPrefs.GetInt("bolt") + "</color>" + "  (Max)";
                    break;

                case 1:
                    returnTxt = "<color=#53ecec>YILDIRIM</color> (E Tu�u)\n\n   En yak�ndaki cehennem k�lesinin kafas�na alan hasar� vuran bir y�ld�r�m g�nderir\n\n" +
                        "<size=33>Maximum Seviyede (5), menzildeki t�m d��manlara vurur</size>\n\nHasar  ";

                    if (PlayerPrefs.GetInt("thunder") != 5)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * (PlayerPrefs.GetInt("thunder") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "</color>" + "  (Max)";
                    break;

                case 2:
                    returnTxt = "<color=#53ecec>TUZAK</color> (S Tu�u)\n\n   �zerine d��man geldi�inde tetiklenen bir elektrik kapan� olu�turur\n\n" +
                        "<size=33>Maximum Seviyede (5), d��manlar� 3 sn yere sabitler</size>\n\nHasar  ";

                    if (PlayerPrefs.GetInt("trap") != 5)
                        returnTxt += ka.trapDmg * PlayerPrefs.GetInt("trap") + "  >>  " + ka.trapDmg * (PlayerPrefs.GetInt("trap") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.trapDmg * PlayerPrefs.GetInt("trap") + "</color>" + "  (Max)";
                    break;

                case 3:
                    returnTxt = "<color=#53ecec>BAR�YER</color> (Q Tu�u)\n\n    Kam kendisini enerji alan� ile ku�at�r ve hasar g�remez hale gelir\n\n" +
                        "<size=33>Maximum Seviyede (5) al�nan hasar� cana �evirir</size>\n\nS�re  ";

                    if (PlayerPrefs.GetInt("barrier") != 5)
                        returnTxt += ka.barrierDuration * PlayerPrefs.GetInt("barrier") + "  >>  " + ka.barrierDuration * (PlayerPrefs.GetInt("barrier") + 1) + " saniye  (Max)";
                    else
                        returnTxt += "<color=#53ecec>" + (ka.barrierDuration * PlayerPrefs.GetInt("barrier")) + "</color>" + " saniye  (Max)"; 
                    break;
            }
        }
        else
        {
            switch (skillID)//en
            {
                case 0:
                    returnTxt = "<color=#53ecec>Bolt</color> (Left Mouse Btn)\n\n     This basic spell is fired towards the place where the mouse cursor is.\n\nDamage   ";
                        
                    if (PlayerPrefs.GetInt("bolt") != 5)
                        returnTxt += ka.boltDmg * PlayerPrefs.GetInt("bolt") + "  >>  " + ka.boltDmg * (PlayerPrefs.GetInt("bolt") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.boltDmg * PlayerPrefs.GetInt("bolt") + "</color>" + "  (Max)";
                    break;

                case 1:
                    returnTxt = "<color=#53ecec>Lightning</color> (E Key)\n\n    Kam sends a lightning bolt to the head of the nearest hell slave that hits field damage.\n\n" +
                        "<size=33>At Maximum Level (5), it hits all enemies in the range</size>\n\nDamage  ";

                    if (PlayerPrefs.GetInt("thunder") != 5)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * (PlayerPrefs.GetInt("thunder") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "</color>" + "  (Max)";
                    break;

                case 2:
                    returnTxt = "<color=#53ecec>Electrap</color> (S Key)\n\n    It creates an electric trap that is triggered when the enemy comes at it.\n\n" +
                        "<size=33>At Maximum Level (5), it pins enemies to the ground for 3 seconds</size>\n\nDamage  ";

                    if (PlayerPrefs.GetInt("trap") != 5)
                        returnTxt += ka.trapDmg * PlayerPrefs.GetInt("trap") + "  >>  " + ka.trapDmg * (PlayerPrefs.GetInt("trap") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.trapDmg * PlayerPrefs.GetInt("trap") + "</color>" + "  (Max)";
                    break;

                case 3:
                    returnTxt = "<color=#53ecec>Barrier</color> (Q Key)\n\n     Kam surrounds itself with the energy field and becomes undamaged.\n\n" +
                        "<size=33>At Maximum Level (5) convert damage received to life</size>\n\nTime  ";

                    if (PlayerPrefs.GetInt("barrier") != 5)
                        returnTxt += ka.barrierDuration * PlayerPrefs.GetInt("barrier") + "  >>  " + ka.barrierDuration * (PlayerPrefs.GetInt("barrier") + 1) + " second  (Max)";
                    else
                        returnTxt += "<color=#53ecec>" + (ka.barrierDuration * PlayerPrefs.GetInt("barrier")) + "</color>" + " second  (Max)";
                    break;
            }
        }

        return returnTxt;
    }
    public string RegenTips()//sonradan bu k�s�m �e�itlendirilecek
    {
        int a = Random.Range(1, 16);

        if (PlayerPrefs.GetInt("language") == 0)        //tr
        {
            return a switch
            {
                1 => "\n\n      Bir varm�� ormanda ya�ayan bir adam varm�� ve herkese yard�m eder ama ortal�klarda g�z�kmezmi�.",
                2 => "",
                3 => "",
                4 => "",
                5 => "",
                6 => "",
                7 => "",
                8 => "",
                9 => "",
                10 => "",
                11 => "",
                _ => "\n\n      Biraz soluklanay�m, ben ya�l� bir adam�m.",
            };
        }
        else     //eng
        {
            return a switch
            {
                1 => "\n\n      Bir varm�� ormanda ya�ayan bir adam varm�� ve herkese yard�m eder ama ortal�klarda g�z�kmezmi�.",
                2 => "",
                3 => "",
                4 => "",
                5 => "",
                6 => "",
                7 => "",
                8 => "",
                9 => "",
                10 => "",
                11 => "",
                _ => "\n\n      Let me take a breather, I'm an old man.",
            };
        }
    }


    public void SelectLanguage(int index)                //0 = Turkish, 1 = English, ...
    {
        PlayerPrefs.SetInt("language", index);

        switch (index)
        {
            case 0:
                upgradeTxt.text = "Y�kselt";
                quitTxt.text = "��k��";
                restartTxt.text = "Yenile";
                break;

            case 1:
                upgradeTxt.text = "Upgrade";
                quitTxt.text = "Quit";
                restartTxt.text = "Restart";
                break;
        }

        RegenTexts();
    }
    public void RestartLevel()
    {
        skillHolder = null;
        PlayerPrefs.SetInt("bolt",1);
        PlayerPrefs.SetInt("thunder",0);
        PlayerPrefs.SetInt("trap",0);
        PlayerPrefs.SetInt("barrier", 0);
        PlayerPrefs.SetInt("skillPoint", 10);

        RegenTexts();
    }
    public void QuitToMenu()                             //oyun sahnesinden ana men�ye gider
    {

    }
}