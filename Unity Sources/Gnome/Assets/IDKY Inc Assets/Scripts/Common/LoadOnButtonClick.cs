// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class LoadOnButtonClick : MonoBehaviour
{
    #region Public Fields

    public bool DestroySelf = true;

    public GameObject ObjectToLoad;

    public GameObject ParentObjectToDestroy;

    #endregion

    #region Private Methods

    private void OnMouseDown()
    {
        if (this.ObjectToLoad != null)
        {
            Instantiate(this.ObjectToLoad);
        }

        if (this.DestroySelf)
        {
            Destroy(this.ParentObjectToDestroy);
        }
    }

    #endregion
}