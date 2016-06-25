// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class SoundFxOnClick : MonoBehaviour
{
    #region Public Fields

    public AudioClip SoundFx;

    #endregion

    #region Private Methods

    private void OnClick()
    {
        SoundManager.Instance.PlaySoundFx(this.SoundFx, this.transform.position);
    }

    #endregion
}