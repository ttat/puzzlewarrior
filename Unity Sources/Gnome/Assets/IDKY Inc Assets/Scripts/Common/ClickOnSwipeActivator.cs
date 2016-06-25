// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ClickOnSwipeActivator : MonoBehaviour
{
    #region Public Fields

    public GameObject Receiver;

    #endregion

    #region Private Methods

    private void OnDisable()
    {
        if (this.Receiver != null)
        {
            this.Receiver.SendMessage("ActivateClickOnSwipe", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            this.transform.parent.BroadcastMessage("ActivateClickOnSwipe", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnEnable()
    {
        if (this.Receiver != null)
        {
            this.Receiver.SendMessage("DeactivateClickOnSwipe", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            this.transform.parent.BroadcastMessage("DeactivateClickOnSwipe", SendMessageOptions.DontRequireReceiver);
        }
    }

    #endregion
}