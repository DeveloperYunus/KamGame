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
                        "<size=33>Maximum Seviyede (5), hasar yar�ya d��er ancak menzildeki t�m d��manlara vurur</size>\n\nHasar  ";

                    if (PlayerPrefs.GetInt("thunder") != 5)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * (PlayerPrefs.GetInt("thunder") + 1);
                    else if (PlayerPrefs.GetInt("thunder") == 4)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * 0.5f * (PlayerPrefs.GetInt("thunder") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.thunderDmg * PlayerPrefs.GetInt("thunder") * 0.5f + "</color>" + "  (Max)";
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
                        "<size=33>Maximum Seviyede (5), al�nan hasar� cana �evirir</size>\n\nS�re  ";

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
                        "<size=33>At Maximum Level (5), damage reduce to half but it hits all enemies in the range</size>\n\nDamage  ";

                    if (PlayerPrefs.GetInt("thunder") != 5)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * (PlayerPrefs.GetInt("thunder") + 1);
                    else if (PlayerPrefs.GetInt("thunder") == 4)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * 0.5f * (PlayerPrefs.GetInt("thunder") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.thunderDmg * PlayerPrefs.GetInt("thunder") * 0.5f + "</color>" + "  (Max)";
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
                        "<size=33>At Maximum Level (5), convert damage received to life</size>\n\nTime  ";

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
        int a = Random.Range(1, 10);

        if (PlayerPrefs.GetInt("language") == 0)        //tr
        {
            return a switch
            {
                1 => "\n\n      1. Asya bozk�rlar�ndaki g��ebe halklar�n koruyucusu ve yol g�stericisi Kamlar... " +
                "Bizim hikayemizde ise bu mistik ve gizemli ki�ilerden 'Y�ld�r�m' ad�ndaki ya�� bilinmez bir �stad�n m�nzevi hayat�n� anlatmakta.",

                2 => "\n\n      2. Ustas�n�n kendisine aktard��� ��retiler �����nda kendisini ve sonraki nesilleri t�ketmek i�in bekleyen ate�i, oda beklemekte.",

                3 => "\n\n      3. Ve Kam art�k fark�nda, bunu a�a�lar�n g�lgesinde, havan�n bo�uklu�unda ve hayvanlar�n g�zlerinde g�r��yor, hepsi korku ve umutsuzluk i�inde. " +
                "Sab�rla bekleyen ve kendisininde sab�rla bekledi�i ate� sonunda zuhur etti.",

                4 => "\n\n      4. A�aba hangisi daha k�t�d�r, bu tehlike ile ne zaman y�zle�ece�ini bilememek mi? Yoksa vakti geldi�inde nas�l y�zle�ece�ini bilememek mi? " +
                "Ya ustas� Raad olsa ne yapard�?",

                5 => "\n\n      5. Ya ustas�n�n anlatt��� ve bu yok olu�u �nlemek i�in defetmek gereken Ha�kaar. G��lerini ve zay�fl�klar�n� bile bilmiyordu Kam.",

                6 => "\n\n      6. Bunun gibi onlarca soru kafas�n�, bu olaylar�n sonucunun nas�l bir y�k�m olabilece�i ise ruhunu rahats�z edip duruyordu... " +
                "Kam art�k yola ��kmal�yd�.",

                7 => "\n\n      7. Yola ��kmazsa e�er kim durduracakt� bu felaketi, feda etmezse e�er kendisini, �ahs�yla beraber nice bedenleri, helak edecekti bu cehennem k�leleri",

                8 => "\n\n      8. Art�k birtek hedefi ve can�ndan ba�ka kaybedek bir m�lk� olmayan Y�ld�r�m -ki can�da Tanr�ya emanetti- " +
                "��phe ve endi�elerini gelecekte def edece�i d��manlar� gibi yere seri�ti.",

                9 => "\n\n      9. Gelin Y�ld�r�m Kam'�n bundan sonraki hikayesine bizzat beraber �ahit olal�m.",

                _ => "\n\n      Biraz soluklanay�m, ben ya�l� bir adam�m.",
            };
        }
        else     //eng
        {
            return a switch
            {
                1 => "\n\n      1. Kamlar, the protector and guide of the nomadic peoples in the steppes of Asia... " +
                "In our story, it tells the ascetic life of an unknown master named 'Y�ld�r�m' from these mystical and mysterious people.",

                2 => "\n\n      2. In the light of the teachings that his master has passed on to him, the fire that awaits him and the next generations to consume is waiting in him.",

                3 => "\n\n      3. And Kam is now aware, seeing it in the shade of trees, in the muffle of the air, and in the eyes of animals, all in fear and despair." +
                "The fire, which had been patiently waiting and which he himself had been waiting patiently for, finally appeared.",

                4 => "\n\n      4. Which is worse, not knowing when to face this danger? Or not knowing how to face it when the time comes? " +
                "What if his master was Raad, what would he do?",

                5 => "\n\n      5. Either the Ha�kaar that his master told him and that he had to fight off to prevent this destruction. Kam didn't even know their strengths and weaknesses.",

                6 => "\n\n      6. Dozens of questions like this were bothering his mind, and how the result of these events could be devastated, bothered his soul... " +
                "Kam had to hit the road now.",

                7 => "\n\n      7. If he did not set out, who would stop this catastrophe, if he did not sacrifice himself, many bodies along with his person, these hellish slaves would perish.",

                8 => "\n\n      8. Y�ld�r�m, who no longer has a single target and no property to lose but his life -which is entrusted to God- " +
                "spread his doubts and anxieties as enemies he would dispel in the future.",

                9 => "\n\n      9. Let's witness Y�ld�r�m Kam's next story together.",

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