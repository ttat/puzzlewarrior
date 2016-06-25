// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class StageSelectButton : MonoBehaviour
{
    #region Fields

    public ActivateOnClick ActivateOnClick;

    public UILabel Label;

    public GameObject LockedStageIcon;

    public GameObject StageCompletedIcon;

    public GameObject StageNotCompletedIcon;

    private LevelSelectCreator levelSelectCreator;

    private int stageNumber;

    private GameObject stageSelectOverrideActivateTarget;

    private GameObject stageSelectOverrideDeactivateTarget;

    #endregion

    #region Public Methods and Operators

    public void SelectStage()
    {
        this.levelSelectCreator.SelectStage(this.stageNumber);
    }

    public void SetStage(LevelSelectCreator levelSelect, int stage, GameObject activateTarget, GameObject deactivateTarget)
    {
        this.stageNumber = stage;
        this.Label.text = string.Format("Stage {0}", this.stageNumber.ToString("D"));
        this.name = string.Format("Stage {0}", this.stageNumber.ToString("D2"));
        this.levelSelectCreator = levelSelect;
        this.stageSelectOverrideActivateTarget = activateTarget;
        this.stageSelectOverrideDeactivateTarget = deactivateTarget;

        this.SetCompletedIcons();
    }

    #endregion

    #region Methods

    private void OnEnable()
    {
        this.SetCompletedIcons();
    }

    private void SetCompletedIcons()
    {
        // Didn't initialize yet
        if (this.levelSelectCreator == null)
        {
            return;
        }

        // Get the first and last level in the stage
        int lastLevelInStage = this.levelSelectCreator.LevelsPerStage * this.stageNumber;
        int firstLevelInStage = lastLevelInStage - this.levelSelectCreator.LevelsPerStage + 1;

        int levelUnlocked = PlayerPrefsFast.GetInt(SharedResources.LevelUnlockedKey, 1);

        // Stage is completed if the level after the last level in the stage is unlocked
        bool isStageCompleted = levelUnlocked > lastLevelInStage;

        // Stage is unlocked if the first level in the stage is unlocked
        bool isStageUnlocked = levelUnlocked >= firstLevelInStage;

        // Turn on the appropriate icon
        this.StageCompletedIcon.SetActive(isStageCompleted);
        this.StageNotCompletedIcon.SetActive(!isStageCompleted && isStageUnlocked);
        this.LockedStageIcon.SetActive(!isStageUnlocked);

        // Enable the button only if the stage is unlocked
        if (isStageUnlocked)
        {
            this.ActivateOnClick.ActivateTarget = this.stageSelectOverrideActivateTarget;
            this.ActivateOnClick.DeactivateTarget = this.stageSelectOverrideDeactivateTarget;
        }
    }

    #endregion
}