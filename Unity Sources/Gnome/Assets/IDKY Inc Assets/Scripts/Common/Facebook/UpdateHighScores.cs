// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections.Generic;
using System.Linq;

using Idky;

using UnityEngine;

public class UpdateHighScores : MonoBehaviour
{
    #region Fields

    public UIRect.AnchorPoint BottomAnchor;

    public UIGrid Grid;

    public GameObject HighScoreGridItem;

    public UIRect.AnchorPoint LeftAnchor;

    /// <summary>
    /// Limits how many high scores to show
    /// </summary>
    public int MaxItems = 10;

    public UIRect.AnchorPoint RightAnchor;

    public UIScrollView ScrollView;

    public UIRect.AnchorPoint TopAnchor;

    #endregion

    #region Public Methods and Operators

    public void Reposition()
    {
        this.Grid.Reposition();
        this.ScrollView.ResetPosition();
    }

    public void UpdateGrid()
    {
        if (FacebookManager.Instance.IsLoggedIn)
        {
            int numItems = Mathf.Min(FacebookManager.Instance.HighScores.Count, this.MaxItems);
            List<UserHighScore> userHighScore = FacebookManager.Instance.HighScores.GetRange(0, numItems);

            foreach (Transform childTransform in this.Grid.transform)
            {
                Destroy(childTransform.gameObject);
            }

            foreach (UserHighScore highScore in userHighScore.OrderByDescending(u => u.HighScore))
            {
                GameObject item = NGUITools.AddChild(this.Grid.gameObject, this.HighScoreGridItem);

                HighScoreGridItem highScoreGridItem = item.GetComponent<HighScoreGridItem>();
                highScoreGridItem.UserHighScore = highScore;
                highScoreGridItem.UpdateUi();

                // To make sure the alignment is correct
                UIWidget widgetItem = highScoreGridItem.BackgroundUiWidget;
                widgetItem.leftAnchor = this.LeftAnchor;
                widgetItem.rightAnchor = this.RightAnchor;
                widgetItem.topAnchor = this.TopAnchor;
                widgetItem.bottomAnchor = this.BottomAnchor;
                widgetItem.ResetAnchors();
            }
        }
    }

    #endregion

    #region Methods

    private void OnEnable()
    {
        this.Reposition();
    }

    #endregion
}