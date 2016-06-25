// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class SpecialStageLevelSelectButton : MonoBehaviour
{
    #region Fields

    public ActivateOnClick[] ActivateOnClick;

    public GameObject ButtonCompleted;

    public GameObject ButtonCompletedFirstTry;

    public GameObject ButtonUnlocked;

    public UILabel[] Labels;

    private GameTable gameTable;

    private int level;

    private string stageId;

    #endregion

    #region Public Methods and Operators

    public void SelectLevel()
    {
        this.gameTable.Level = this.level;
        this.gameTable.SetSpecialStageMode(true);
        this.gameTable.LoadGameBoard();
    }

    public void SetLevel(GameTable table, string id, int setLevel)
    {
        this.stageId = id;
        this.level = setLevel;

        foreach (UILabel label in this.Labels)
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
        string key = string.Format("{0}_{1}_{2}", SharedResources.TimesLevelPlayedToCompletePrefix, this.stageId, this.level);
        int timeToComplete = PlayerPrefsFast.GetInt(key, 0);

        this.ButtonCompleted.SetActive(false);
        this.ButtonCompletedFirstTry.SetActive(false);
        this.ButtonUnlocked.SetActive(false);

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

    #endregion
}