// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

[RequireComponent(typeof (GUIText))]
public class GUITextScaler : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The resolution the GUIText was designed at
    /// </summary>
    public float DesignedHeight = 400;

    public float DesignedWidth = 640;

    #endregion

    // Use this for initialization

    #region Private Methods

    private void Start()
    {
        // Make sure it's not null
        if (this.guiText != null)
        {
            // Find the ratio needed to make it fit on the screen
            float widthRatio = Screen.width/this.DesignedWidth;
            float heightRatio = Screen.height/this.DesignedHeight;
            float ratio = Mathf.Max(widthRatio, heightRatio);

            // Resize it
            this.guiText.fontSize = (int) (this.guiText.fontSize*ratio);
        }
    }

    #endregion
}