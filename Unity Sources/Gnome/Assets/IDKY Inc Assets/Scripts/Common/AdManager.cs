// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;

using ChartboostSDK;

using UnityEngine;

public class AdManager : MonoBehaviour
{
	#region Static Fields

	public static AdManager Instance;

	public bool CacheMoreGamesOnStart = true;

	public bool CacheVideoOnStart = true;

	public GameObject[] EnableOnCachedMoreGames;

	public GameObject[] EnableOnCachedVideo;

	private static int CurrentInterval;

	#endregion

	#region Fields

	/// <summary>
	/// Number of requests to display before actually display the ad.
	/// </summary>
	public int AdDisplayInterval = 3;

	public int StartingCount = 2;

	#endregion

	#region Enums

	public enum AdNetwork
	{
		ChartBoost
	}

	public enum DisplayedAdType
	{
		FullScreen,

		MoreGames,

		Video
	}

	#endregion

	#region Public Methods and Operators

	public void CacheAd(AdNetwork network, DisplayedAdType displayType, CBLocation location)
	{

#if ADFREE
		// Don't cache if Ad-free
		return;
		
#endif

		if (Debug.isDebugBuild)
		{
			Debug.Log("Caching ad");
		}

		switch (displayType)
		{
			case DisplayedAdType.FullScreen:

#if !MOREAPPSONLY

				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						Chartboost.cacheInterstitial(location);
						break;
				}

#endif

				break;

			case DisplayedAdType.MoreGames:
				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						Chartboost.cacheMoreApps(location);
						break;
				}
				break;

			case DisplayedAdType.Video:
				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						Chartboost.cacheRewardedVideo(location);
						break;
				}
				break;
		}
	}

	public bool IsCached(AdNetwork network, DisplayedAdType displayType, CBLocation location)
	{
		bool isCached = false;

#if ADFREE
		
		// Just return false if Ad-free
		return false;
		
#endif

		switch (displayType)
		{
			case DisplayedAdType.FullScreen:

#if !MOREAPPSONLY

				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						isCached = Chartboost.hasInterstitial(location);
						break;
				}

#endif

				break;

			case DisplayedAdType.MoreGames:
				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						isCached = Chartboost.hasMoreApps(location);
						break;
				}
				break;

			case DisplayedAdType.Video:
				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						isCached = Chartboost.hasRewardedVideo(location);
						break;
				}
				break;
		}

		if (Debug.isDebugBuild)
		{
			Debug.Log("Ad is cached: " + isCached.ToString());
		}

		return isCached;
	}

	public bool IsImpressionVisible()
	{
		return Chartboost.isImpressionVisible();
	}

	public void ShowAd(AdNetwork network, DisplayedAdType displayType, CBLocation location)
	{

#if ADFREE
		
		// Don't show Ad if Ad-free
		return;
		
#endif

		CurrentInterval++;

		if (CurrentInterval % this.AdDisplayInterval != 0 && displayType == DisplayedAdType.FullScreen)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Can't display Ad yet: " + CurrentInterval.ToString());
			}

			// Don't display until every interval
			return;
		}

		if (Debug.isDebugBuild)
		{
			Debug.Log("Showing Ad");
		}

		switch (displayType)
		{
			case DisplayedAdType.FullScreen:

#if !MOREAPPSONLY

				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						Chartboost.showInterstitial(location);
						break;
				}

#endif

				break;

			case DisplayedAdType.MoreGames:
				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						Chartboost.showMoreApps(location);
						break;
				}
				break;

			case DisplayedAdType.Video:
				switch (network)
				{
					case AdNetwork.ChartBoost:
					default:
						Chartboost.showRewardedVideo(location);
						break;
				}
				break;
		}
	}

	#endregion

	#region Methods

	public AdNetwork StartUpAdNetwork = AdNetwork.ChartBoost;

	private void Start()
	{
		if (this.CacheMoreGamesOnStart)
		{
			this.CacheAd(StartUpAdNetwork, DisplayedAdType.MoreGames, CBLocation.HomeScreen);
		}

		if (this.CacheVideoOnStart)
		{
			this.CacheAd(StartUpAdNetwork, DisplayedAdType.Video, CBLocation.HomeScreen);
		}
	}


	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
		}

		Instance = this;

		// Set initial count
		CurrentInterval = this.StartingCount;

		DontDestroyOnLoad(this.gameObject);
	}

	private void ChartboostOnDidCacheInPlay(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidCacheInterstitial(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidCacheMoreApps(CBLocation cbLocation)
	{
		foreach (GameObject obj in this.EnableOnCachedMoreGames)
		{
			obj.SetActive(true);
		}
	}

	private void ChartboostOnDidCacheRewardedVideo(CBLocation cbLocation)
	{
		foreach (GameObject obj in this.EnableOnCachedVideo)
		{
			obj.SetActive(true);
		}
	}

	private void ChartboostOnDidClickInterstitial(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidClickMoreApps(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidClickRewardedVideo(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidCloseInterstitial(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidCloseMoreApps(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidCloseRewardedVideo(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidCompleteAppStoreSheetFlow()
	{
	}

	private void ChartboostOnDidCompleteRewardedVideo(CBLocation cbLocation, int i)
	{
		ShopManager.Instance.GiveTokens(i);
	}

	private void ChartboostOnDidDismissInterstitial(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidDismissMoreApps(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidDismissRewardedVideo(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidDisplayInterstitial(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidDisplayMoreApps(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidDisplayRewardedVideo(CBLocation cbLocation)
	{
	}

	private void ChartboostOnDidFailToLoadInPlay(CBLocation cbLocation, CBImpressionError cbImpressionError)
	{
	}

	private void ChartboostOnDidFailToLoadInterstitial(CBLocation cbLocation, CBImpressionError cbImpressionError)
	{
	}

	private void ChartboostOnDidFailToLoadMoreApps(CBLocation cbLocation, CBImpressionError cbImpressionError)
	{
	}

	private void ChartboostOnDidFailToLoadRewardedVideo(CBLocation cbLocation, CBImpressionError cbImpressionError)
	{
	}

	private void ChartboostOnDidFailToRecordClick(CBLocation cbLocation, CBImpressionError cbImpressionError)
	{
	}

	private void ChartboostOnDidPauseClickForConfirmation()
	{
	}

	private bool ChartboostOnShouldDisplayInterstitial(CBLocation cbLocation)
	{
		return true;
	}

	private bool ChartboostOnShouldDisplayMoreApps(CBLocation cbLocation)
	{
		return true;
	}

	private bool ChartboostOnShouldDisplayRewardedVideo(CBLocation cbLocation)
	{
		return true;
	}

	private void OnDisable()
	{
		// Listen to all impression-related events
		Chartboost.didFailToLoadInterstitial -= this.ChartboostOnDidFailToLoadInterstitial;
		Chartboost.didDismissInterstitial -= this.ChartboostOnDidDismissInterstitial;
		Chartboost.didCloseInterstitial -= this.ChartboostOnDidCloseInterstitial;
		Chartboost.didClickInterstitial -= this.ChartboostOnDidClickInterstitial;
		Chartboost.didCacheInterstitial -= this.ChartboostOnDidCacheInterstitial;
		Chartboost.shouldDisplayInterstitial -= this.ChartboostOnShouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial -= this.ChartboostOnDidDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps -= this.ChartboostOnDidFailToLoadMoreApps;
		Chartboost.didDismissMoreApps -= this.ChartboostOnDidDismissMoreApps;
		Chartboost.didCloseMoreApps -= this.ChartboostOnDidCloseMoreApps;
		Chartboost.didClickMoreApps -= this.ChartboostOnDidClickMoreApps;
		Chartboost.didCacheMoreApps -= this.ChartboostOnDidCacheMoreApps;
		Chartboost.shouldDisplayMoreApps -= this.ChartboostOnShouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps -= this.ChartboostOnDidDisplayMoreApps;
		Chartboost.didFailToRecordClick -= this.ChartboostOnDidFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo -= this.ChartboostOnDidFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo -= this.ChartboostOnDidDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo -= this.ChartboostOnDidCloseRewardedVideo;
		Chartboost.didClickRewardedVideo -= this.ChartboostOnDidClickRewardedVideo;
		Chartboost.didCacheRewardedVideo -= this.ChartboostOnDidCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo -= this.ChartboostOnShouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo -= this.ChartboostOnDidCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo -= this.ChartboostOnDidDisplayRewardedVideo;
		Chartboost.didCacheInPlay -= this.ChartboostOnDidCacheInPlay;
		Chartboost.didFailToLoadInPlay -= this.ChartboostOnDidFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation -= this.ChartboostOnDidPauseClickForConfirmation;
#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow -= this.ChartboostOnDidCompleteAppStoreSheetFlow;
#endif
	}

	private void OnEnable()
	{
		// Listen to all impression-related events
		Chartboost.didFailToLoadInterstitial += this.ChartboostOnDidFailToLoadInterstitial;
		Chartboost.didDismissInterstitial += this.ChartboostOnDidDismissInterstitial;
		Chartboost.didCloseInterstitial += this.ChartboostOnDidCloseInterstitial;
		Chartboost.didClickInterstitial += this.ChartboostOnDidClickInterstitial;
		Chartboost.didCacheInterstitial += this.ChartboostOnDidCacheInterstitial;
		Chartboost.shouldDisplayInterstitial += this.ChartboostOnShouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial += this.ChartboostOnDidDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps += this.ChartboostOnDidFailToLoadMoreApps;
		Chartboost.didDismissMoreApps += this.ChartboostOnDidDismissMoreApps;
		Chartboost.didCloseMoreApps += this.ChartboostOnDidCloseMoreApps;
		Chartboost.didClickMoreApps += this.ChartboostOnDidClickMoreApps;
		Chartboost.didCacheMoreApps += this.ChartboostOnDidCacheMoreApps;
		Chartboost.shouldDisplayMoreApps += this.ChartboostOnShouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps += this.ChartboostOnDidDisplayMoreApps;
		Chartboost.didFailToRecordClick += this.ChartboostOnDidFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo += this.ChartboostOnDidFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo += this.ChartboostOnDidDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo += this.ChartboostOnDidCloseRewardedVideo;
		Chartboost.didClickRewardedVideo += this.ChartboostOnDidClickRewardedVideo;
		Chartboost.didCacheRewardedVideo += this.ChartboostOnDidCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo += this.ChartboostOnShouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo += this.ChartboostOnDidCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo += this.ChartboostOnDidDisplayRewardedVideo;
		Chartboost.didCacheInPlay += this.ChartboostOnDidCacheInPlay;
		Chartboost.didFailToLoadInPlay += this.ChartboostOnDidFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation += this.ChartboostOnDidPauseClickForConfirmation;
#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow += this.ChartboostOnDidCompleteAppStoreSheetFlow;
#endif
	}

	#endregion
}