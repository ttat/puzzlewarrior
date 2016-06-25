// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections;

using UnityEngine;

public class ClickOnBackButtonPress : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// If anything here is active, it doesn't activate OnClick regardless of the 
    /// status Activated is in.
    /// </summary>
    public GameObject[] ActiveOverride;

    private bool activated = true;

    #endregion

    #region Methods

    private void ActivateClickOnBackButtonPress()
    {
        this.activated = true;
    }

    private IEnumerator CallOnClick()
    {
        yield return new WaitForSeconds(0.1f);

        this.SendMessage("OnClick");
    }

    private void DeactivateClickOnBackButtonPress()
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && this.activated && this.NoOverrides())
        {
            // Calls SendMessage after a slight delay otherwise multiple ClickOnButtonPress might be triggered
            // when they're not all supposed to
            this.StartCoroutine(this.CallOnClick());
        }
    }

    #endregion
}