// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    #region Fields

    public ActivateOnClick[] ActivateOnClick;

    public GameObject ButtonCompleted;

    public GameObject ButtonCompletedFirstTry;

    public GameObject ButtonLocked;

    public GameObject ButtonUnlocked;

    public UILabel[] Labels;

    private GameTable gameTable;

    private int level;

    #endregion

    #region Public Methods and Operators

    public void SelectLevel()
    {
        this.gameTable.Level = this.level;
        this.gameTable.SetSpecialStageMode(false);
        this.gameTable.LoadGameBoard();
    }

    public void SetLevel(GameTable table, int setLevel)
    {
        this.level = setLevel;

        foreach (UILabel label in Labels)
        {
            label.text = this.level.ToString();
        }

        this.name = string.Format("Level {0}", this.level.ToString("D3"));
        this.gameTable = table;
    }

    #endregion

    #region Methods

    private void OnEnable()
    {
        int timeToComplete = PlayerPrefsFast.GetInt(
            string.Format("{0}_{1}", SharedResources.TimesLevelPlayedToCompletePrefix, this.level), 0);
        int levelsUnlocked = PlayerPrefsFast.GetInt(SharedResources.LevelUnlockedKey, 1);

        this.ButtonCompleted.SetActive(false);
        this.ButtonCompletedFirstTry.SetActive(false);
        this.ButtonLocked.SetActive(false);
        this.ButtonUnlocked.SetActive(false);

        if (this.level <= levelsUnlocked)
        {
            if (timeToComplete == 0)
            {
                // Didn't pass level yet
                this.ButtonUnlocked.SetActive(true);
            }
            else if (timeToComplete == 1)
            {
                // Passed the level on the first try
                this.ButtonCompletedFirstTry.SetActive(true);
            }
            else
            {
                // Passed the level, but not on the first try
                this.ButtonCompleted.SetActive(true);
            }
        }
        else
        {
            // Level still locked
            this.ButtonLocked.SetActive(true);
        }
    }

    #endregion
}