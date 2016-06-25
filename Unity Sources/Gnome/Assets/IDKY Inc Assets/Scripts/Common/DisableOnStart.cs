// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
    #region Private and Protected Methods

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}