// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ClickOnApplicationPause : MonoBehaviour
{
    #region Private Fields

    private bool activated = true;

    #endregion

    #region Public Fields

    /// <summary>
    /// If anything here is active, it doesn't activate OnClick regardless of the 
    /// status Activated is in.
    /// </summary>
    public GameObject[] ActiveOverride;

    #endregion

    #region Private Methods

    private void ActivateClickOnApplicationPause()
    {
        this.activated = true;
    }

    private void DeactivateClickOnApplicationPause()
    {
        this.activated = false;
    }

    private bool NoOverrides()
    {
        if (this.ActiveOverride == null || this.ActiveOverride.Length == 0)
        {
            return true;
        }
        else
        {
            bool noOverride = true;

            foreach (GameObject obj in this.ActiveOverride)
            {
                if (obj.activeInHierarchy)
                {
                    noOverride = false;
                    break;
                }
            }

            return noOverride;
        }
    }

    private void OnApplicationPause()
    {
        if (this.activated && this.NoOverrides())
        {
            this.SendMessage("OnClick");
        }
    }

    #endregion
}