// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class SoundFxOnTrigger : MonoBehaviour
{
	#region Private Fields

	private bool played;

	#endregion

	#region Public Fields

	public AudioClip SoundFx;

	#endregion

	protected virtual void OnTriggerEnter(Collider col)
	{
		/*MonsterRunner runner = col.gameObject.GetComponent<MonsterRunner>();

		if (runner != null)
		{
			if (!this.played)
			{
				SoundManager.Instance.PlaySoundFx(this.SoundFx, this.transform.position);
				this.played = true;
			}
		}*/
	}
}