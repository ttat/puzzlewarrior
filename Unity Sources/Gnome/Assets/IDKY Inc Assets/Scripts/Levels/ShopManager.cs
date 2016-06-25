// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using System.Collections.Generic;

using Idky;

using Soomla.Store;

using UnityEngine;

public class ShopManager : MonoBehaviour
{
	#region Static Fields

	public static Dictionary<string, int> GoodsBalances = new Dictionary<string, int>();

	public static ShopManager Instance;

	public static List<VirtualCurrency> VirtualCurrencies = null;

	public static List<VirtualCurrencyPack> VirtualCurrencyPacks = null;

	public static List<VirtualGood> VirtualGoods = null;

	public static Dictionary<string, VirtualGood> VirtualGoodsDictionary = null;

	#endregion

	#region Fields

	public UILabel[] CoinSubscribers;

	private bool isFirstTime;

	private int tokens;

	#endregion

	#region Public Events

	public event EventHandler<ShopDataInitializedEventArgs> ShopDataInitializedEvent;

	#endregion

	#region Public Properties

	public string PurchaseButtonLocation { get; set; }

	public int Tokens
	{
		get
		{
			return this.tokens;
		}
	}

	#endregion

	#region Public Methods and Operators

	public void GiveStage(string stageId)
	{
		if (GoodsBalances[stageId] == 0)
		{
			StoreInventory.GiveItem(stageId, 1);
			GoodsBalances[stageId] = 1;
		}
	}

	public bool IsStageAvailable(string stageId)
	{
		return GoodsBalances[stageId] > 0;
	}

	public bool PurchaseStage(string stageId)
	{
		// Already purchased the item
		if (GoodsBalances[stageId] > 0)
		{
			return true;
		}

		try
		{
			// Buy the item
			StoreInventory.BuyItem(stageId);

		    return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void GiveTokens(int amount)
	{
		this.tokens += amount;
		StoreInventory.GiveItem(GnomeStoreAssets.TokenId, amount);

		this.NotifyTokensChanged();
	}

	public bool UseTokens(int amount)
	{
		lock (this)
		{
			if (amount > this.tokens)
			{
				return false;
			}

			this.tokens -= amount;
			StoreInventory.TakeItem(GnomeStoreAssets.TokenId, amount);

			this.NotifyTokensChanged();
			return true;
		}
	}

	#endregion

	#region Methods

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}

		Instance = this;
		this.isFirstTime = true;

		DontDestroyOnLoad(this.gameObject);
	}

	private void InitStoreData()
	{
		VirtualCurrencies = StoreInfo.Currencies;
		VirtualGoods = StoreInfo.Goods;
		VirtualCurrencyPacks = StoreInfo.CurrencyPacks;
		VirtualGoodsDictionary = new Dictionary<string, VirtualGood>();

		foreach (VirtualGood virtualGood in VirtualGoods)
		{
			VirtualGoodsDictionary.Add(virtualGood.ItemId, virtualGood);
		}

		this.UpdateStoreBalances();

		Debug.Log("GNOME: Notifying Shop Data Initialized");
		this.NotifyShopDataInitialized();

		// Give 10 tokens for free if it's the first time playing
		if (PlayerPrefsFast.GetBool(SharedResources.FirstTimeUserKey, true))
		{
			StoreInventory.GiveItem(GnomeStoreAssets.TokenId, 10);
			PlayerPrefsFast.SetBool(SharedResources.FirstTimeUserKey, false);
			PlayerPrefsFast.Flush();
		}

		this.NotifyTokensChanged();
	}

	private void NotifyShopDataInitialized()
	{
		if (this.ShopDataInitializedEvent != null)
		{
			this.ShopDataInitializedEvent(this, new ShopDataInitializedEventArgs());
		}
	}

	private void NotifyTokensChanged()
	{
		foreach (UILabel label in this.CoinSubscribers)
		{
			label.text = this.tokens.ToString();
		}
	}

	private void OnBillingNotSupported()
	{
		Debug.Log("GNOME: OnBillingNotSupported");
	}

	private void OnBillingSupported()
	{
		Debug.Log("GNOME: OnBillingSupported");
	}

	private void OnCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded)
	{
		Debug.Log(string.Format("GNOME: CurrencyBalanceChanged - Balance: {0}", balance));
		this.tokens = balance;

		this.NotifyTokensChanged();
	}

	private void OnGoodBalanceChanged(VirtualGood vg, int balance, int amountAdded)
	{
		Debug.Log(string.Format("GNOME: VirtualGoodBalanceChanged - Id: {0}, Balance: {1}", vg.ItemId, balance));
		GoodsBalances[vg.ItemId] = balance;
	}

	private void OnGoodEquipped(EquippableVG good)
	{
		Debug.Log(string.Format("GNOME: GoodEquipped - ItemId: {0}", good.ItemId));
	}

	private void OnGoodUnequipped(EquippableVG good)
	{
		Debug.Log(string.Format("GNOME: GoodUnequipped - ItemId: {0}", good.ItemId));
	}

	private void OnGoodUpgrade(VirtualGood good, UpgradeVG currentUpgrade)
	{
		Debug.Log(string.Format("GNOME: GoodUpgrade - ItemId: {0}", good.ItemId));
	}

	private void OnItemPurchaseStarted(PurchasableVirtualItem pvi)
	{
		Debug.Log(string.Format("GNOME: ItemPurchaseStarted - ItemId: {0}", pvi.ItemId));
	}

	private void OnItemPurchased(PurchasableVirtualItem pvi, string paylod)
	{
		Debug.Log(string.Format("GNOME: ItemPurchased - ItemId: {0}", pvi.ItemId));

		// Set the item information
		ItemHitBuilder itemHitBuilder = new ItemHitBuilder().SetTransactionID(pvi.ID).SetName(pvi.Name).SetSKU(pvi.ItemId);

		// Then set the price
		PurchaseWithMarket marketItem = pvi.PurchaseType as PurchaseWithMarket;
		PurchaseWithVirtualItem virtualItem = pvi.PurchaseType as PurchaseWithVirtualItem;

		if (marketItem != null)
		{
			itemHitBuilder.SetPrice(marketItem.MarketItem.Price).SetCurrencyCode(marketItem.MarketItem.MarketCurrencyCode);
		}

		if (virtualItem != null)
		{
			itemHitBuilder.SetPrice(virtualItem.Amount).SetCurrencyCode("Tokens");
		}

		// Now log it
		GoogleAnalyticsV3.instance.LogItem(itemHitBuilder);

		Debug.Log(string.Format("GNOME: Analytics: {0}, {1}, {2}", "Purchase", this.PurchaseButtonLocation, pvi.ItemId));
	}

	private void OnLevelWasLoaded(int level)
	{
		// If it was the first time, the notification would have triggered in Start()
		if (!this.isFirstTime)
		{
			this.InitStoreData();
		}
	}

	private void OnMarketPurchase(PurchasableVirtualItem pvi, string purchaseToken, Dictionary<string, string> arg3)
	{
		Debug.Log(string.Format("GNOME: MarketPurchase - ItemId: {0}", pvi.ItemId));
	}

	private void OnMarketPurchaseCancelled(PurchasableVirtualItem pvi)
	{
		Debug.Log(string.Format("GNOME: MarketPurchaseCancelled - ItemId: {0}", pvi.ItemId));
	}

	private void OnMarketPurchaseStarted(PurchasableVirtualItem pvi)
	{
		Debug.Log(string.Format("GNOME: MarketPurchaseStarted - ItemId: {0}", pvi.ItemId));
	}

	private void OnMarketRefund(PurchasableVirtualItem pvi)
	{
		Debug.Log(string.Format("GNOME: PurchasableVirtualItem - ItemId: {0}", pvi.ItemId));
	}

	private void OnRestoreTransactionsFinished(bool success)
	{
		Debug.Log(string.Format("GNOME: RestoreTransactions - Success: {0}", success));
	}

	private void OnRestoreTransactionsStarted()
	{
		Debug.Log("GNOME: RestoreTransactionsStarted");
	}

	private void OnStoreControllerInitialized()
	{
		Debug.Log("GNOME: Initialized.  Calling InitStoreData");
		this.InitStoreData();
	}

	private void OnUnexpectedErrorInStore(string message)
	{
		Debug.Log(string.Format("GNOME: UnexpectedErrorInStore - Error: {0}", message));
	}

#if UNITY_ANDROID && !UNITY_EDITOR
	private void OnIabServiceStarted()
	{
		Debug.Log("GNOME: IabServiceStarted");
	}

	private void OnIabServiceStopped()
	{
		Debug.Log("GNOME: IabServiceStopped");
	}
#endif

	private void Start()
	{
		// Just in case we started it in the level already.  It doesn't call OnLevelWasLoaded
		// when we first start the game in the editor.
		this.OnLevelWasLoaded(Application.loadedLevel);

		if (this.isFirstTime)
		{
			Debug.Log("GNOME: Subscribing to events");
			this.SubscribeToStoreEvents();
			Debug.Log("GNOME: Initializing store");
			SoomlaStore.Initialize(new GnomeStoreAssets());

			this.isFirstTime = false;
		}

		this.NotifyTokensChanged();
	}

	private void SubscribeToStoreEvents()
	{
		StoreEvents.OnMarketPurchase += this.OnMarketPurchase;
		StoreEvents.OnMarketRefund += this.OnMarketRefund;
		StoreEvents.OnItemPurchased += this.OnItemPurchased;
		StoreEvents.OnGoodEquipped += this.OnGoodEquipped;
		StoreEvents.OnGoodUnEquipped += this.OnGoodUnequipped;
		StoreEvents.OnGoodUpgrade += this.OnGoodUpgrade;
		StoreEvents.OnBillingSupported += this.OnBillingSupported;
		StoreEvents.OnBillingNotSupported += this.OnBillingNotSupported;
		StoreEvents.OnMarketPurchaseStarted += this.OnMarketPurchaseStarted;
		StoreEvents.OnItemPurchaseStarted += this.OnItemPurchaseStarted;
		StoreEvents.OnUnexpectedErrorInStore += this.OnUnexpectedErrorInStore;
		StoreEvents.OnCurrencyBalanceChanged += this.OnCurrencyBalanceChanged;
		StoreEvents.OnGoodBalanceChanged += this.OnGoodBalanceChanged;
		StoreEvents.OnMarketPurchaseCancelled += this.OnMarketPurchaseCancelled;
		StoreEvents.OnRestoreTransactionsStarted += this.OnRestoreTransactionsStarted;
		StoreEvents.OnRestoreTransactionsFinished += this.OnRestoreTransactionsFinished;
		StoreEvents.OnSoomlaStoreInitialized += this.OnStoreControllerInitialized;
#if UNITY_ANDROID && !UNITY_EDITOR
		StoreEvents.OnIabServiceStarted += this.OnIabServiceStarted;
		StoreEvents.OnIabServiceStopped += this.OnIabServiceStopped;
#endif
	}

	private void UpdateStoreBalances()
	{
		Debug.Log("GNOME: Updating Store Balances");

		if (VirtualCurrencies.Count > 0)
		{
			this.tokens = StoreInventory.GetItemBalance(VirtualCurrencies[0].ItemId);
			this.NotifyTokensChanged();
			Debug.Log("GNOME: VirtualCurrencies: " + this.tokens);
		}
		else
		{
			Debug.Log("GNOME: No VirtualCurrencies");
		}

		if (VirtualGoods.Count > 0)
		{
			foreach (VirtualGood vg in VirtualGoods)
			{
				GoodsBalances[vg.ItemId] = StoreInventory.GetItemBalance(vg.ItemId);
				Debug.Log(string.Format("GNOME: VirtualGood - ID: {0}, Quantity: {1} ", vg.ItemId, GoodsBalances[vg.ItemId]));
			}
		}
		else
		{
			Debug.Log("GNOME: No VirtualGoods");
		}

		Debug.Log("GNOME: Initialize got past non consumable");

		if (VirtualCurrencyPacks.Count == 0)
		{
			Debug.Log("GNOME: No VirtualCurrencyPacks");
		}

		foreach (VirtualCurrencyPack vcp in VirtualCurrencyPacks)
		{
			Debug.Log(string.Format("GNOME: VirtualCurrencyPackId: {0}, CurrencyAmount: {1}", vcp.ItemId, vcp.CurrencyAmount));
		}
	}

	#endregion
}