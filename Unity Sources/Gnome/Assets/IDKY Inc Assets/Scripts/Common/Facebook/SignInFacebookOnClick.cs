// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;
using Idky;

public class SignInFacebookOnClick : MonoBehaviour
{
    #region Methods

    private void OnClick()
    {
#if UNITY_ANDROID
        // Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
        if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
        {
            return;
        }
#endif

        FacebookManager.Instance.LogIn(FacebookStrings.Scopes);
    }

    #endregion
}