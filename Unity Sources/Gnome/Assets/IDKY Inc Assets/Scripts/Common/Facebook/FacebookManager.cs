// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

using Facebook;
using Facebook.MiniJSON;

using Idky;

using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    #region Fields

    public int DaysToKeepProfilePicture = 7;

    public List<UserHighScore> HighScores;

    public bool IsLoggedIn;

    public UserHighScore MyHighScore;

    private string profilePicturePath;

    private FacebookAppRequest requestBeingDeleted;

    #endregion

    #region Public Events

    public event EventHandler<FacebookAppRequestDeletedEventArgs> FacebookAppRequestDeletedEvent;

    public event EventHandler<FacebookAppRequestEventArgs> FacebookAppRequestEvent;

    public event EventHandler<FacebookDeepLinkResultEventArgs> FacebookDeepLinkResultEvent;

    public event EventHandler<FacebookFeedCompletedEventArgs> FacebookFeedCompletedEvent;

    public event EventHandler<EventArgs> FacebookInitializedEvent;

    public event EventHandler<FacebookLoggedInEventArgs> FacebookLoggedInEvent;

    public event EventHandler<FacebookProfilePictureEventArgs> FacebookProfilePictureRetreivedEvent;

    public event EventHandler<FacebookProfileEventArgs> FacebookProfileRetreivedEvent;

    #endregion

    #region Public Properties

    public static FacebookManager Instance { get; private set; }

    public List<FacebookAppRequest> AppRequests { get; private set; }

    public bool IsInitialized { get; private set; }

    public FacebookProfile Profile { get; private set; }

    public Texture2D ProfilePicture { get; private set; }

    #endregion

    #region Public Methods and Operators

    public bool DeleteAppRequest(FacebookAppRequest request)
    {
        if (this.requestBeingDeleted != null)
        {
            return false;
        }
        else
        {
            this.requestBeingDeleted = request;
            FB.API(string.Format(@"/{0}", request.RequestId), HttpMethod.DELETE, this.OnDeleteRequestCompleted);
        }

        return true;
    }

    [ContextMenu("Get App Requests")]
    public void GetAppRequests()
    {
        FB.API(@"/me/apprequests", HttpMethod.GET, this.OnGetAppRequestCompleted);
    }

    [ContextMenu("Get Deep Link")]
    public void GetDeepLink()
    {
        FB.GetDeepLink(this.OnGetDeepLinkCompleted);
    }

    [ContextMenu("Get High Scores")]
    public void GetHighScores()
    {
        string scoresQuery = string.Format("/{0}/scores", FB.AppId);
        if (this.IsLoggedIn)
        {
            FB.API(
                scoresQuery,
                HttpMethod.GET,
                result =>
                    {
                        if (result.Error != null)
                        {
                            FbDebug.Error(result.Error);

                            return;
                        }

                        FbDebug.Log("Result: " + result.Text);
                        Debug.Log("Facebook get scores result: " + result.Text);

                        this.HighScores = this.DeserializeHighScores(result.Text);

                        foreach (UserHighScore userHighScore in this.HighScores)
                        {
                            Debug.Log("Facebook high scores result: " + userHighScore.ToString());
                            this.GetUserHighScoreProfilePicture(userHighScore, 128, 128);

                            if (userHighScore.UserId == FB.UserId)
                            {
                                this.MyHighScore = userHighScore;
                            }
                        }
                    });
        }
    }

    [ContextMenu("Get Profile")]
    public void GetProfile()
    {
        if (this.IsLoggedIn)
        {
            FB.API(@"/me?fields=id,first_name,last_name", HttpMethod.GET, this.OnGetProfileCompleted);
        }
    }

    public void GetProfilePicture(int? width = null, int? height = null, string type = null)
    {
        if (File.Exists(this.profilePicturePath)
            && DateTime.Now - File.GetLastWriteTime(this.profilePicturePath) < TimeSpan.FromDays(this.DaysToKeepProfilePicture))
        {
            this.ProfilePicture = TextureIo.ReadTextureFromFile(
                this.profilePicturePath, width ?? this.ProfilePicture.width, height ?? this.ProfilePicture.height);
            this.NotifyProfilePictureRetreived(this.ProfilePicture);
        }
        else
        {
            string url = this.GetPictureURL("me", width, height, type);
            FB.API(url, HttpMethod.GET, this.OnGetProfilePictureCompleted);
        }
    }

    public void GetUserHighScoreProfilePicture(UserHighScore userHighScore, int? width = null, int? height = null, string type = null)
    {
        string path = Path.Combine(Application.persistentDataPath, userHighScore.UserId + "_ProfilePicture.png");

        if (File.Exists(path) && DateTime.Now - File.GetLastWriteTime(path) < TimeSpan.FromDays(this.DaysToKeepProfilePicture))
        {
            userHighScore.ProfilePicture = TextureIo.ReadTextureFromFile(
                path, width ?? userHighScore.ProfilePicture.width, height ?? userHighScore.ProfilePicture.height);
        }
        else
        {
            string url = this.GetPictureURL(userHighScore.UserId, width, height, type);
            FB.API(
                url,
                HttpMethod.GET,
                result =>
                    {
                        if (result.Error != null)
                        {
                            FbDebug.Error(result.Error);

                            return;
                        }

                        userHighScore.ProfilePicture = result.Texture;
                        TextureIo.SaveTextureToFile(path, userHighScore.ProfilePicture);
                    });
        }
    }

    public void Initialize()
    {
        FB.Init(this.OnSetInit, this.OnHideUnity);
    }

    public void Invite(string message, string title, string[] recipients = null, string data = "invite")
    {
        FB.AppRequest(message: message, to: recipients, data: data, title: title, callback: this.OnInviteCompleted);
    }

    public void LogIn(string scope = "")
    {
        FB.Login(scope, this.OnLoginCompleted);
    }

    public void PostHighScore(uint score)
    {
        if (this.IsLoggedIn)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query["score"] = score.ToString();

            Debug.Log("High Score: " + query["score"]);
            FB.API(
                @"/me/scores",
                HttpMethod.POST,
                result =>
                    {
                        if (result.Error != null)
                        {
                            FbDebug.Log("Result: " + result.Error);
                            Debug.LogError("Facebook post score result: " + result.Error);
                        }

                    },
                query);
        }
    }

    public void PostOnFeed(
        string toId = "",
        string link = "",
        string linkName = "",
        string linkCaption = "",
        string linkDescription = "",
        string picture = "",
        string mediaSource = "",
        string actionName = "",
        string actionLink = "",
        string reference = "",
        Dictionary<string, string[]> properties = null)
    {
        if (this.IsLoggedIn)
        {
            FB.Feed(
                toId,
                link,
                linkName,
                linkCaption,
                linkDescription,
                picture,
                mediaSource,
                actionName,
                actionLink,
                reference,
                properties,
                this.OnFeedCompleted);
        }
    }

    public void PostScreenShot(byte[] screenshot, string linkCaption, string linkDescription, string linkName, string link)
    {
        if (this.IsLoggedIn)
        {
            WWWForm wwwForm = new WWWForm();
            string filename = string.Format("TMS_{0:yyyy_MMM_dd_HH_mm_ss}.png", DateTime.Now);
            wwwForm.AddBinaryData("image", screenshot, filename);

            // Post the screen shot on Facebook
            FB.API(
                "me/photos",
                HttpMethod.POST,
                result =>
                    {
                        if (result.Error != null)
                        {
                            FbDebug.Error(result.Error);

                            return;
                        }

                        // The photo posted OK, so write on Feed
                        string photoId = this.DeserializeScreenShotId(result.Text);
                        string url = string.Format(@"https://www.facebook.com/photo.php?fbid={0}", photoId);

                        Debug.Log("Screenshot URL: " + url);

                        // TODO: The URL needs to have .jpg or .png at the end. Not sure how to do this yet.
                        // this.PostOnFeed(
                        // 	link: link, 
                        // 	linkName: linkName, 
                        // 	linkCaption: linkCaption, 
                        // 	linkDescription: linkDescription,
                        // 	picture: url);
                        this.PostOnFeed(link: link, linkName: linkName, linkCaption: linkCaption, linkDescription: linkDescription);
                    },
                wwwForm);
        }
    }

    public void SendAppRequest(string message, string title, string[] recipients = null, string data = "request")
    {
        FB.AppRequest(message: message, to: recipients, data: data, title: title, callback: this.OnSendAppRequestCompleted);
    }

    #endregion

    #region Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        this.IsInitialized = false;
        this.requestBeingDeleted = null;
        this.profilePicturePath = Application.persistentDataPath + @"/profilePicture.png";
        this.HighScores = new List<UserHighScore>();

        DontDestroyOnLoad(this.gameObject);
    }

    private List<FacebookAppRequest> DeserializeAppRequest(string response)
    {
        Debug.Log("App Request Response: " + response);

        Dictionary<string, object> responseObject = Json.Deserialize(response) as Dictionary<string, object>;
        List<FacebookAppRequest> appRequests = new List<FacebookAppRequest>();

        if (responseObject != null)
        {
            object val;

            if (responseObject.TryGetValue("data", out val))
            {
                List<object> valList = (List<object>)val;

                foreach (object valListObject in valList)
                {
                    FacebookAppRequest appRequest = new FacebookAppRequest();
                    Dictionary<string, object> dataObject = valListObject as Dictionary<string, object>;

                    if (dataObject != null)
                    {
                        if (dataObject.TryGetValue("id", out val))
                        {
                            appRequest.RequestId = (string)val;
                        }

                        if (dataObject.TryGetValue("application", out val))
                        {
                            Dictionary<string, object> applicationObject = val as Dictionary<string, object>;

                            if (applicationObject != null)
                            {
                                if (applicationObject.TryGetValue("name", out val))
                                {
                                    appRequest.ApplicationName = (string)val;
                                }

                                if (applicationObject.TryGetValue("id", out val))
                                {
                                    appRequest.ApplicationId = (string)val;
                                }
                            }
                        }

                        if (dataObject.TryGetValue("to", out val))
                        {
                            Dictionary<string, object> toObject = val as Dictionary<string, object>;

                            if (toObject != null)
                            {
                                if (toObject.TryGetValue("name", out val))
                                {
                                    appRequest.ToName = (string)val;
                                }

                                if (toObject.TryGetValue("id", out val))
                                {
                                    appRequest.ToId = (string)val;
                                }
                            }
                        }

                        if (dataObject.TryGetValue("from", out val))
                        {
                            Dictionary<string, object> fromObject = val as Dictionary<string, object>;

                            if (fromObject != null)
                            {
                                if (fromObject.TryGetValue("name", out val))
                                {
                                    appRequest.FromName = (string)val;
                                }

                                if (fromObject.TryGetValue("id", out val))
                                {
                                    appRequest.FromId = (string)val;
                                }
                            }
                        }

                        if (dataObject.TryGetValue("data", out val))
                        {
                            appRequest.Data = (string)val;
                        }

                        if (dataObject.TryGetValue("message", out val))
                        {
                            appRequest.FromMessage = (string)val;
                        }

                        if (dataObject.TryGetValue("created_time", out val))
                        {
                            appRequest.FromCreateTime = (string)val;
                        }
                    }

                    appRequests.Add(appRequest);
                }
            }
        }

        return appRequests;
    }

    private List<UserHighScore> DeserializeHighScores(string response)
    {
        Dictionary<string, object> responseObject = Json.Deserialize(response) as Dictionary<string, object>;
        List<UserHighScore> highScores = new List<UserHighScore>();

        if (responseObject != null)
        {
            object val;

            if (responseObject.TryGetValue("data", out val))
            {
                List<object> userList = (List<object>)val;

                foreach (object userListObject in userList)
                {
                    UserHighScore userHighScore = new UserHighScore();
                    Dictionary<string, object> dataObject = userListObject as Dictionary<string, object>;

                    if (dataObject != null)
                    {
                        if (dataObject.TryGetValue("user", out val))
                        {
                            Dictionary<string, object> userObject = val as Dictionary<string, object>;

                            if (userObject != null)
                            {
                                if (userObject.TryGetValue("id", out val))
                                {
                                    userHighScore.UserId = (string)val;
                                }

                                if (userObject.TryGetValue("name", out val))
                                {
                                    userHighScore.Name = (string)val;
                                }
                            }
                        }

                        if (dataObject.TryGetValue("score", out val))
                        {
                            long highScore = (long)val;
                            userHighScore.HighScore = (uint)highScore;
                        }
                    }

                    highScores.Add(userHighScore);
                }
            }
        }

        return highScores;
    }

    private FacebookProfile DeserializeProfile(string response)
    {
        Dictionary<string, object> responseObject = Json.Deserialize(response) as Dictionary<string, object>;
        FacebookProfile profile = new FacebookProfile();

        if (responseObject != null)
        {
            object val;

            if (responseObject.TryGetValue("id", out val))
            {
                profile.Id = (string)val;
            }

            if (responseObject.TryGetValue("first_name", out val))
            {
                profile.FirstName = (string)val;
            }

            if (responseObject.TryGetValue("last_name", out val))
            {
                profile.LastName = (string)val;
            }
        }

        return profile;
    }

    private string DeserializeScreenShotId(string response)
    {
        Dictionary<string, object> responseObject = Json.Deserialize(response) as Dictionary<string, object>;
        string id = null;

        if (responseObject != null)
        {
            object val;

            if (responseObject.TryGetValue("id", out val))
            {
                id = (string)val;
            }
        }

        return id;
    }

    private string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null)
    {
        string url = string.Format(@"/{0}/picture", facebookID);
        string query = width != null ? "&width=" + width.ToString() : string.Empty;
        query += height != null ? "&height=" + height.ToString() : string.Empty;
        query += type != null ? "&type=" + type : string.Empty;
        if (!string.IsNullOrEmpty(query))
        {
            url += ("?g" + query);
        }

        return url;
    }

    private void NotifyFacebookAppRequestCompleted(List<FacebookAppRequest> appRequests)
    {
        if (this.FacebookAppRequestEvent != null)
        {
            this.FacebookAppRequestEvent(this, new FacebookAppRequestEventArgs(appRequests));
        }
    }

    private void NotifyFacebookAppRequestDeleted(FacebookAppRequest request)
    {
        this.requestBeingDeleted = null;
        this.AppRequests.Remove(request);

        if (this.FacebookAppRequestDeletedEvent != null)
        {
            this.FacebookAppRequestDeletedEvent(this, new FacebookAppRequestDeletedEventArgs(request));
        }
    }

    private void NotifyFacebookDeepLinkCompleted(string requestId)
    {
        if (this.FacebookDeepLinkResultEvent != null)
        {
            this.FacebookDeepLinkResultEvent(this, new FacebookDeepLinkResultEventArgs(requestId));
        }
    }

    private void NotifyFacebookInitialized()
    {
        if (this.FacebookInitializedEvent != null)
        {
            this.FacebookInitializedEvent(this, new EventArgs());
        }
    }

    private void NotifyFacebookLoggedIn(FBResult result)
    {
        if (this.FacebookLoggedInEvent != null)
        {
            this.FacebookLoggedInEvent(this, new FacebookLoggedInEventArgs(result));
        }
    }

    private void NotifyFacebookProfileRetreived(FacebookProfile fbProfile)
    {
        if (this.FacebookProfileRetreivedEvent != null)
        {
            this.FacebookProfileRetreivedEvent(this, new FacebookProfileEventArgs(fbProfile));
        }
    }

    private void NotifyFeedCompleted()
    {
        if (this.FacebookFeedCompletedEvent != null)
        {
            this.FacebookFeedCompletedEvent(this, new FacebookFeedCompletedEventArgs());
        }
    }

    private void NotifyProfilePictureRetreived(Texture2D texture)
    {
        if (this.FacebookProfilePictureRetreivedEvent != null)
        {
            this.FacebookProfilePictureRetreivedEvent(this, new FacebookProfilePictureEventArgs(texture));
        }
    }

    private void OnDeleteRequestCompleted(FBResult result)
    {
        if (result.Text != null && result.Error == null)
        {
            if (result.Text.Equals("True", StringComparison.CurrentCultureIgnoreCase))
            {
                this.NotifyFacebookAppRequestDeleted(this.requestBeingDeleted);
                Debug.Log(string.Format("Delete request completed: {0}", result.Text));
            }
            else
            {
                this.requestBeingDeleted = null;
                Debug.Log(string.Format("Delete didn't return true: {0}", result.Text));
            }
        }
        else
        {
            this.requestBeingDeleted = null;
            Debug.Log(string.Format("Delete failed: {0}", result.Error));
        }
    }

    private void OnFeedCompleted(FBResult result)
    {
        if (result.Error == null)
        {
            // Notify if there's no error
            this.NotifyFeedCompleted();
        }
    }

    private void OnGetAppRequestCompleted(FBResult result)
    {
        if (result.Text != null)
        {
            try
            {
                this.AppRequests = this.DeserializeAppRequest(result.Text);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error deserializing App Request: " + ex.Message);
                this.AppRequests = new List<FacebookAppRequest>();
            }

            this.NotifyFacebookAppRequestCompleted(this.AppRequests);
        }
    }

    private void OnGetDeepLinkCompleted(FBResult result)
    {
        string requests = string.Empty;

        if (result.Text != null)
        {
            string requestId = this.ParseQuery(result.Text, "request_ids");
            if (requestId != null)
            {
                requests = requestId;
            }
        }

        this.NotifyFacebookDeepLinkCompleted(requests);
    }

    private void OnGetProfileCompleted(FBResult result)
    {
        if (result.Error != null)
        {
            FbDebug.Error(result.Error);

            // Let's just try again                                                                                                
            FB.API(@"/me?fields=id,first_name,last_name", HttpMethod.GET, this.OnGetProfileCompleted);
            return;
        }

        this.Profile = this.DeserializeProfile(result.Text);

        this.NotifyFacebookProfileRetreived(this.Profile);
    }

    private void OnGetProfilePictureCompleted(FBResult result)
    {
        if (result.Error != null)
        {
            FbDebug.Error(result.Error);

            return;
        }

        this.ProfilePicture = result.Texture;
        TextureIo.SaveTextureToFile(this.profilePicturePath, this.ProfilePicture);
        this.NotifyProfilePictureRetreived(this.ProfilePicture);
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // pause the game - we will need to hide                                             
            Time.timeScale = 0;
        }
        else
        {
            // start the game back up - we're getting focus again                                
            Time.timeScale = 1;
        }
    }

    private void OnInviteCompleted(FBResult result)
    {
    }

    private void OnLoginCompleted(FBResult result)
    {
        this.IsLoggedIn = FB.IsLoggedIn;

        this.NotifyFacebookLoggedIn(result);
    }

    private void OnSendAppRequestCompleted(FBResult result)
    {
    }

    private void OnSetInit()
    {
        this.IsInitialized = true;
        this.IsLoggedIn = FB.IsLoggedIn;

        this.NotifyFacebookInitialized();
    }

    /// <summary>
    /// Parses the query.
    /// i.e. 
    /// query = https://www.facebook.com/apps/application.php?id=67890&request_ids=12345&ref=notif
    /// parameter = request_ids
    /// 
    /// return = 12345
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    private string ParseQuery(string query, string parameter)
    {
        string result = null;

        string[] strings = query.Split(new[] { '&' });

        foreach (string str in strings)
        {
            if (str.StartsWith(parameter))
            {
                string[] data = str.Split(new[] { '=' });

                if (data.Length == 2)
                {
                    result = data[1];
                    break;
                }
            }
        }

        return result;
    }

    #endregion
}