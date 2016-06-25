// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class DisableOnCall : MonoBehaviour
{
    #region Public Methods and Operators

    public void DisableGameObject()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}