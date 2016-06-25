// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class InitializeInventory : MonoBehaviour
{
    #region Methods

    private void Awake()
    {
        GameManager.Instance.Inventory = this.gameObject;
    }

    #endregion
}