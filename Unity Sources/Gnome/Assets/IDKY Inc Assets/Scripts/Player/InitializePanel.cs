// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

[RequireComponent(typeof(UIPanel))]
public class InitializePanel : MonoBehaviour
{
    #region Methods

    private void Awake()
    {
        GameManager.Instance.Panel = this.GetComponent<UIPanel>();
    }

    #endregion
}