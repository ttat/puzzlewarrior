// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ExitApplicationOnBackButton : MonoBehaviour
{
    #region Private Methods

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_ANDROID
            // Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
            if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
            {
                return;
            }
#endif

            Application.Quit();
        }
    }

    #endregion
}