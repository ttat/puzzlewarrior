// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class HighScoreGridItem : MonoBehaviour
{
    #region Fields

    public UIWidget BackgroundUiWidget;

    public UITexture ProfilePicture;

    public UILabel Score;

    public UserHighScore UserHighScore;

    #endregion

    #region Public Methods and Operators

    public void UpdateUi()
    {
        this.Score.text = this.UserHighScore.HighScore.ToString();
        this.ProfilePicture.mainTexture = this.UserHighScore.ProfilePicture;
    }

    #endregion
}