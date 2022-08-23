using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif





public class AnimationManager : MonoBehaviour
{
    public bool PlayOnEnable = true;
    bool WaitForPickupFriends;

    bool WaitForAksFriends;
    System.Collections.Generic.Dictionary<string, string> parameters;

    void OnEnable()
    {
        if (PlayOnEnable)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[0]);

            //if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
            //    GetComponent<SequencePlayer>().Play();
        }
        if (name == "MenuPlay")
        {
            for (int i = 1; i <= 3; i++)
            {
                transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
            }
            int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", PlayerPrefs.GetInt("OpenLevel")), 0);
            if (stars > 0 && stars <= 3)
            {
                for (int i = 1; i <= stars; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
                }

            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
                }

            }
        }

        if (name == "PrePlay")
        {
            // GameObject
        }
        if (name == "PreFailed")
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.gameOver[0]);
            transform.Find("Video").gameObject.SetActive(true);
            transform.Find("Buy").GetComponent<Button>().interactable = true;

            GetComponent<Animation>().Play();
        }

        if (name == "Settings" || name == "MenuPause")
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(true);
            else
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(false);

            if (PlayerPrefs.GetInt("Music") == 0)
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(true);
            else
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(false);

        }

        if (name == "GemsShop")
        {
            for (int i = 1; i <= 4; i++)
            {
                transform.Find("Image/Pack" + i + "/Count").GetComponent<Text>().text = "" + LevelManager.THIS.gemsProducts[i - 1].count;
                transform.Find("Image/Pack" + i + "/Buy/Price").GetComponent<Text>().text = "$" + LevelManager.THIS.gemsProducts[i - 1].price;
            }
        }
        if (name == "MenuComplete")
        {
            for (int i = 1; i <= 3; i++)
            {
                transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
            }

        }

        var videoAdsButton = transform.Find("Image/Video");
        if (videoAdsButton == null) videoAdsButton = transform.Find("Video");
        if (videoAdsButton != null )
        {
            var videoButton = videoAdsButton == null ? transform.Find("Video") : videoAdsButton;
            if (videoButton == null) return;
            if (videoButton.GetComponent<RewardedButton>() == null)
            {
                RewardedButton b = videoButton.gameObject.AddComponent<RewardedButton>();
                b.type = GetReward();
                b.SetEnabled();
            }
#if UNITY_ADS
            InitScript.Instance.rewardedVideoZone = "rewardedVideo";

            if (!InitScript.Instance.enableUnityAds || !InitScript.Instance.GetRewardedUnityAdsReady())
            {
                videoButton.gameObject.SetActive(false);
            }
#elif GOOGLE_MOBILE_ADS//2.2
			bool stillShow = true;
#if UNITY_ADS
        stillShow = !InitScript.Instance.GetRewardedUnityAdsReady ();
#endif
			if (!InitScript.Instance.enableGoogleMobileAds || !RewAdmobManager.THIS.IsRewardedAdIsLoaded() || !stillShow)
				videoAdsButton.gameObject.SetActive(false);
#else
			videoAdsButton.gameObject.SetActive(false);
#endif
        }

    }

    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (name == "MenuPlay" || name == "Settings" || name == "BoostInfo" || name == "GemsShop" || name == "LiveShop" || name == "BoostShop" || name == "Reward")
                CloseMenu();
        }
    }

    public void ShowAds()
    {
        InitScript.Instance.currentReward = GetReward();
        InitScript.Instance.ShowRewardedAds();
        CloseMenu();
    }

    private RewardedAdsType GetReward()
    {
        switch (name)
        {
            case "GemsShop":
                return RewardedAdsType.GetGems;
            case "LiveShop":
                return RewardedAdsType.GetLifes;
            case "PreFailed":
                return RewardedAdsType.GetGoOn;
        }
        return RewardedAdsType.GetGoOn;
    }

    public void GoRate()
    {
#if UNITY_ANDROID//2.1.5 ios rate link
        Application.OpenURL(InitScript.Instance.RateURL);
#elif UNITY_IOS
		Application.OpenURL(InitScript.Instance.RateURLIOS);
#endif
        PlayerPrefs.SetInt("Rated", 1);
        PlayerPrefs.Save();
    }

    void OnDisable()
    {
        if (transform.Find("Image/Video") != null)
        {
            transform.Find("Image/Video").gameObject.SetActive(true);
        }

        //if( PlayOnEnable )
        //{
        //    if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
        //        GetComponent<SequencePlayer>().sequenceArray[0].Play
        //}
    }

    public void OnFinished()
    {
        if (name == "MenuComplete")
        {
            StartCoroutine(MenuComplete());
            StartCoroutine(MenuCompleteScoring());
        }
        if (name == "MenuPlay")
        {
            //            InitScript.Instance.currentTarget = InitScript.Instance.targets[PlayerPrefs.GetInt( "OpenLevel" )];
            transform.Find("Image/Boost1").GetComponent<BoostIcon>().InitBoost();
            transform.Find("Image/Boost2").GetComponent<BoostIcon>().InitBoost();
            transform.Find("Image/Boost3").GetComponent<BoostIcon>().InitBoost();

        }
        if (name == "MenuPause")
        {
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                LevelManager.THIS.gameStatus = GameState.Pause;
        }

        if (name == "PrePlay")
        {
            CloseMenu();
            LevelManager.THIS.gameStatus = GameState.WaitForPopup;

        }
        if (name == "PreFailed")
        {
            if (LevelManager.THIS.Limit <= 0)
                LevelManager.THIS.gameStatus = GameState.GameOver;
            transform.Find("Video").gameObject.SetActive(false);

            CloseMenu();

        }

        if (name.Contains("gratzWord"))
            gameObject.SetActive(false);
        if (name == "NoMoreMatches")
            gameObject.SetActive(false);
        if (name == "CompleteLabel")
            gameObject.SetActive(false);

    }

    void TargetCheck(bool check, int n = 1)
    {
        Transform TargetCheck = transform.Find("Image/TargetCheck" + n);
        Transform TargetUnCheck = transform.Find("Image/TargetUnCheck" + n);
        TargetCheck.gameObject.SetActive(check);
        TargetUnCheck.gameObject.SetActive(!check);
    }

    public void WaitForGiveUp()
    {
        if (name == "PreFailed")
        {
            GetComponent<Animation>()["bannerFailed"].speed = 0;
#if UNITY_ADS

            if (InitScript.Instance.enableUnityAds)
            {

                if (InitScript.Instance.GetRewardedUnityAdsReady())
                {
                    transform.Find("Video").gameObject.SetActive(true);
                }
            }
#elif GOOGLE_MOBILE_ADS//2.2
			bool stillShow = true;
#if UNITY_ADS
        stillShow = !InitScript.Instance.GetRewardedUnityAdsReady ();
#endif
			if (InitScript.Instance.enableGoogleMobileAds && stillShow)
			{
				if (RewAdmobManager.THIS.IsRewardedAdIsLoaded())
				{
					transform.Find("Video").gameObject.SetActive(true);
				}
			}

#endif
        }
    }

    IEnumerator MenuComplete()
    {
        for (int i = 1; i <= LevelManager.Instance.stars; i++)
        {
            //  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoringStar );
            transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.star[i - 1]);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator MenuCompleteScoring()
    {
        Text scores = transform.Find("Image").Find("Score").GetComponent<Text>();
        for (int i = 0; i <= LevelManager.Score; i += 500)
        {
            scores.text = "" + i;
            // SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
            yield return new WaitForSeconds(0.00001f);
        }
        scores.text = "" + LevelManager.Score;
    }

    public void Info()
    {
        GameObject.Find("CanvasGlobal").transform.Find("Tutorial").gameObject.SetActive(true);
        CloseMenu();
    }



    public void PlaySoundButton()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

    }

    public IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void CloseMenu()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (gameObject.name == "MenuPreGameOver")
        {
            ShowGameOver();
        }
        if (gameObject.name == "MenuComplete")
        {
            LevelManager.THIS.gameStatus = GameState.Map;
            //2.1
            PlayerPrefs.SetInt("OpenLevel", LevelManager.THIS.currentLevel + 1);
            LevelManager.THIS.LoadLevel();//2.1.5  Switch target
            if (LevelsMap._instance.GetMapLevels().Count >= LevelManager.THIS.currentLevel)
                GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
        }
        if (gameObject.name == "MenuFailed")
        {
            LevelManager.THIS.gameStatus = GameState.Map;
        }

        if (SceneManager.GetActiveScene().name == "game")
        {
            if (LevelManager.Instance.gameStatus == GameState.Pause)
            {
                LevelManager.Instance.gameStatus = GameState.WaitAfterClose;

            }
        }
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[1]);

        gameObject.SetActive(false);
    }

    public void SwishSound()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[1]);

    }

    public void ShowInfo()
    {
        GameObject.Find("CanvasGlobal").transform.Find("BoostInfo").gameObject.SetActive(true);

    }

    public void Play()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (gameObject.name == "MenuPreGameOver")
        {
            if (InitScript.Gems >= 12)
            {
                InitScript.Instance.SpendGems(12);
                //                LevelData.LimitAmount += 12;
                LevelManager.Instance.gameStatus = GameState.WaitAfterClose;
                gameObject.SetActive(false);

            }
            else
            {
                BuyGems();
            }
        }
        else if (gameObject.name == "MenuFailed")
        {
            LevelManager.Instance.gameStatus = GameState.Map;
        }
        else if (gameObject.name == "MenuPlay")
        {
            if (InitScript.lifes > 0)
            {
                InitScript.Instance.SpendLife(1);
                LevelManager.THIS.gameStatus = GameState.PrepareGame;
                CloseMenu();
                //Application.LoadLevel( "game" );
            }
            else
            {
                BuyLifeShop();
            }

        }
        else if (gameObject.name == "MenuPause")
        {
            CloseMenu();
            LevelManager.Instance.gameStatus = GameState.Playing;
        }
    }

    public void PlayTutorial()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        LevelManager.Instance.gameStatus = GameState.Playing;
        //    mainscript.Instance.dropDownTime = Time.time + 0.5f;
        //        CloseMenu();
    }

    public void BackToMap()
    {
        Time.timeScale = 1;
        LevelManager.THIS.gameStatus = GameState.GameOver;
        CloseMenu();
    }

    public void Next()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        CloseMenu();
    }

    public void Again()//2.2.2
    {
        GameObject gm = new GameObject();
        gm.AddComponent<RestartLevel>();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BuyGems()
    {

        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
    }

    public void Buy(GameObject pack)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (pack.name == "Pack1")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[0]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }

        if (pack.name == "Pack2")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[1]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }
        if (pack.name == "Pack3")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[2]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }
        if (pack.name == "Pack4")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[3]);
#else
            Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//2.1.6
#endif
        }
        CloseMenu();

    }

    public void BuyLifeShop()
    {

        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (InitScript.lifes < InitScript.Instance.CapOfLife)
            GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.SetActive(true);

    }

    public void BuyLife(GameObject button)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text))
        {
            InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text));
            InitScript.Instance.RestoreLifes();
            CloseMenu();
        }
        else
        {
            GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
        }

    }

    public void BuyFailed(GameObject button)
    {
        if (GetComponent<Animation>()["bannerFailed"].speed == 0)
        {
            if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text))
            {
                InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text));
                button.GetComponent<Button>().interactable = false;
                GoOnFailed();
            }
            else
            {
                GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
            }
        }
    }

    public void GoOnFailed()
    {
        if (LevelManager.THIS.limitType == LIMIT.MOVES)
            LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedMoves;
        else
            LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedSecs;
        GetComponent<Animation>()["bannerFailed"].speed = 1;
        LevelManager.THIS.gameStatus = GameState.Playing;
    }

    public void GiveUp()
    {
        GetComponent<Animation>()["bannerFailed"].speed = 1;
    }

    void ShowGameOver()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.gameOver[1]);

        GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
        gameObject.SetActive(false);

    }

    #region boosts

    public void BuyBoost(BoostType boostType, int price, int count)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        if (InitScript.Gems >= price)
        {
            InitScript.Instance.SpendGems(price);
            InitScript.Instance.BuyBoost(boostType, price, count);
            //InitScript.Instance.SpendBoost(boostType);
            CloseMenu();
        }
        else
        {
            BuyGems();
        }
    }

    #endregion

    public void SoundOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 0;
            InitScript.sound = false;

            Off.SetActive(true);
        }
        else
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 1;
            InitScript.sound = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt("Sound", (int)SoundBase.Instance.GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

    public void MusicOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
            InitScript.music = false;

            Off.SetActive(true);
        }
        else
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
            InitScript.music = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt("Music", (int)GameObject.Find("Music").GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

}
