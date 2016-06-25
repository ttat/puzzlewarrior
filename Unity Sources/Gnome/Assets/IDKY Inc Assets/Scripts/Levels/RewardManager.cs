// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using Idky;

using UnityEngine;

public class RewardManager : MonoBehaviour
{
    #region Fields

    public string LastDateRewardKey = "LastDateReward";

    public string RewardIndexKey = "RewardIndex";

    public UIScrollView RewardScrollViewer;

    public UIGrid RewardsGrid;

    private UICenterOnChild centerOnChildTarget;

    private bool refreshGrid;

    #endregion

    #region Methods

    private IEnumerator RefreshGrid()
    {
        yield return new WaitForSeconds(.01f);

        this.RewardsGrid.repositionNow = true;

        this.RewardScrollViewer.ResetPosition();

        yield return new WaitForSeconds(0.01f);

        if (this.centerOnChildTarget != null)
        {
            this.centerOnChildTarget.enabled = true;

            yield return new WaitForSeconds(0.01f);

            this.centerOnChildTarget.enabled = false;
        }
    }

    private void Start()
    {
        // Get the current prize index
        int index = PlayerPrefsFast.GetInt(this.RewardIndexKey, 0);

        // Get the last time a reward was given
        string lastDateString = PlayerPrefsFast.GetString(this.LastDateRewardKey, string.Empty);
        DateTime lastDate;
        bool parsed = DateTime.TryParse(lastDateString, out lastDate);

        if (parsed)
        {
            // Give reward after 1 day
            if (lastDate.AddDays(1) <= DateTime.Now)
            {
                // Give reward, and then set it to the next index
                index++;
                List<Transform> childRewards = this.RewardsGrid.GetChildList();

                // Make sure we don't give more rewards that we hvae set up
                if (index <= childRewards.Count)
                {
                    foreach (Transform child in childRewards)
                    {
                        DailyReward dailyReward = child.GetComponent<DailyReward>();

                        if (dailyReward != null)
                        {
                            dailyReward.SetCurrentReward(index);

                            if (dailyReward.Day == index)
                            {
                                this.centerOnChildTarget = child.GetComponent<UICenterOnChild>();
                            }
                        }
                    }

                    PlayerPrefsFast.SetInt(this.RewardIndexKey, index);
                    PlayerPrefsFast.SetString(this.LastDateRewardKey, DateTime.Now.ToString());
                    PlayerPrefsFast.Flush();

                    this.RewardsGrid.enabled = true;
                    this.RewardsGrid.repositionNow = true;
                    this.refreshGrid = true;
                }
                else
                {
                   this.gameObject.SetActive(false); 
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            // It's the first time, so set the date as now
            PlayerPrefsFast.SetString(this.LastDateRewardKey, DateTime.Now.ToString());
            PlayerPrefsFast.Flush();

            this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (this.refreshGrid)
        {
            this.refreshGrid = false;

            this.StartCoroutine(this.RefreshGrid());
        }
    }

    #endregion
}