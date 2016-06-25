// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class AchievementsOnClick : MonoBehaviour
{
    #region Private Methods

    private void OnClick()
    {

#if !UNITY_EDITOR
		
		GooglePlayGameServicesWrapper.Instance().ShowAchievements();
		
#endif

    }

    #endregion
}