// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections;

using UnityEngine;

public class BragOnClick : MonoBehaviour
{
    #region Fields

	public string LinkCaption = "Test caption";

	public string LinkDescription = "Test description";

	public string LinkName = "Test link name";

	public string Link;

    #endregion

    #region Methods

    private void OnClick()
    {
		if (string.IsNullOrEmpty(this.Link))
		{
			this.Link = "https://apps.facebook.com/" + FB.AppId;
		}

		this.StartCoroutine("PostScreenShot");
    }

	private IEnumerator PostScreenShot()
	{
		yield return new WaitForEndOfFrame();
		
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		
		// split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
		yield return 0;

		byte[] screenshot = tex.EncodeToPNG();

		FacebookManager.Instance.PostScreenShot(screenshot, this.LinkCaption, this.LinkDescription, this.LinkName, this.Link);

		DestroyObject(tex);

		yield return 0;
	}

    #endregion
}