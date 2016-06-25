// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;

using Idky;

using Soomla.Store;

using UnityEngine;

public class PurchaseCurrencyPackItem : MonoBehaviour
{
    #region Fields

    public UILabel Description;

    public string ItemId;

    public ItemPackType ItemPack = ItemPackType.Token75Pack;

    public UILabel Price;

    public int TrimSize = 200;

    public VirtualCurrencyPack VirtualCurrencyPack;

    #endregion

    #region Enums

    public enum ItemPackType
    {
        Token75Pack,

        Token250Pack,

        Token500Pack
    }

    #endregion

    #region Methods

    private void OnClick()
    {
#if UNITY_ANDROID
        // Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
        if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
        {
            return;
        }
#endif

        StoreInventory.BuyItem(this.ItemId);
    }

    // Use this for initialization
    private void Start()
    {
        switch (this.ItemPack)
        {
            case ItemPackType.Token75Pack:
                this.VirtualCurrencyPack = GnomeStoreAssets.TokenPack75;
                break;

            case ItemPackType.Token250Pack:
                this.VirtualCurrencyPack = GnomeStoreAssets.TokenPack250;
                break;

            case ItemPackType.Token500Pack:
                this.VirtualCurrencyPack = GnomeStoreAssets.TokenPack500;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        this.ItemId = this.VirtualCurrencyPack.ItemId;
        this.Description.text = this.VirtualCurrencyPack.Description;

        this.Price.text = "$" + ((PurchaseWithMarket)this.VirtualCurrencyPack.PurchaseType).MarketItem.Price.ToString("F2");

        UIPanel uiPanel = NGUITools.FindInParents<UIPanel>(this.gameObject);
        Vector2 viewSize = uiPanel.GetViewSize();
        int buttonWidth = (int)viewSize.x - this.TrimSize;

        UIWidget uiWidget = this.GetComponent<UIWidget>();

        if (uiWidget != null)
        {
            uiWidget.SetDimensions(buttonWidth, uiWidget.height);
        }
    }

    #endregion
}