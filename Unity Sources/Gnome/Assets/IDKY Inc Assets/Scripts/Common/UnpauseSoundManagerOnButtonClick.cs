//  ----------------------------------------------
//  
//              Tearible Monster Run
//  
//   Copyright © 2013 IDKY
//   Authors: Thomson Tat, Dinh Cao
//  
//  ----------------------------------------------
// 
using UnityEngine;
using System.Collections;

public class UnpauseSoundManagerOnButtonClick : MonoBehaviour
{
	#region Public Fields

	public SoundManager SoundManager;

	#endregion

	#region Private Methods

	private void OnClick()
	{
		this.SoundManager.Unpause();
	}

	private void Start()
	{
		this.SoundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
	}
	
	#endregion
}

