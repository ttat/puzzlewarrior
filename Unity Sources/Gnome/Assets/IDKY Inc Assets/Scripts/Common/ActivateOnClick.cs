// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ActivateOnClick : MonoBehaviour
{
    #region Fields

    public GameObject ActivateTarget;

    public ActivateOnClick BackButton;

    public GameObject BackButtonActivateTarget;

    public GameObject DeactivateTarget;

    public bool OverrideBackTarget = false;

    #endregion

    #region Methods

    protected virtual void OnClick()
    {
#if UNITY_ANDROID
        // Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
        if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
        {
            return;
        }
#endif

        if (this.DeactivateTarget != null)
        {
            this.DeactivateTarget.SetActive(false);
        }

        if (this.ActivateTarget != null)
        {
            this.ActivateTarget.SetActive(true);
        }

        if (this.OverrideBackTarget && this.BackButton != null)
        {
            this.BackButton.ActivateTarget = this.BackButtonActivateTarget;
        }
    }

    #endregion
}