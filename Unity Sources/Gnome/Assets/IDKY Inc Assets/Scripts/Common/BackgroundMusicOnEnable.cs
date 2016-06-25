// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections;

using UnityEngine;

public class BackgroundMusicOnEnable : MonoBehaviour
{
    #region Fields

    public AudioClip BackgroundMusic;

    #endregion

    #region Methods

    private void OnEnable()
    {
		if (SoundManager.Instance != null)
		{
			SoundManager.Instance.PlayBackgroundMusic(this.BackgroundMusic, true);
		}
    }

    private void Start()
    {
		if (SoundManager.Instance != null)
		{
			SoundManager.Instance.PlayBackgroundMusic(this.BackgroundMusic, true);
		}
    }

    #endregion
}