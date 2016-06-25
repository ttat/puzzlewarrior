// ----------------------------------------------
// 
//             2 Upper
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class UpdateShopClickLocation : MonoBehaviour
{
    #region Fields

    public string PurchaseButtonLocation = "Unknown";

    #endregion

    #region Public Methods and Operators

    public void OnClick()
    {
        ShopManager.Instance.PurchaseButtonLocation = this.PurchaseButtonLocation;
    }

    #endregion
}