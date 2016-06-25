// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class SpecialStageSelectButton : MonoBehaviour
{
    #region Fields

    public ActivateOnClick ActivateOnClick;

    public bool IsUnlocked;

    public UILabel Label;

    public GameObject StageCompletedIcon;

    public GameObject StageLockedIcon;

    public GameObject StageNotCompletedIcon;

    private LevelSelectCreator levelSelectCreator;

    private PurchaseStage purchaseStagePopUp;

    private GameObject rewardedOnlyPopUp;

    private SpecialStage specialStage;

    private GameObject stageSelectOverrideActivateTarget;

    private GameObject stageSelectOverrideDeactivateTarget;

    #endregion

    #region Public Methods and Operators

    public void SelectStage()
    {
        if (this.IsUnlocked)
        {
            this.levelSelectCreator.SelectStage(this.specialStage);
        }
        else
        {
            SpecialStageLifetimeVg specialStageLifetimeVg = GnomeStoreAssets.GetSpecialStagesStatic()[this.specialStage.StageId];

            this.purchaseStagePopUp.SetSpecialStageItem(specialStageLifetimeVg, this.levelSelectCreator, this);
        }
    }

    public void SetSpecialStageCompletedIcons(bool unlocked = false)
    {
        // Didn't initialize yet
        if (this.levelSelectCreator == null)
        {
            return;
        }

        this.IsUnlocked = ShopManager.GoodsBalances[this.specialStage.StageId] > 0 || unlocked;

        if (this.IsUnlocked)
        {
            bool hasLevelIncomplete = false;

            foreach (int levelInStage in this.specialStage.LevelMapping.Keys)
            {
                string key = string.Format(
                    "{0}_{1}_{2}", SharedResources.TimesLevelPlayedToCompletePrefix, this.specialStage.StageId, levelInStage);

                int timesToCompleteLevel = PlayerPrefsFast.GetInt(key, 0);

                // Check if the level has been completed yet
                if (timesToCompleteLevel == 0)
                {
                    hasLevelIncomplete = true;
                    break;
                }
            }

            this.StageCompletedIcon.SetActive(!hasLevelIncomplete);
            this.StageNotCompletedIcon.SetActive(hasLevelIncomplete);
            this.StageLockedIcon.SetActive(false);

            // Set the stage select activate targets if it's unlocked
            if (this.ActivateOnClick != null)
            {
                this.ActivateOnClick.ActivateTarget = this.stageSelectOverrideActivateTarget;
                this.ActivateOnClick.DeactivateTarget = this.stageSelectOverrideDeactivateTarget;
            }
        }
        else
        {
            SpecialStageLifetimeVg specialStageLifetimeVg = GnomeStoreAssets.GetSpecialStagesStatic()[this.specialStage.StageId];

            if (specialStageLifetimeVg != null)
            {
                if (specialStageLifetimeVg.CanPurchase)
                {
                    // Show purchase with tokens pop up
                    this.ActivateOnClick.ActivateTarget = this.purchaseStagePopUp.gameObject;
                    this.ActivateOnClick.DeactivateTarget = this.levelSelectCreator.gameObject;
                }
                else
                {
                    // Show reward only pop up
                    this.ActivateOnClick.ActivateTarget = this.rewardedOnlyPopUp;
                    this.ActivateOnClick.DeactivateTarget = this.levelSelectCreator.gameObject;
                }
            }

            this.StageLockedIcon.SetActive(true);
            this.StageCompletedIcon.SetActive(false);
            this.StageNotCompletedIcon.SetActive(false);
        }
    }

    public void SetStage(
        LevelSelectCreator levelSelect,
        SpecialStage stage,
        GameObject stageSelectActivateTarget,
        GameObject stageSelectDeactivateTarget,
        PurchaseStage purchaseStage,
        GameObject rewardPopUp)
    {
        this.specialStage = stage;
        this.Label.text = this.specialStage.Description;
        this.name = this.specialStage.StageId;
        this.levelSelectCreator = levelSelect;
        this.stageSelectOverrideActivateTarget = stageSelectActivateTarget;
        this.stageSelectOverrideDeactivateTarget = stageSelectDeactivateTarget;
        this.purchaseStagePopUp = purchaseStage;
        this.rewardedOnlyPopUp = rewardPopUp;

        this.SetSpecialStageCompletedIcons();
    }

    #endregion

    #region Methods

    private void OnEnable()
    {
        this.SetSpecialStageCompletedIcons();
    }

    #endregion
}