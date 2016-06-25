// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using ChartboostSDK;
using UnityEngine;

public class ShowAdOnClick : MonoBehaviour
{
    #region Public Fields

    public AdManager.AdNetwork AdNetwork = AdManager.AdNetwork.ChartBoost;

    public AdManager.DisplayedAdType DisplayType = AdManager.DisplayedAdType.FullScreen;

    public CBLocation Location = CBLocation.Default;

    public bool RequireCachedFirst = true;

    #endregion

    #region Private and Protected Methods

    private void OnClick()
    {
        bool canContinue = true;

        if (this.RequireCachedFirst)
        {
            canContinue = AdManager.Instance.IsCached(this.AdNetwork, this.DisplayType, this.Location);
        }

        if (canContinue)
        {
            AdManager.Instance.ShowAd(this.AdNetwork, this.DisplayType, this.Location);
        }
    }

    #endregion
}