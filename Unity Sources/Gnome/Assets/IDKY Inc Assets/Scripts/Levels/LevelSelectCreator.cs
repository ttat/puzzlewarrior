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

using Soomla.Store;

using UnityEngine;

public class LevelSelectCreator : MonoBehaviour
{
    #region Fields

    public GameObject GameBoard;

    public GameTable GameTable;

    public GameObject[] HideOnEnable;

    public GameObject LevelSelectButton;

    public UITable LevelSelectTable;

    public UIWidget LevelSelectTableLimits;

    public int LevelsPerStage = 12;

    public int NumberOfColumns = 3;

    public float Padding = 10;

    public PurchaseStage PurchaseStagePopUp;

    public GameObject RewardOnlyStagePopUp;

    public float SelectLevelVerticalOffset = 0f;

    public GameObject[] ShowOnEnable;

    public GameObject SpecialStageLevelSelectButton;

    public GameObject SpecialStageSelectButton;

    public GameObject StageSelectButton;

    public int StageSelectButtonTrimSize = 200;

    public UIGrid StageSelectGrid;

    public GameObject StageSelectOverrideActivateTarget;

    public GameObject StageSelectOverrideDeactivateTarget;

    public GameObject StageSelectScrollViewer;

    private LevelMapping levelMapping;

    private bool refreshGrid;

    #endregion

    #region Public Methods and Operators

    public void SelectStage(SpecialStage specialStage)
    {
        this.GameTable.SpecialStage = specialStage;
        this.RemoveTableChildren();
        this.LevelSelectTable.Reposition();

        // Fudge Factor so it doesn't use up the whole space, just most of it
        const float FudgeFactor = .9f;

        // Set the number of columns
        int rows = specialStage.LevelMapping.Mapping.Count / this.NumberOfColumns;
        this.LevelSelectTable.columns = this.NumberOfColumns;

        // Set the padding
        this.LevelSelectTable.padding = new Vector2(this.Padding, this.Padding);

        // Calculate what is available for width and height
        float columnPadding = ((this.NumberOfColumns + 1) * this.Padding);
        float rowPadding = ((rows + 1) * this.Padding);
        float availableWidth = (this.LevelSelectTableLimits.localSize.x * FudgeFactor) - columnPadding;
        float availableHeight = (this.LevelSelectTableLimits.localSize.y * FudgeFactor) - rowPadding;

        // Choose a button size based on the smaller of the two
        float width = availableWidth / this.NumberOfColumns;
        float height = availableHeight / rows;
        float buttonSize = Math.Min(width, height);

        for (int i = 1; i <= specialStage.LevelMapping.Mapping.Count; i++)
        {
            GameObject item = NGUITools.AddChild(this.LevelSelectTable.gameObject, this.SpecialStageLevelSelectButton);

            UIWidget uiWidget = item.GetComponent<UIWidget>();

            if (uiWidget != null)
            {
                uiWidget.SetDimensions((int)buttonSize, (int)buttonSize);
            }

            SpecialStageLevelSelectButton levelSelectButton = item.GetComponent<SpecialStageLevelSelectButton>();

            if (levelSelectButton != null)
            {
                levelSelectButton.SetLevel(this.GameTable, specialStage.StageId, i);

                foreach (ActivateOnClick levelSelectActivateOnClick in levelSelectButton.ActivateOnClick)
                {
                    if (levelSelectActivateOnClick != null)
                    {
                        levelSelectActivateOnClick.ActivateTarget = this.GameBoard;
                        levelSelectActivateOnClick.DeactivateTarget = this.gameObject;
                    }
                }
            }

            this.LevelSelectTable.children.Add(item.transform);
        }

        // Center the table
        float heightOffset = (rowPadding + this.Padding + (buttonSize * rows)) / 2;
        float widthOffset = -(columnPadding + this.Padding + (buttonSize * this.NumberOfColumns)) / 2;
        this.LevelSelectTable.transform.localPosition = new Vector3(widthOffset, heightOffset + this.SelectLevelVerticalOffset);

        this.LevelSelectTable.enabled = true;
        this.LevelSelectTable.repositionNow = true;
    }

    public void SelectStage(int stageNumber)
    {
        this.RemoveTableChildren();
        this.LevelSelectTable.Reposition();

        // Fudge Factor so it doesn't use up the whole space, just most of it
        const float FudgeFactor = .9f;

        // Set the number of columns
        int rows = this.LevelsPerStage / this.NumberOfColumns;
        this.LevelSelectTable.columns = this.NumberOfColumns;

        // Set the padding
        this.LevelSelectTable.padding = new Vector2(this.Padding, this.Padding);

        // Calculate what is available for width and height
        float columnPadding = ((this.NumberOfColumns + 1) * this.Padding);
        float rowPadding = ((rows + 1) * this.Padding);
        float availableWidth = (this.LevelSelectTableLimits.localSize.x * FudgeFactor) - columnPadding;
        float availableHeight = (this.LevelSelectTableLimits.localSize.y * FudgeFactor) - rowPadding;

        // Choose a button size based on the smaller of the two
        float width = availableWidth / this.NumberOfColumns;
        float height = availableHeight / rows;
        float buttonSize = Math.Min(width, height);

        for (int i = 1; i <= this.LevelsPerStage; i++)
        {
            GameObject item = NGUITools.AddChild(this.LevelSelectTable.gameObject, this.LevelSelectButton);

            UIWidget uiWidget = item.GetComponent<UIWidget>();

            if (uiWidget != null)
            {
                uiWidget.SetDimensions((int)buttonSize, (int)buttonSize);
            }

            LevelSelectButton levelSelectButton = item.GetComponent<LevelSelectButton>();

            if (levelSelectButton != null)
            {
                int level = ((stageNumber - 1) * this.LevelsPerStage) + i;
                levelSelectButton.SetLevel(this.GameTable, level);

                foreach (ActivateOnClick levelSelectActivateOnClick in levelSelectButton.ActivateOnClick)
                {
                    if (levelSelectActivateOnClick != null)
                    {
                        levelSelectActivateOnClick.ActivateTarget = this.GameBoard;
                        levelSelectActivateOnClick.DeactivateTarget = this.gameObject;
                    }
                }
            }

            this.LevelSelectTable.children.Add(item.transform);
        }

        // Center the table
        float heightOffset = (rowPadding + this.Padding + (buttonSize * rows)) / 2;
        float widthOffset = -(columnPadding + this.Padding + (buttonSize * this.NumberOfColumns)) / 2;
        this.LevelSelectTable.transform.localPosition = new Vector3(widthOffset, heightOffset + this.SelectLevelVerticalOffset);

        this.LevelSelectTable.enabled = true;
        this.LevelSelectTable.repositionNow = true;
    }

    #endregion

    #region Methods

    private void Awake()
    {
        this.levelMapping = LevelMappingXmlIo.ReadLevelMapping();
    }

    private void CreateSpecialStageButtons()
    {
        UIPanel uiPanel = this.StageSelectScrollViewer.GetComponent<UIPanel>();
        Vector2 viewSize = uiPanel.GetViewSize();
        int buttonWidth = (int)viewSize.x - this.StageSelectButtonTrimSize;

        foreach (VirtualGood virtualGood in ShopManager.VirtualGoods)
        {
            SpecialStage readSpecialStage = SpecialStageXmlIo.ReadSpecialStage(virtualGood.ItemId);

            GameObject item = NGUITools.AddChild(this.StageSelectGrid.gameObject, this.SpecialStageSelectButton);

            SpecialStageSelectButton stageSelectButton = item.GetComponent<SpecialStageSelectButton>();

            if (stageSelectButton != null)
            {
                stageSelectButton.SetStage(
                    this,
                    readSpecialStage,
                    this.StageSelectOverrideActivateTarget,
                    this.StageSelectOverrideDeactivateTarget,
                    this.PurchaseStagePopUp,
                    this.RewardOnlyStagePopUp);
            }

            UIWidget uiWidget = item.GetComponent<UIWidget>();

            if (uiWidget != null)
            {
                uiWidget.SetDimensions(buttonWidth, uiWidget.height);
            }
        }

        this.StageSelectGrid.enabled = true;
        this.StageSelectGrid.repositionNow = true;

        UIScrollView scrollView = this.StageSelectScrollViewer.GetComponent<UIScrollView>();
        scrollView.enabled = true;
        scrollView.ResetPosition();

        this.refreshGrid = true;
    }

    private void CreateStageButtons()
    {
        foreach (Transform childTransform in this.StageSelectGrid.transform)
        {
            Destroy(childTransform.gameObject);
        }

        int totalLevels = this.levelMapping.Mapping.Count;

        int remainder;
        int numStages = Math.DivRem(totalLevels, this.LevelsPerStage, out remainder);

        if (remainder > 0)
        {
            numStages++;
        }

        UIPanel uiPanel = this.StageSelectScrollViewer.GetComponent<UIPanel>();
        Vector2 viewSize = uiPanel.GetViewSize();
        int buttonWidth = (int)viewSize.x - this.StageSelectButtonTrimSize;

        for (int i = 1; i <= numStages; i++)
        {
            GameObject item = NGUITools.AddChild(this.StageSelectGrid.gameObject, this.StageSelectButton);

            StageSelectButton stageSelectButton = item.GetComponent<StageSelectButton>();

            if (stageSelectButton != null)
            {
                stageSelectButton.SetStage(this, i, this.StageSelectOverrideActivateTarget, this.StageSelectOverrideDeactivateTarget);
            }

            UIWidget uiWidget = item.GetComponent<UIWidget>();

            if (uiWidget != null)
            {
                uiWidget.SetDimensions(buttonWidth, uiWidget.height);
            }
        }

        this.StageSelectGrid.enabled = true;
        this.StageSelectGrid.repositionNow = true;

        UIScrollView scrollView = this.StageSelectScrollViewer.GetComponent<UIScrollView>();
        scrollView.enabled = true;
        scrollView.ResetPosition();

        this.refreshGrid = true;
    }

    private void OnEnable()
    {
        foreach (GameObject hide in this.HideOnEnable)
        {
            hide.SetActive(false);
        }

        foreach (GameObject show in this.ShowOnEnable)
        {
            show.SetActive(true);
        }
    }

    private IEnumerator RefreshGrid()
    {
        yield return new WaitForSeconds(.01f);

        this.StageSelectGrid.repositionNow = true;
    }

    private void RemoveTableChildren()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in this.LevelSelectTable.children)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            NGUITools.Destroy(child.gameObject);
        }
    }

    private void Start()
    {
        this.CreateStageButtons();
        this.CreateSpecialStageButtons();
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