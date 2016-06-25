// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class ResetAnimationOnDisable : MonoBehaviour
{
    #region Fields

    public UI2DSpriteAnimationIdky[] Animations;

    #endregion

    #region Methods

    private void OnDisable()
    {
        foreach (UI2DSpriteAnimationIdky spriteAnimation in this.Animations)
        {
            spriteAnimation.RestartAnimation();
        }
    }

    #endregion
}