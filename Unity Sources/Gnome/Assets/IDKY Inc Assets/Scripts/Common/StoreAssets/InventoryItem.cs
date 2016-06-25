// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using Soomla.Store;

using UnityEngine;

public abstract class InventoryItem : MonoBehaviour
{
    #region Static Fields

    private static bool StoreInitialized;

    #endregion

    #region Fields

    public UILabel Description;

    public UILabel Info;

    public string ItemId;

    public UILabel Quantity;

    public VirtualGood VirtualGood;

    #endregion

    #region Public Methods and Operators

    public void AllowClick(bool allow)
    {
        this.gameObject.SetActive(allow);
    }

    [ContextMenu("Set Allow Click False")]
    public void SetAllowClickFalse()
    {
        this.AllowClick(false);
    }

    [ContextMenu("Set Allow Click True")]
    public void SetAllowClickTrue()
    {
        this.AllowClick(true);
    }

    #endregion

    #region Methods

    protected abstract VirtualGood GetVirtualGood();

    protected abstract void OnClick();

    private void Awake()
    {
        this.VirtualGood = this.GetVirtualGood();
        this.ItemId = this.VirtualGood.ItemId;
        this.Description.text = this.VirtualGood.Name;
        this.Info.text = this.VirtualGood.Description;

        StoreEvents.OnGoodBalanceChanged += this.OnGoodBalanceChanged;
        ShopManager.Instance.ShopDataInitializedEvent += this.OnShopDataInitialized;
    }

    private void OnEnable()
    {
        if (this.VirtualGood != null && StoreInitialized)
        {
            int count;

            Debug.Log(string.Format("2UPPER: InventoryOnEnable ItemId: {0}", this.ItemId));
            if (ShopManager.GoodsBalances.TryGetValue(this.ItemId, out count))
            {
                Debug.Log(string.Format("2UPPER: InventoryOnEnable Found - ItemId: {0}, Count: {1}", this.ItemId, count));
                this.Quantity.text = "Quantity: " + count.ToString("D");
            }
            else
            {
                Debug.Log(string.Format("2UPPER: InventoryOnEnable - ItemId: {0}, NotFound", this.ItemId));
                this.Quantity.text = "Quantity: 0";
            }
        }
    }

    private void OnGoodBalanceChanged(VirtualGood vg, int balance, int amountAdded)
    {
        if (this.ItemId.Equals(vg.ItemId))
        {
            Debug.Log(string.Format("2UPPER: GoodsBalanceChanged - ItemId: {0}, Balance: {1}", this.ItemId, balance));
            this.Quantity.text = "Quantity: " + balance.ToString("D");
        }
    }

    private void OnShopDataInitialized(object sender, ShopDataInitializedEventArgs e)
    {
        Debug.Log("2UPPER: InventoryItem OnShopDataInitialized");
        StoreInitialized = true;

        int count;

        Debug.Log(string.Format("2UPPER: InventoryOnStoreControllerInitialized ItemId: {0}", this.ItemId));
        if (ShopManager.GoodsBalances.TryGetValue(this.ItemId, out count))
        {
            Debug.Log(
                string.Format("2UPPER: InventoryOnStoreControllerInitialized Found - ItemId: {0}, Count: {1}", this.ItemId, count));
            this.Quantity.text = "Quantity: " + count.ToString("D");
        }
        else
        {
            Debug.Log(string.Format("2UPPER: InventoryOnStoreControllerInitialized - ItemId: {0}, NotFound", this.ItemId));
            this.Quantity.text = "Quantity: 0";
        }
    }

    private void Start()
    {
        int count;

        if (ShopManager.GoodsBalances.TryGetValue(this.ItemId, out count))
        {
            this.Quantity.text = "Quantity: " + count.ToString("D");
        }
        else
        {
            this.Quantity.text = "Quantity: 0";
        }
    }

    #endregion
}