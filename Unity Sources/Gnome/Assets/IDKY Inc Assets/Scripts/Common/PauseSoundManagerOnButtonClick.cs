//  ----------------------------------------------
//  
//              Tearible Monster Run
//  
//   Copyright © 2013 IDKY
//   Authors: Thomson Tat, Dinh Cao
//  
//  ----------------------------------------------

using UnityEngine;
using System.Collections;

public class PauseSoundManagerOnButtonClick : MonoBehaviour
{
	#region Public Fields

	public SoundManager SoundManager;

	#endregion

	#region Private Methods

	private void OnClick()
	{
		this.SoundManager.Pause();
	}

	private void Start()
	{
		this.SoundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
	}
	
	#endregion
}

