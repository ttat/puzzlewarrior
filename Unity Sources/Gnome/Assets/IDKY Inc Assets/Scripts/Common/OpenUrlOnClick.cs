// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

public class OpenUrlOnClick : MonoBehaviour
{
    #region Public Fields
	
    public string UrlAmazon;
	
	public string UrlApple;
    
	public string UrlDefault;
    
	public string UrlGoogle;
    
	public string UrlSamsung;
    
#endregion

    #region Private Methods

    private void OnClick()
    {
		string url = null;
		
#if GOOGLE
		
		url = this.UrlGoogle;
		
#elif AMAZON
		
		url = this.UrlAmazon;
		
#elif SAMSUNG
		
		url = this.UrlSamsung;
		
#elif APPLE
		
		url = this.UrlApple;
		
#endif
		
		if (string.IsNullOrEmpty(url))
		{
			url = this.UrlDefault;
		}
			
		Application.OpenURL(url);
    }

    #endregion
}