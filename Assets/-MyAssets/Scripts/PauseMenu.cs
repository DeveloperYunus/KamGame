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

            if (!isPaused)      //þu anda kapalý o zaman açýlsýn
            {
                gameObject.GetComponent<RectTransform>().DOScale(1f, 0f);
                gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.2f).SetUpdate(true);
            }
            else      //þu anda açýk o zaman kapansýn
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
        else                    //demekki bir skill var açýklamasýný yaz
            skillDesc.text = ReturnSkillTxt(skillID);
    }
    public string ReturnSkillTxt(int skillID)       //<color=#53ecec>a sd</color>
    {
        if (PlayerPrefs.GetInt("language") == 0)
        {
            switch (skillID)//tr
            {
                case 0:
                    returnTxt = "<color=#53ecec>ÞÝMÞEK TOPU</color> (Sol Fare Tuþu)\n\n   Bu temel büyü fare simgesinin olduðu yere doðru ateþlenir\n\nHasar  ";

                    if (PlayerPrefs.GetInt("bolt") != 5)
                        returnTxt += ka.boltDmg * PlayerPrefs.GetInt("bolt") + "  >>  " + ka.boltDmg * (PlayerPrefs.GetInt("bolt") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.boltDmg * PlayerPrefs.GetInt("bolt") + "</color>" + "  (Max)";
                    break;

                case 1:
                    returnTxt = "<color=#53ecec>YILDIRIM</color> (E Tuþu)\n\n   En yakýndaki cehennem kölesinin kafasýna alan hasarý vuran bir yýldýrým gönderir\n\n" +
                        "<size=33>Maximum Seviyede (5), hasar yarýya düþer ancak menzildeki tüm düþmanlara vurur</size>\n\nHasar  ";

                    if (PlayerPrefs.GetInt("thunder") != 5)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * (PlayerPrefs.GetInt("thunder") + 1);
                    else if (PlayerPrefs.GetInt("thunder") == 4)
                        returnTxt += ka.thunderDmg * PlayerPrefs.GetInt("thunder") + "  >>  " + ka.thunderDmg * 0.5f * (PlayerPrefs.GetInt("thunder") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.thunderDmg * PlayerPrefs.GetInt("thunder") * 0.5f + "</color>" + "  (Max)";
                    break;

                case 2:
                    returnTxt = "<color=#53ecec>TUZAK</color> (S Tuþu)\n\n   Üzerine düþman geldiðinde tetiklenen bir elektrik kapaný oluþturur\n\n" +
                        "<size=33>Maximum Seviyede (5), düþmanlarý 3 sn yere sabitler</size>\n\nHasar  ";

                    if (PlayerPrefs.GetInt("trap") != 5)
                        returnTxt += ka.trapDmg * PlayerPrefs.GetInt("trap") + "  >>  " + ka.trapDmg * (PlayerPrefs.GetInt("trap") + 1);
                    else
                        returnTxt += "<color=#53ecec>" + ka.trapDmg * PlayerPrefs.GetInt("trap") + "</color>" + "  (Max)";
                    break;

                case 3:
                    returnTxt = "<color=#53ecec>BARÝYER</color> (Q Tuþu)\n\n    Kam kendisini enerji alaný ile kuþatýr ve hasar göremez hale gelir\n\n" +
                        "<size=33>Maximum Seviyede (5), alýnan hasarý cana çevirir</size>\n\nSüre  ";

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
    public string RegenTips()//sonradan bu kýsým çeþitlendirilecek
    {
        int a = Random.Range(1, 10);

        if (PlayerPrefs.GetInt("language") == 0)        //tr
        {
            return a switch
            {
                1 => "\n\n      1. Asya bozkýrlarýndaki göçebe halklarýn koruyucusu ve yol göstericisi Kamlar... " +
                "Bizim hikayemizde ise bu mistik ve gizemli kiþilerden 'Yýldýrým' adýndaki yaþý bilinmez bir üstadýn münzevi hayatýný anlatmakta.",

                2 => "\n\n      2. Ustasýnýn kendisine aktardýðý öðretiler ýþýðýnda kendisini ve sonraki nesilleri tüketmek için bekleyen ateþi, oda beklemekte.",

                3 => "\n\n      3. Ve Kam artýk farkýnda, bunu aðaçlarýn gölgesinde, havanýn boðukluðunda ve hayvanlarýn gözlerinde görüüyor, hepsi korku ve umutsuzluk içinde. " +
                "Sabýrla bekleyen ve kendisininde sabýrla beklediði ateþ sonunda zuhur etti.",

                4 => "\n\n      4. Açaba hangisi daha kötüdür, bu tehlike ile ne zaman yüzleþeceðini bilememek mi? Yoksa vakti geldiðinde nasýl yüzleþeceðini bilememek mi? " +
                "Ya ustasý Raad olsa ne yapardý?",

                5 => "\n\n      5. Ya ustasýnýn anlattýðý ve bu yok oluþu önlemek için defetmek gereken Haçkaar. Güçlerini ve zayýflýklarýný bile bilmiyordu Kam.",

                6 => "\n\n      6. Bunun gibi onlarca soru kafasýný, bu olaylarýn sonucunun nasýl bir yýkým olabileceði ise ruhunu rahatsýz edip duruyordu... " +
                "Kam artýk yola çýkmalýydý.",

                7 => "\n\n      7. Yola çýkmazsa eðer kim durduracaktý bu felaketi, feda etmezse eðer kendisini, þahsýyla beraber nice bedenleri, helak edecekti bu cehennem köleleri",

                8 => "\n\n      8. Artýk birtek hedefi ve canýndan baþka kaybedek bir mülkü olmayan Yýldýrým -ki canýda Tanrýya emanetti- " +
                "þüphe ve endiþelerini gelecekte def edeceði düþmanlarý gibi yere seriþti.",

                9 => "\n\n      9. Gelin Yýldýrým Kam'ýn bundan sonraki hikayesine bizzat beraber þahit olalým.",

                _ => "\n\n      Biraz soluklanayým, ben yaþlý bir adamým.",
            };
        }
        else     //eng
        {
            return a switch
            {
                1 => "\n\n      1. Kamlar, the protector and guide of the nomadic peoples in the steppes of Asia... " +
                "In our story, it tells the ascetic life of an unknown master named 'Yýldýrým' from these mystical and mysterious people.",

                2 => "\n\n      2. In the light of the teachings that his master has passed on to him, the fire that awaits him and the next generations to consume is waiting in him.",

                3 => "\n\n      3. And Kam is now aware, seeing it in the shade of trees, in the muffle of the air, and in the eyes of animals, all in fear and despair." +
                "The fire, which had been patiently waiting and which he himself had been waiting patiently for, finally appeared.",

                4 => "\n\n      4. Which is worse, not knowing when to face this danger? Or not knowing how to face it when the time comes? " +
                "What if his master was Raad, what would he do?",

                5 => "\n\n      5. Either the Haçkaar that his master told him and that he had to fight off to prevent this destruction. Kam didn't even know their strengths and weaknesses.",

                6 => "\n\n      6. Dozens of questions like this were bothering his mind, and how the result of these events could be devastated, bothered his soul... " +
                "Kam had to hit the road now.",

                7 => "\n\n      7. If he did not set out, who would stop this catastrophe, if he did not sacrifice himself, many bodies along with his person, these hellish slaves would perish.",

                8 => "\n\n      8. Yýldýrým, who no longer has a single target and no property to lose but his life -which is entrusted to God- " +
                "spread his doubts and anxieties as enemies he would dispel in the future.",

                9 => "\n\n      9. Let's witness Yýldýrým Kam's next story together.",

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
                upgradeTxt.text = "Yükselt";
                quitTxt.text = "Çýkýþ";
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
    public void QuitToMenu()                             //oyun sahnesinden ana menüye gider
    {

    }
}