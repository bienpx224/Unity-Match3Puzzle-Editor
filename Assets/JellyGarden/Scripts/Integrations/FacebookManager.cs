using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

#if FACEBOOK
using Facebook.Unity;
#endif


public class FacebookManager : MonoBehaviour
{
    private bool LoginEnable;
    public GameObject facebookButton;
    //1.3.3
    private string lastResponse = string.Empty;
    public static string userID;
    public static List<FriendData> Friends = new List<FriendData>();

    protected string LastResponse
    {
        get
        {
            return this.lastResponse;
        }

        set
        {
            this.lastResponse = value;
        }
    }

    private string status = "Ready";

    protected string Status
    {
        get
        {
            return this.status;
        }

        set
        {
            this.status = value;
        }
    }

    bool loginForSharing;
    public static FacebookManager THIS;
    bool loginOnce;
    //2.1.3

    void Awake()
    {
        THIS = this;
    }

    void OnEnable()
    {
#if PLAYFAB
		NetworkManager.OnLoginEvent += GetUserName;

#endif
    }


    void OnDisable()
    {
#if PLAYFAB
		NetworkManager.OnLoginEvent -= GetUserName;

#endif
    }

    public void AddFriend(FriendData friend)
    { //2.1.2
        FriendData friendIndex = FacebookManager.Friends.Find(delegate (FriendData bk)
        {
            return bk.userID == friend.userID;
        });
        if (friendIndex == null)
            Friends.Add(friend);
    }

    public void SetPicture(string userID, Sprite sprite)
    {//2.1.2
        FriendData friendIndex = FacebookManager.Friends.Find(delegate (FriendData bk)
        {
            return bk.userID == userID;
        });
        if (friendIndex != null)
            friendIndex.picture = sprite;
    }

#if PLAYFAB || GAMESPARKS
    public FriendData GetCurrentUserAsFriend()
    {
        FriendData friend = new FriendData()
        {
            FacebookID = NetworkManager.facebookUserID,
            userID = NetworkManager.UserID,
            picture = InitScript.profilePic
        };
        //		print ("playefab id: " + friend.PlayFabID);
        return friend;
    }
#endif

    #region FaceBook_stuff

#if FACEBOOK
    public void CallFBInit()
    {
        Debug.Log("init facebook");
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void OnInitComplete()
    {
        Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
        if (FB.IsLoggedIn)
        {//1.3
            LoggedSuccefull();//2.1.3
        }

    }

    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Is game showing? " + isGameShown);
    }

    void OnGUI()
    {
        if (LoginEnable)
        {
            CallFBLogin();
            LoginEnable = false;
        }

    }


    public void CallFBLogin()
    {
        if (!loginOnce)
        {//2.1.3
            loginOnce = true;
            Debug.Log("login");
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
        }
    }

    public void CallFBLoginForPublish()
    {
        // It is generally good behavior to split asking for read and publish
        // permissions rather than ask for them all at once.
        //
        // In your own game, consider postponing this call until the moment
        // you actually need it.
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, this.HandleResult);
    }

    public void CallFBLogout()
    {
        FB.LogOut();
        facebookButton.SetActive(true);





#if PLAYFAB || GAMESPARKS
        NetworkManager.THIS.IsLoggedIn = false;
#endif
        SceneManager.LoadScene("game");
    }

    public void Share()
    {
        if (!FB.IsLoggedIn)
        {
            loginForSharing = true;
            LoginEnable = true;
            Debug.Log("not logged, logging");
        }
        else
        {
            //2.1.4
            FB.FeedShare(
               link: new Uri("https://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + (FB.IsLoggedIn ? AccessToken.CurrentAccessToken.UserId : "guest")),
               linkName: "Jelly Garden",
               linkCaption: "I've got " + LevelManager.Score + " scores! Try to beat me!"
           //picture: "https://fbexternal-a.akamaihd.net/safe_image.php?d=AQCzlvjob906zmGv&w=128&h=128&url=https%3A%2F%2Ffbcdn-photos-h-a.akamaihd.net%2Fhphotos-ak-xtp1%2Ft39.2081-0%2F11891368_513258735497916_1832270581_n.png&cfs=1"
           );
            // var path = LevelManager.THIS.androidSharingPath;
            // if (Application.platform == RuntimePlatform.IPhonePlayer)
            //     path = LevelManager.THIS.iosSharingPath;

            // FB.FeedShare("",
            //     new Uri(path),
            //     "Juice Fresh",
            //     "I just scored " + LevelManager.Score + " points! Try to beat me!",
            //     "Juice Fresh",
            //     new Uri("http://candy-smith.com/wp-content/uploads/2017/02/Juice-Fresh_mini.png"));

        }
    }

    protected void HandleResult(IResult result)
    {
        loginOnce = false;//2.1.3
        if (result == null)
        {
            this.LastResponse = "Null Response\n";
            Debug.Log(this.LastResponse);
            return;
        }

        //     this.LastResponseTexture = null;

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            this.Status = "Error - Check log for details";
            this.LastResponse = "Error Response:\n" + result.Error;
            Debug.Log(result.Error);
        }
        else if (result.Cancelled)
        {
            this.Status = "Cancelled - Check log for details";
            this.LastResponse = "Cancelled Response:\n" + result.RawResult;
            Debug.Log(result.RawResult);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            this.Status = "Success - Check log for details";
            this.LastResponse = "Success Response:\n" + result.RawResult;
            LoggedSuccefull();//1.3
        }
        else
        {
            this.LastResponse = "Empty Response\n";
            Debug.Log(this.LastResponse);
        }
    }

    public void LoggedSuccefull()
    {//2.1.2
        PlayerPrefs.SetInt("Facebook_Logged", 1);
        PlayerPrefs.Save();

        facebookButton.SetActive(false);//1.3.3

        //Debug.Log(result.RawResult);
        userID = AccessToken.CurrentAccessToken.UserId;
        GetPicture(AccessToken.CurrentAccessToken.UserId);






#if PLAYFAB || GAMESPARKS
        NetworkManager.facebookUserID = AccessToken.CurrentAccessToken.UserId;
        NetworkManager.THIS.LoginWithFB(AccessToken.CurrentAccessToken.TokenString);
#endif
    }

    void GetUserName()
    {
        FB.API("/me?fields=first_name", HttpMethod.GET, GettingNameCallback);
    }

    private void GettingNameCallback(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error))
        {
            IDictionary dict = result.ResultDictionary as IDictionary;
            string fbname = dict["first_name"].ToString();

#if PLAYFAB || GAMESPARKS
            NetworkManager.THIS.UpdateName(fbname);
#endif

        }

    }

    IEnumerator loadPicture(string url)//2.1.4
    {
        WWW www = new WWW(url);
        yield return www;

        var texture = www.texture;

        var sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
        InitScript.profilePic = sprite;

#if PLAYFAB || GAMESPARKS
        SetPicture(NetworkManager.UserID, InitScript.profilePic);
        NetworkManager.PlayerPictureLoaded();

#endif
    }


    void GetPicture(string id)
    {
        FB.API("/" + id + "/picture?g&width=128&height=128&redirect=false", HttpMethod.GET, this.ProfilePhotoCallback);//2.1.4
    }

    private void ProfilePhotoCallback(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error))//2.1.4
        {
            var dic = result.ResultDictionary["data"] as Dictionary<string, object>;
            string url = dic.Where(i => i.Key == "url").First().Value as string;
            StartCoroutine(loadPicture(url));
        }

    }



    public void SaveScores()
    {
        int score = 10000;

        var scoreData =
            new Dictionary<string, string>() { { "score", score.ToString() } };

        IDictionary<string, string> dic =
            new Dictionary<string, string>();
        //dic.Add("stat_type", "http://samples.ogp.me/768772476466403");
        //dic.Add("object1", "{\"fb:app_id\":\"1040909022611487\",\"og:type\":\"object\",\"og:title\":\"111\"}");
        //FB.API("/me/scores", HttpMethod.POST, APICallBack, scoreData);
        //FB.API("me/objects/object1", HttpMethod.POST, APICallBack, dic);
        //"object": "{\"fb:app_id\":\"302184056577324\",\"og:type\":\"object\",\"og:url\":\"Put your own URL to the object here\",\"og:title\":\"Sample Object\",\"og:image\":\"https:\\\/\\\/s-static.ak.fbcdn.net\\\/images\\\/devsite\\\/attachment_blank.png\"}"

    }

    public void ReadScores()
    {

        FB.API("/me/objects/object", HttpMethod.GET, APICallBack);
    }

    public void GetFriendsPicture()
    {
        FB.API("me/friends?fields=picture.width(128).height(128)", HttpMethod.GET, RequestFriendsCallback); //2.1.6
    }


    private void RequestFriendsCallback(IGraphResult result)
    {
        if (!string.IsNullOrEmpty(result.RawResult))
        {
            //			Debug.Log (result.RawResult);

            var resultDictionary = result.ResultDictionary;
            if (resultDictionary.ContainsKey("data"))
            {
                var dataArray = (List<object>)resultDictionary["data"];//2.1.4
                var dic = dataArray.Select(x => x as Dictionary<string, object>).ToArray();

                foreach (var item in dic)
                {
                    string id = (string)item["id"];
                    var url = item.Where(x => x.Key == "picture").SelectMany(x => x.Value as Dictionary<string, object>).Where(x => x.Key == "data").SelectMany(x => x.Value as Dictionary<string, object>).Where(i => i.Key == "url").First().Value;
                    FriendData friend = Friends.Where(x => x.FacebookID == id).FirstOrDefault();
                    if (friend != null)
                        GetPictureByURL("" + url, friend);
                }
            }


            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.Log(result.Error);

            }
        }
    }

    public void GetPictureByURL(string url, FriendData friend)
    {
        StartCoroutine(GetPictureCor(url, friend));
    }

    IEnumerator GetPictureCor(string url, FriendData friend)
    {
        WWW www = new WWW(url);
        yield return www;
        var sprite = Sprite.Create(www.texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
        friend.picture = sprite;
        //		print ("get picture for " + url);
    }

    public void APICallBack(IGraphResult result)
    {
        Debug.Log(result);
    }

#endif
    #endregion

}

public class FriendData
{
    public string userID;
    public string FacebookID;
    public Sprite picture;
    public int level;
    public GameObject avatar;
}
