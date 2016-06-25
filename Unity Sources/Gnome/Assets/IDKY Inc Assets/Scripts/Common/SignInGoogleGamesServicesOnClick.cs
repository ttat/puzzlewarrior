// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using Idky;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SignInGoogleGamesServicesOnClick : MonoBehaviour
{	
    #region Private Fields
	
	private bool clicked;
	
	#endregion

	#region Public Fields

	public string GooglePlusKey = "GooglePlusSignedIn";

	public bool IsSignIn = true;

	#endregion

	#region Public Methods

	public void SignIn()
	{

#if UNITY_EDITOR

		this.SetActiveState();

#else
		
		GooglePlayGameServicesWrapper.Instance().SignedInSucceeded += this.OnSignInSucceeded;
		GooglePlayGameServicesWrapper.Instance().SignedInFailed += this.OnSignInFailed;
		GooglePlayGameServicesWrapper.Instance().SignedOut += this.OnSignedOut;
		GooglePlayGameServicesWrapper.Instance().SignIn();

#endif

	}

	public void SignOut()
	{

#if UNITY_EDITOR

		this.SetActiveState();

#else
		
		GooglePlayGameServicesWrapper.Instance().SignOut();
		this.SetActiveState();

#endif

	}

	#endregion

	#region Private and Protected Methods
	
	private void Awake()
	{
		this.clicked = false;
		
#if !UNITY_EDITOR

		GooglePlayGameServicesWrapper.Instance().SignedInSucceeded += this.OnSignInSucceeded;
		GooglePlayGameServicesWrapper.Instance().SignedInFailed += this.OnSignInFailed;
		GooglePlayGameServicesWrapper.Instance().SignedOut += this.OnSignedOut;
		
#endif

	}
	
	private void OnClick()
	{
		this.clicked = true;
		
		if (this.IsSignIn)
		{
			this.SignIn();
		}
		else
		{
			this.SignOut();
		}
	}
	
	private void OnDestroy()
	{
		
#if !UNITY_EDITOR

		GooglePlayGameServicesWrapper.Instance().SignedInSucceeded -= this.OnSignInSucceeded;
		GooglePlayGameServicesWrapper.Instance().SignedInFailed -= this.OnSignInFailed;
		GooglePlayGameServicesWrapper.Instance().SignedOut -= this.OnSignedOut;
		
#endif

	}
	
	private void OnSignInFailed(object sender, EventArgs args)
	{
        if (Debug.isDebugBuild)
        {
            Debug.LogError("Sign In Failed");
        }

	    this.SetActiveState();
	}

	private void OnSignInSucceeded(object sender, EventArgs args)
	{
        if (Debug.isDebugBuild)
        {
            Debug.Log("Sign In Succeeded");
        }

	    // Successfully signed in, so silently sign in next time
		PlayerPrefsFast.SetBool(this.GooglePlusKey, true);
		PlayerPrefsFast.Flush();
		/*
		if (this.clicked)
		{
			LevelDataInitializer.InitializeLevels();
		}
		*/
		this.clicked = false;
		
		this.SetActiveState();
	}

	private void OnSignedInOrOut(object sender, EventArgs args)
	{
		this.SetActiveState();
	}

	private void OnSignedOut(object sender, EventArgs args)
	{
        if (Debug.isDebugBuild)
        {
            Debug.Log("Signed Out");
        }

	    // Signed out, so next time don't silently sign in
		PlayerPrefsFast.SetBool(this.GooglePlusKey, false);
		PlayerPrefsFast.Flush();

		this.SetActiveState();
	}

	private void SetActiveState()
	{

#if UNITY_EDITOR

		this.gameObject.SetActive(false);

#else

		bool signedIn = GooglePlayGameServicesWrapper.Instance().IsSignedIn();
		this.gameObject.SetActive(signedIn != this.IsSignIn);

#endif

	}

	private void Start()
	{
		this.SetActiveState();
	}

	#endregion
}