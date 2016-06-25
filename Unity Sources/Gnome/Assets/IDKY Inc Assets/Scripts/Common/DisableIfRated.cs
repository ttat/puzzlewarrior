// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class DisableIfRated : MonoBehaviour
{
    #region Fields

    public string RateOptionKey = "RateOption";

    #endregion

    #region Methods

    private void Start()
    {
        SetRateOptionOnClick.RateOptions rateOption = (SetRateOptionOnClick.RateOptions)PlayerPrefsFast.GetInt(this.RateOptionKey, 0);

        // Only show it if it's not been rated yet
        switch (rateOption)
        {
            case SetRateOptionOnClick.RateOptions.NotRated:
                this.gameObject.SetActive(true);
                break;

            case SetRateOptionOnClick.RateOptions.Rated:
            case SetRateOptionOnClick.RateOptions.DoNotShowAgain:
                this.gameObject.SetActive(false);
                break;
        }
    }

    #endregion
}