// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ClickOnBackButtonPressActivator : MonoBehaviour
{
    #region Public Fields

    public GameObject Receiver;

    #endregion

    #region Private Methods

    private void OnDisable()
    {
        if (this.Receiver != null)
        {
            this.Receiver.SendMessage("ActivateClickOnBackButtonPress", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnEnable()
    {
        if (this.Receiver != null)
        {
            this.Receiver.SendMessage("DeactivateClickOnBackButtonPress", SendMessageOptions.DontRequireReceiver);
        }
    }

    #endregion
}