// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using System.Collections.Generic;

using Idky;

using UnityEngine;

public class FacebookStartMenuUi : MonoBehaviour
{
    #region Fields

    public GameObject FacebookNotification;

    public GameObject LogInButton;

    public GameObject MailboxButton;

    public UILabel NotificationCount;

    public int ProfileHeight = 256;

    public UITexture ProfileImage;

    public UILabel ProfileName;

    public int ProfileWidth = 256;

    private FacebookProfile profile;

    #endregion

    #region Methods

    private void DoLogIn()
    {
        this.EnableDisableUi();

        if (FacebookManager.Instance.IsLoggedIn)
        {
            FacebookManager.Instance.GetProfile();
            FacebookManager.Instance.GetProfilePicture(width: this.ProfileWidth, height: this.ProfileHeight);
            FacebookManager.Instance.GetHighScores();
        }
    }

    private void EnableDisableUi()
    {
        if (LogInButton != null)
        {
            this.LogInButton.SetActive(!FacebookManager.Instance.IsLoggedIn);
        }

        if (MailboxButton != null)
        {
            this.MailboxButton.SetActive(FacebookManager.Instance.IsLoggedIn);
        }

        if (ProfileImage != null)
        {
            this.ProfileImage.gameObject.SetActive(FacebookManager.Instance.IsLoggedIn);
        }

        if (ProfileName != null)
        {
            this.ProfileName.gameObject.SetActive(FacebookManager.Instance.IsLoggedIn);
        }
    }

    private void OnDisable()
    {
        if (FacebookManager.Instance != null)
        {
            FacebookManager.Instance.FacebookLoggedInEvent -= this.OnFacebookLoggedIn;
            FacebookManager.Instance.FacebookProfileRetreivedEvent -= this.OnFacebookProfileRetreived;
            FacebookManager.Instance.FacebookProfilePictureRetreivedEvent -= this.OnFacebookProfilePictureRetreived;
            FacebookManager.Instance.FacebookAppRequestEvent -= this.OnFacebookAppRequest;
            FacebookManager.Instance.FacebookDeepLinkResultEvent -= this.OnFacebookDeepLinkResult;
            FacebookManager.Instance.FacebookAppRequestDeletedEvent -= this.OnFacebookAppRequestDeleted;
        }
    }

    private void OnFacebookAppRequest(object sender, FacebookAppRequestEventArgs args)
    {
        this.SetUpMailboxNotification(args.AppRequests);
    }

    private void OnFacebookAppRequestDeleted(object sender, FacebookAppRequestDeletedEventArgs e)
    {
        this.SetUpMailboxNotification(FacebookManager.Instance.AppRequests);
    }

    private void OnFacebookDeepLinkResult(object send, FacebookDeepLinkResultEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.RequestId))
        {
            this.MailboxButton.SendMessage("OnClick");
        }

        FacebookManager.Instance.GetAppRequests();
    }

    private void OnFacebookInitialized(object sender, EventArgs e)
    {
        FacebookManager.Instance.FacebookInitializedEvent -= this.OnFacebookInitialized;

        if (FacebookManager.Instance.IsLoggedIn)
        {
            // Only get deep link when initializing since we don't want to do this every time
            // the user goes to the screen
            FacebookManager.Instance.GetDeepLink();
        }

        this.DoLogIn();
    }

    private void OnFacebookLoggedIn(object sender, FacebookLoggedInEventArgs e)
    {
        this.DoLogIn();
    }

    private void OnFacebookProfilePictureRetreived(object sender, FacebookProfilePictureEventArgs e)
    {
        this.ProfileImage.material = new Material(Shader.Find("Unlit/Transparent Colored"));
		this.ProfileImage.mainTexture = e.ProfilePicture;
    }

    private void OnFacebookProfileRetreived(object sender, FacebookProfileEventArgs e)
    {
        this.profile = e.Profile;

        if (this.ProfileName != null)
        {
            this.ProfileName.text = string.Format("Hello {0} ", this.profile.FirstName, this.profile.LastName);
        }
    }

    private void SetUpMailboxNotification(List<FacebookAppRequest> appRequests)
    {
        if (appRequests.Count > 0)
        {
            this.NotificationCount.text = appRequests.Count.ToString();
            this.FacebookNotification.SetActive(true);
        }
        else
        {
            this.FacebookNotification.SetActive(false);
            this.NotificationCount.text = "0";
        }
    }

    private void Start()
    {
        FacebookManager.Instance.FacebookLoggedInEvent += this.OnFacebookLoggedIn;
        FacebookManager.Instance.FacebookProfileRetreivedEvent += this.OnFacebookProfileRetreived;
        FacebookManager.Instance.FacebookProfilePictureRetreivedEvent += this.OnFacebookProfilePictureRetreived;
        FacebookManager.Instance.FacebookAppRequestEvent += this.OnFacebookAppRequest;
        FacebookManager.Instance.FacebookDeepLinkResultEvent += this.OnFacebookDeepLinkResult;
        FacebookManager.Instance.FacebookAppRequestDeletedEvent += this.OnFacebookAppRequestDeleted;

        if (!FacebookManager.Instance.IsInitialized)
        {
            FacebookManager.Instance.FacebookInitializedEvent += this.OnFacebookInitialized;
            FacebookManager.Instance.Initialize();
        }
        else
        {
            this.DoLogIn();
            FacebookManager.Instance.GetAppRequests();
        }
    }

    #endregion
}