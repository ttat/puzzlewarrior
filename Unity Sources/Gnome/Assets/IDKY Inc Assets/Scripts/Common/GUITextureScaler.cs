// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using UnityEngine;

[RequireComponent(typeof (GUITexture))]
public class GUITextureScaler : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The resolution the GUITexture was designed at
    /// </summary>
    public float DesignedHeight = 400;

    public float DesignedWidth = 640;

    #endregion

    // Use this for initialization

    #region Private Methods

    private void Start()
    {
        // Make sure it's not null
        if (this.guiTexture != null)
        {
            // Find the ratio needed to make it fit on the screen
            float widthRatio = Screen.width/this.DesignedWidth;
            float heightRatio = Screen.height/this.DesignedHeight;
            float ratio = Mathf.Max(widthRatio, heightRatio);

            // Calculate the new sizes
            Rect pixelInset = new Rect();
            pixelInset.width = this.guiTexture.pixelInset.width*ratio;
            pixelInset.height = this.guiTexture.pixelInset.height*ratio;
            pixelInset.x = this.guiTexture.pixelInset.x*ratio;
            pixelInset.y = this.guiTexture.pixelInset.y*ratio;

            // Set the new size
            this.guiTexture.pixelInset = pixelInset;
        }
    }

    #endregion
}