// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class InviteOnClick : MonoBehaviour
{
    #region Fields

    public string Message = "Test message";

    public string Title = "Test title";

    #endregion

    #region Methods

    private void OnClick()
    {
        FacebookManager.Instance.Invite(this.Message, this.Title);
    }

    #endregion
}