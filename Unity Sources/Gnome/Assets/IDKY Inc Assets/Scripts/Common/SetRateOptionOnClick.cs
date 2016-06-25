// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;
using UnityEngine;

public class SetRateOptionOnClick : MonoBehaviour
{
    #region RateOptions enum

    public enum RateOptions
    {
        NotRated,

        Rated,

        DoNotShowAgain
    }

    #endregion

    #region Public Fields

    public RateOptions Option = RateOptions.NotRated;

    public string OptionKey = "RateOption";

    #endregion

    #region Private and Protected Methods

    private void OnClick()
    {
        // Log what button the user clicked for rating us
        GoogleAnalyticsV3.instance.LogEvent(new EventHitBuilder().SetEventCategory("Rating").SetEventAction(this.Option.ToString()));

        PlayerPrefsFast.SetInt(this.OptionKey, (int) this.Option);
        PlayerPrefsFast.Flush();
    }

    #endregion
}