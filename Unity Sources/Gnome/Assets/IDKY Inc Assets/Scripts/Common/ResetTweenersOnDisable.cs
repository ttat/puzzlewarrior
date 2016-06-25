// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ResetTweenersOnDisable : MonoBehaviour
{
    #region Fields

    public UITweener[] Tweeners;

    #endregion

    #region Methods

    private void OnDisable()
    {
        foreach (UITweener tweener in this.Tweeners)
        {
            tweener.enabled = true;
            tweener.ResetToBeginning();
        }
    }

    #endregion
}