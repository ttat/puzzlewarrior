// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using Soomla.Store;

using UnityEngine;

public class PurchaseStage : MonoBehaviour
{
    #region Fields

    public UILabel Message;

    public GameObject NotEnoughTokensPopUp;

    private LevelSelectCreator levelSelectCreator;

    private SpecialStageSelectButton specialStageSelectButton;

    private SpecialStageLifetimeVg specialStageVG;

    #endregion

    #region Public Methods and Operators

    public void PurchaseSpecialStage()
    {
        if (ShopManager.Instance.PurchaseStage(this.specialStageVG.ItemId))
        {
            this.levelSelectCreator.gameObject.SetActive(true);
            this.specialStageSelectButton.SetSpecialStageCompletedIcons(true);
        }
        else
        {
            this.NotEnoughTokensPopUp.SetActive(true);
        }
    }

    public void SetSpecialStageItem(
        SpecialStageLifetimeVg specialStage, LevelSelectCreator levelSelect, SpecialStageSelectButton selectStageButton)
    {
        this.specialStageVG = specialStage;
        this.levelSelectCreator = levelSelect;
        this.specialStageSelectButton = selectStageButton;

        PurchaseWithVirtualItem purchaseType = (PurchaseWithVirtualItem)specialStage.PurchaseType;

        this.Message.text = string.Format("Would you like to purchase this stage for {0} tokens?", purchaseType.Amount);
    }

    #endregion
}