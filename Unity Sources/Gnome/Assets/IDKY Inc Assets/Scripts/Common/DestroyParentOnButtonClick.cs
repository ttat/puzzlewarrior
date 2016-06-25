// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class DestroyParentOnButtonClick : MonoBehaviour
{
    #region Public Fields

    public GameObject ParentObjectToDestroy;

    #endregion

    #region Private Methods

    private void OnMouseDown()
    {
        Destroy(this.ParentObjectToDestroy);
    }

    #endregion
}