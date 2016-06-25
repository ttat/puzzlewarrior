// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class LoadSceneOnButtonClick : MonoBehaviour
{
	#region Fields

	public string SceneName;

	#endregion

	#region Methods

	protected virtual void OnClick()
	{
		// Load the level with a slight delay so the loading screen shows up.
		Time.timeScale = 1;
		this.Invoke("LoadLevel", 0.25f);
	}

	private void Awake()
	{
	}

	private void LoadLevel()
	{
		Application.LoadLevel(this.SceneName);
	}

	#endregion
}