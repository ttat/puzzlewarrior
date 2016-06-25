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

public class QuestDialog : MonoBehaviour
{
    #region Fields

    public GameObject CompletedImage;

    public IdkyKeyValuePairGameObject[] SpecialStageMappings;

    public GameObject[] StageMapping;

    #endregion

    #region Public Methods and Operators

    public void SetEndDialog(int level, int levelsPerStage, bool isSpecialMode, SpecialStage specialStage)
    {
        if (isSpecialMode)
        {
            this.SetEndDialogSpecial(specialStage);
        }
        else
        {
            this.SetEndDialogNormal(level, levelsPerStage);
        }
    }

    [ContextMenu("Initialize Special Stage Mappings")]
    public void SetSpecialStageInitialMappings()
    {
        VirtualGood[] specialStages = GnomeStoreAssets.GetGoodsStatic();

        // Initialize mappings
        List<IdkyKeyValuePairGameObject> backgrounds = this.SpecialStageMappings != null
                                                           ? new List<IdkyKeyValuePairGameObject>(this.SpecialStageMappings)
                                                           : new List<IdkyKeyValuePairGameObject>();

        if (this.SpecialStageMappings == null)
        {
            this.SpecialStageMappings = new IdkyKeyValuePairGameObject[specialStages.Length];

            foreach (VirtualGood specialStage in specialStages)
            {
                backgrounds.Add(new IdkyKeyValuePairGameObject { Key = specialStage.ItemId });
            }
        }
        else
        {
            foreach (VirtualGood specialStage in specialStages)
            {
                IIdkyKeyValuePair<string, GameObject> idkyKeyValuePair = this.SpecialStageMappings.GetPair(specialStage.ItemId);

                if (idkyKeyValuePair == null)
                {
                    backgrounds.Add(new IdkyKeyValuePairGameObject { Key = specialStage.ItemId });
                }
            }
        }

        this.SpecialStageMappings = backgrounds.ToArray();
    }

    public void SetStartDialog(int level, int levelsPerStage, bool isSpecialMode, SpecialStage specialStage)
    {
        if (isSpecialMode)
        {
            this.SetStartDialogSpecial(specialStage);
        }
        else
        {
            this.SetStartDialogNormal(level, levelsPerStage);
        }
    }

    #endregion

    #region Methods

    private void SetEndDialogNormal(int level, int levelsPerStage)
    {
        // Show the Quest Dialog if this is the first time beating the last level of a stage
        int remainder;
        int stageIndex = Math.DivRem(level, levelsPerStage, out remainder);
        stageIndex -= remainder == 0 ? 1 : 0;

        if (remainder == 0)
        {
            this.CompletedImage.SetActive(true);

            this.gameObject.SetActive(true);

            for (int i = 0; i < this.StageMapping.Length; i++)
            {
                this.StageMapping[i].SetActive(i == stageIndex);
            }

            foreach (IdkyKeyValuePairGameObject specialStageMapping in this.SpecialStageMappings)
            {
                specialStageMapping.GetValue().SetActive(false);
            }
        }
    }

    private void SetEndDialogSpecial(SpecialStage specialStage)
    {
        // Show the Quest Dialog if all the special stages are beaten
        string key = string.Format("{0}_{1}", SharedResources.SpecialStageEndQuestDialogShownKey, specialStage.StageId);

        bool shown = PlayerPrefsFast.GetBool(key, false);

        if (!shown)
        {
            bool hasLevelIncomplete = false;

            foreach (int levelInStage in specialStage.LevelMapping.Keys)
            {
                string levelKey = string.Format(
                    "{0}_{1}_{2}", SharedResources.TimesLevelPlayedToCompletePrefix, specialStage.StageId, levelInStage);

                int timesToCompleteLevel = PlayerPrefsFast.GetInt(levelKey, 0);

                // Check if the level has been completed yet
                if (timesToCompleteLevel == 0)
                {
                    hasLevelIncomplete = true;
                    break;
                }
            }

            if (!hasLevelIncomplete)
            {
                this.CompletedImage.SetActive(true);
                this.gameObject.SetActive(true);

                foreach (GameObject dialog in this.StageMapping)
                {
                    dialog.SetActive(false);
                }

                foreach (IdkyKeyValuePairGameObject specialStageMapping in this.SpecialStageMappings)
                {
                    specialStageMapping.GetValue().SetActive(specialStageMapping.GetKey().Equals(specialStage.StageId));
                }

                PlayerPrefsFast.SetBool(key, true);
                PlayerPrefsFast.Flush();
            }
        }
    }

    private void SetStartDialogNormal(int level, int levelsPerStage)
    {
        // Show the Quest Dialog if this is the first time playing the first level of a stage
        int remainder;
        int stageIndex = Math.DivRem(level, levelsPerStage, out remainder);
        stageIndex -= remainder == 0 ? 1 : 0;

        if (remainder == 1)
        {
            this.CompletedImage.SetActive(false);

            string key = string.Format("{0}_{1}", SharedResources.TimesLevelPlayedPrefix, level);
            int timesPlayed = PlayerPrefsFast.GetInt(key, 0);
            this.gameObject.SetActive(timesPlayed == 0);

            for (int i = 0; i < this.StageMapping.Length; i++)
            {
                this.StageMapping[i].SetActive(i == stageIndex);
            }

            foreach (IdkyKeyValuePairGameObject specialStageMapping in this.SpecialStageMappings)
            {
                specialStageMapping.GetValue().SetActive(false);
            }
        }
    }

    private void SetStartDialogSpecial(SpecialStage specialStage)
    {
        // Show the Quest Dialog if this is the first time playing the a level in the special stage
        string key = string.Format("{0}_{1}", SharedResources.SpecialStageStartQuestDialogShownKey, specialStage.StageId);

        bool shown = PlayerPrefsFast.GetBool(key, false);

        if (!shown)
        {
            this.CompletedImage.SetActive(false);
            this.gameObject.SetActive(true);

            foreach (GameObject dialog in this.StageMapping)
            {
                dialog.SetActive(false);
            }

            foreach (IdkyKeyValuePairGameObject specialStageMapping in this.SpecialStageMappings)
            {
                specialStageMapping.GetValue().SetActive(specialStageMapping.GetKey().Equals(specialStage.StageId));
            }

            PlayerPrefsFast.SetBool(key, true);
            PlayerPrefsFast.Flush();
        }
    }

    #endregion
}