// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class DailyReward : MonoBehaviour
{
    #region Fields

    public GameObject Awarded;

    public GameObject Awarding;

    public int Day;

    public UILabel[] DayTextLabels;

    public GameObject NotAwarded;

    public RewardType Reward;

    public int RewardCount;

    public UILabel[] RewardTextLabels;

    public string StageId;

    #endregion

    #region Enums

    public enum RewardType
    {
        Token,

        Stage
    }

    #endregion

    #region Public Methods and Operators

    public void SetCurrentReward(int day)
    {
        Debug.Log("Setting reward day " + day);
        this.NotAwarded.SetActive(day < this.Day);
        this.Awarding.SetActive(day == this.Day);
        this.Awarded.SetActive(day > this.Day);

        // Give reward
        if (day == this.Day)
        {
            // Log that a reward was given
            GoogleAnalyticsV3.instance.LogEvent(
                new EventHitBuilder().SetEventCategory("Reward").SetEventAction("Received Reward").SetEventLabel("Day " + day));

            switch (this.Reward)
            {
                case RewardType.Token:
                    ShopManager.Instance.GiveTokens(this.RewardCount);
                    break;

                case RewardType.Stage:
                    ShopManager.Instance.GiveStage(this.StageId);
                    break;
            }
        }
    }

    #endregion

    #region Methods

    private void Start()
    {
        foreach (UILabel dayTextLabel in this.DayTextLabels)
        {
            dayTextLabel.text = string.Format("Day {0}", this.Day);
        }

        foreach (UILabel rewardTextLabel in this.RewardTextLabels)
        {
            switch (this.Reward)
            {
                case RewardType.Token:
                    rewardTextLabel.text = string.Format("{0} Tokens", this.RewardCount);
                    break;

                case RewardType.Stage:
                    SpecialStage rewardStage = SpecialStageXmlIo.ReadSpecialStage(this.StageId);
                    rewardTextLabel.text = rewardStage.Description;
                    break;
            }
        }
    }

    #endregion
}