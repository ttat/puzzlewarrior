// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SetActiveGoogleGamesServicesSignedIn : MonoBehaviour
{
    #region Public Fields

    public bool ActiveIfSignIn = true;

    #endregion

    #region Private and Protected Methods

	private void Awake()
	{
		
#if !UNITY_EDITOR

        GooglePlayGameServicesWrapper.Instance().SignedInSucceeded += this.OnSignInSucceeded;
        GooglePlayGameServicesWrapper.Instance().SignedInFailed += this.OnSignInFailed;
        GooglePlayGameServicesWrapper.Instance().SignedOut += this.OnSignedOut;
		
#endif
		
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
        this.SetActiveState();
    }

    private void OnSignInSucceeded(object sender, EventArgs args)
    {
        this.SetActiveState();
    }

    private void OnSignedOut(object sender, EventArgs args)
    {
        this.SetActiveState();
    }

    private void SetActiveState()
    {

#if UNITY_EDITOR

        this.gameObject.SetActive(!this.ActiveIfSignIn);

#else

        bool signedIn = GooglePlayGameServicesWrapper.Instance().IsSignedIn();
        this.gameObject.SetActive(signedIn == this.ActiveIfSignIn);

#endif

    }

    private void Start()
    {		
        this.SetActiveState();
    }

    #endregion
}