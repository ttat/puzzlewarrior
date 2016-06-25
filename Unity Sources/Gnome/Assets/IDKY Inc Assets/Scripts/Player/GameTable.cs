// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ChartboostSDK;

using Idky;

using Soomla.Store;

using UnityEngine;

public class GameTable : MonoBehaviour
{
	#region Fields

	public GameBlockAnimationMapping[] AnimationMappings;

	public int BlockSize = 90;

	public UILabel ClearedLevelText;

	public UILabel CurrentLevelText;

	public GameObject[] DisableOnStart;

	public int FastCompleteReward = 2;

	public float FastCompleteTime = 5f;

	public GameObject FirstTimeIndicator;

	public GameBoard GameBoard;

	public GamePiece GamePiece;

	public int Level = 1;

	public LevelSelectCreator LevelSelectCreator;

	public int LevelsCompleteBeforeShowingRateUs = 5;

	public int MaxBlockSize = 120;

	public float MinimumTimeBetweenAnimations = 0.25f;

	public AudioClip[] MusicMappings;

	public GameObject NotEnoughTokensPopUp;

	public GameObject ParentGameBoard;

	public UIPanel ParentPanel;

	public int PlayedTooManyTimes = 10;

	public GameObject PlayedTooManyTimesSounds;

	public QuestDialog QuestDialog;

	public string RateOptionKey = "RateOption";

	public GameObject RateUsDialog;

	public UI2DSpriteAnimationIdky[] ResetAnimationsOnLevelStart;

	public GameObject RestartLevelButton;

	public GameObject ResultsScreen;

	public GameObject ResultsScreenFreeToken;

	public GameObject ResultsScreenNextButton;

	public UITweener[] ResultsScreenTweeners;

	public GameObject SkipLevelButton;

	public int SkipLevelCost = 10;

	public SpecialStage SpecialStage;

	public IdkyKeyValuePairGameObject[] SpecialStageBackgroundMappings;

	public IdkyKeyValuePairAudioClip[] SpecialStageMusicMappings;

	public GameObject[] StageBackgroundMappings;

	public UITable Table;

	public UIWidget TableLimits;

	public float TimeBeforeShowingResults = 1.5f;

	public TutorialAnimation[] TutorialAnimations;

	public TutorialMessage[] Tutorials;

	public int TutorialSize = 120;

	public UILabel TutorialTipStripMessage;

	public GameObject TutorialTipStripObject;

	public GameObject UndoButton;

	public UILabel UseTokensMessage;

	public GameObject UseTokensPopUp;

	/// <summary>
	/// The number of pixels to offset after repositioning the table.
	/// </summary>
	public float VerticalOffset = -30f;

	private Dictionary<BlockAnimationType, GameObject> animationMappingsDictionary;

	private GameObject currentBackground;

	private string currentSpecialStageId;

	private int currentStageIndex;

	private float endLevelTime;

	private bool firstMove;

	private bool goToNextLevel;

	private float lastAnimationTime;

	private LevelMapping levelMapping;

	private int levelsCompleted;

	private Stack<BlockMovement> movesHistory; 

	private GamePiece selectedGamePiece;

	private Queue<Action> setAnimationQueue = new Queue<Action>();

	private bool shownRateUsDialog;

	private bool specialStageMode;

	private float startLevelTime;

	private bool usedUndo;

	#endregion

	#region Public Methods and Operators

	public void GameBlockSelected(object obj)
	{
		GamePiece gamePiece = obj as GamePiece;

		if (gamePiece != null)
		{
			this.selectedGamePiece = gamePiece;
		}
	}

	public void GameBlockUnselected(object obj)
	{
		this.StartCoroutine("DelayedUnselectBlock", obj);
	}

	public void LoadGameBoard()
	{
#if UNITY_ANDROID
		// Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
		if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
		{
			return;
		}
#endif

		if (AdManager.Instance != null)
		{
			AdManager.Instance.ShowAd(AdManager.AdNetwork.ChartBoost, AdManager.DisplayedAdType.FullScreen, CBLocation.LevelStart);
		}

		foreach (GameObject o in this.DisableOnStart)
		{
			o.SetActive(false);
		}

		this.RemoveTableChildren();

		this.LoadPuzzleIntoTable();

		if (this.specialStageMode)
		{
			this.LoadSpecialStageBackground();
		}
		else
		{
			this.LoadBackground();
		}

		this.QuestDialog.SetStartDialog(this.Level, this.LevelSelectCreator.LevelsPerStage, this.specialStageMode, this.SpecialStage);

		this.RestartLevelButton.SetActive(true);
		this.SkipLevelButton.SetActive(true);
		this.UndoButton.SetActive(true);
		this.usedUndo = false;

		foreach (UI2DSpriteAnimationIdky spriteAnimation in ResetAnimationsOnLevelStart)
		{
			spriteAnimation.RestartAnimation();
		}
	}

	/// <summary>
	/// Loads the game board while in edit mode.
	/// </summary>
	[ContextMenu("Load Game Board")]
	public void LoadGameBoardEditMode()
	{
		this.Awake();

		this.LoadPuzzleIntoTable();
	}

	[ContextMenu("Load Next Level")]
	public void NextLevel()
	{
		this.Level++;
		this.LoadGameBoard();
	}

	public void OnQueueAnimation(object obj)
	{
		Action action = (Action)obj;
		this.setAnimationQueue.Enqueue(action);
	}

	public void OnSwipeCanceled()
	{
		this.CanceledMoved();
	}

	public void OnSwipeDown()
	{
		this.ApplyMove(MovementDirection.Down);
	}

	public void OnSwipeLeft()
	{
		this.ApplyMove(MovementDirection.Left);
	}

	public void OnSwipeRight()
	{
		this.ApplyMove(MovementDirection.Right);
	}

	public void OnSwipeUp()
	{
		this.ApplyMove(MovementDirection.Up);
	}

	public void OnSwipingDown(object pixels)
	{
		this.ApplyMoving(MovementDirection.Down, (float)pixels);
	}

	public void OnSwipingLeft(object pixels)
	{
		this.ApplyMoving(MovementDirection.Left, (float)pixels);
	}

	public void OnSwipingRight(object pixels)
	{
		this.ApplyMoving(MovementDirection.Right, (float)pixels);
	}

	public void OnSwipingUp(object pixels)
	{
		this.ApplyMoving(MovementDirection.Up, (float)pixels);
	}

	[ContextMenu("Load Previous Level")]
	public void PreviousLevel()
	{
		this.Level--;
		this.LoadGameBoard();
	}

	[ContextMenu("Initialize Special Stage Mappings")]
	public void SetSpecialStageInitialMappings()
	{
		VirtualGood[] specialStages = GnomeStoreAssets.GetGoodsStatic();

		// Initialize backgrounds
		List<IdkyKeyValuePairGameObject> backgrounds = this.SpecialStageBackgroundMappings != null
														   ? new List<IdkyKeyValuePairGameObject>(this.SpecialStageBackgroundMappings)
														   : new List<IdkyKeyValuePairGameObject>();

		if (this.SpecialStageBackgroundMappings == null)
		{
			this.SpecialStageBackgroundMappings = new IdkyKeyValuePairGameObject[specialStages.Length];

			foreach (VirtualGood specialStage in specialStages)
			{
				backgrounds.Add(new IdkyKeyValuePairGameObject { Key = specialStage.ItemId });
			}
		}
		else
		{
			foreach (VirtualGood specialStage in specialStages)
			{
				IIdkyKeyValuePair<string, GameObject> idkyKeyValuePair = this.SpecialStageBackgroundMappings.GetPair(specialStage.ItemId);

				if (idkyKeyValuePair == null)
				{
					backgrounds.Add(new IdkyKeyValuePairGameObject { Key = specialStage.ItemId });
				}
			}
		}

		this.SpecialStageBackgroundMappings = backgrounds.ToArray();

		// Initialize background music
		List<IdkyKeyValuePairAudioClip> music = this.SpecialStageBackgroundMappings != null
													? new List<IdkyKeyValuePairAudioClip>(this.SpecialStageMusicMappings)
													: new List<IdkyKeyValuePairAudioClip>();

		if (this.SpecialStageMusicMappings == null)
		{
			this.SpecialStageMusicMappings = new IdkyKeyValuePairAudioClip[specialStages.Length];

			foreach (VirtualGood specialStage in specialStages)
			{
				music.Add(new IdkyKeyValuePairAudioClip { Key = specialStage.ItemId });
			}
		}
		else
		{
			foreach (VirtualGood specialStage in specialStages)
			{
				IIdkyKeyValuePair<string, AudioClip> idkyKeyValuePair = this.SpecialStageMusicMappings.GetPair(specialStage.ItemId);

				if (idkyKeyValuePair == null)
				{
					music.Add(new IdkyKeyValuePairAudioClip { Key = specialStage.ItemId });
				}
			}
		}

		this.SpecialStageMusicMappings = music.ToArray();
	}

	public void SetSpecialStageMode(bool isSpecialStageMode)
	{
		if (isSpecialStageMode)
		{
			if (!this.specialStageMode)
			{
				this.specialStageMode = true;
				this.currentStageIndex = -1;
				this.currentSpecialStageId = string.Empty;
			}
		}
		else
		{
			if (this.specialStageMode)
			{
				this.specialStageMode = false;
				this.currentStageIndex = -1;
				this.currentSpecialStageId = string.Empty;
			}
		}
	}

	public void SkipLevel()
	{
#if UNITY_ANDROID
		// Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
		if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
		{
			return;
		}
#endif
		Dictionary<int, string> mapping = this.specialStageMode ? this.SpecialStage.LevelMapping.Mapping : this.levelMapping.Mapping;

		// This is already the last level
		if (this.Level == mapping.Count)
		{
			return;
		}

		if (this.specialStageMode)
		{
			// Special stages always has all levels unlocked
			this.NextLevel();
		}
		else
		{
			// Get the level that's been unlocked
			int levelUnlocked = PlayerPrefsFast.GetInt(SharedResources.LevelUnlockedKey, this.Level);

			// If the next level is unlocked, go to the next level
			if (levelUnlocked > this.Level)
			{
				this.NextLevel();
			}
			else
			{
				if (ShopManager.Instance.Tokens >= this.SkipLevelCost)
				{
					this.UseTokensMessage.text = string.Format("Use {0} tokens to skip this level?", this.SkipLevelCost);

					// Show Skip Level pop up if there's enough tokens
					this.UseTokensPopUp.SetActive(true);
				}
				else
				{
					this.NotEnoughTokensPopUp.SetActive(true);
				}
			}
		}
	}

	public void UseTokensAndSkipLevel()
	{
		if (ShopManager.Instance.UseTokens(this.SkipLevelCost))
		{
			// Log that the level was skipped with tokens
			GoogleAnalyticsV3.instance.LogEvent("Levels", "Skipped With Tokens", "Level " + this.Level, 1);

			// Unlock the next level
			int levelUnlocked = PlayerPrefsFast.GetInt(SharedResources.LevelUnlockedKey, this.Level);

			levelUnlocked++;
			PlayerPrefsFast.SetInt(SharedResources.LevelUnlockedKey, levelUnlocked);

			PlayerPrefsFast.Flush();

			this.SkipLevel();
		}
	}

	public void UndoMove()
	{
		if (this.movesHistory.Count > 0)
		{
			BlockMovement blockMovement = this.movesHistory.Pop();

			blockMovement.UndoMove();

			this.usedUndo = true;
		}
	}

	#endregion

	#region Methods

	private void ApplyMove(MovementDirection direction)
	{
		Debug.Log(string.Format("Applying move {0}", direction.ToString()));

		if (this.selectedGamePiece != null && this.selectedGamePiece.GameBlock is IGameBlockParent)
		{
			Debug.Log("Applying move");

			BlockMovement move;
			bool moveApplied = this.selectedGamePiece.ApplyMove(null, direction, out move);

			if (moveApplied)
			{
				Debug.Log("Move applied");

				this.movesHistory.Push(move);

				if (this.firstMove)
				{
					this.firstMove = false;

					this.startLevelTime = Time.time;

					// Update the number of times played
					string key = this.specialStageMode
									 ? string.Format(
										 "{0}_{1}_{2}", SharedResources.TimesLevelPlayedPrefix, this.SpecialStage.StageId, this.Level)
									 : string.Format("{0}_{1}", SharedResources.TimesLevelPlayedPrefix, this.Level);

					int timesPlayed = PlayerPrefsFast.GetInt(key, 0);
					timesPlayed++;
					PlayerPrefsFast.SetInt(key, timesPlayed);
					PlayerPrefsFast.Flush();
				}

				this.goToNextLevel = this.GameBoard.IsSolved();

				if (this.goToNextLevel)
				{
					this.endLevelTime = Time.time;

					bool fastComplete = this.endLevelTime - this.startLevelTime <= this.FastCompleteTime;

					// Set the number of times it took to complete this level.  Only set this once.
					string key = this.specialStageMode
									 ? string.Format(
										 "{0}_{1}_{2}", SharedResources.TimesLevelPlayedToCompletePrefix, this.SpecialStage.StageId, this.Level)
									 : string.Format("{0}_{1}", SharedResources.TimesLevelPlayedToCompletePrefix, this.Level);
					
					int timesToComplete = PlayerPrefsFast.GetInt(key, 0);

					if (timesToComplete == 0)
					{
						string timesPlayedKey = this.specialStageMode
										? string.Format(
											"{0}_{1}_{2}", SharedResources.TimesLevelPlayedPrefix, this.SpecialStage.StageId, this.Level)
										: string.Format("{0}_{1}", SharedResources.TimesLevelPlayedPrefix, this.Level);

						timesToComplete = PlayerPrefsFast.GetInt(timesPlayedKey, 1);
						PlayerPrefsFast.SetInt(key, timesToComplete);

						bool beatOnFirstTry = timesToComplete == 1;
						bool beatOnTooManyTries = timesToComplete >= this.PlayedTooManyTimes;

						// Give a free token if passed on the first try
						if (beatOnFirstTry && fastComplete && !this.usedUndo)
						{
							ShopManager.Instance.GiveTokens(this.FastCompleteReward);
							this.ResultsScreenFreeToken.GetComponent<UILabel>().text = string.Format(
								"You earned {0} free tokens!", this.FastCompleteReward);
						}
						else if (beatOnFirstTry && !this.usedUndo)
						{
							ShopManager.Instance.GiveTokens(1);
							this.ResultsScreenFreeToken.GetComponent<UILabel>().text = "You earned a free token!";
						}
						else if (beatOnTooManyTries)
						{
							this.PlayedTooManyTimesSounds.SetActive(true);
						}

						this.ResultsScreenFreeToken.SetActive(beatOnFirstTry && !usedUndo);
					}
					else
					{
						// This level has been beaten before, so no free token
						this.ResultsScreenFreeToken.SetActive(false);

						this.PlayedTooManyTimesSounds.SetActive(false);
					}

					if (!this.specialStageMode)
					{
						// Unlock the next level if needed
						int levelUnlocked = PlayerPrefsFast.GetInt(SharedResources.LevelUnlockedKey, this.Level);

						if (levelUnlocked <= this.Level)
						{
							levelUnlocked++;
							PlayerPrefsFast.SetInt(SharedResources.LevelUnlockedKey, levelUnlocked);
						}
					}

					// Log how many tries it took to complete the level
					string levelPlayed = this.specialStageMode
											 ? string.Format("{0}_{1}", this.SpecialStage.StageId, this.Level)
											 : this.Level.ToString();
					GoogleAnalyticsV3.instance.LogEvent("Levels", "Completed Level", levelPlayed, timesToComplete);

					PlayerPrefsFast.Flush();
				}
			}
		}
		else
		{
			Debug.Log("Gamepiece is null");
		}
	}

	private void ApplyMoving(MovementDirection direction, float pixels)
	{
		Debug.Log(string.Format("Moving {0} {1} pixels", direction.ToString(), pixels));

		if (this.selectedGamePiece != null)
		{
			Debug.Log("Applying moving");

			this.selectedGamePiece.OnMoving(direction, pixels);
		}
		else
		{
			Debug.Log("Gamepiece is null");
		}
	}

	private void Awake()
	{
		this.levelMapping = LevelMappingXmlIo.ReadLevelMapping();

		this.animationMappingsDictionary = new Dictionary<BlockAnimationType, GameObject>();

		foreach (GameBlockAnimationMapping mapping in this.AnimationMappings)
		{
			this.animationMappingsDictionary.Add(mapping.AnimationType, mapping.Animation);
		}
	}

	private void CanceledMoved()
	{
		if (this.selectedGamePiece != null)
		{
			Debug.Log("Canceled move");

			this.selectedGamePiece.OnCancelMove();
		}
		else
		{
			Debug.Log("Gamepiece is null");
		}
	}

	private IEnumerator DelayedUnselectBlock(object obj)
	{
		GamePiece gamePiece = obj as GamePiece;

		Debug.Log("Waiting to clear selected block");

		// Give time for the swipe to occur
		yield return new WaitForSeconds(.5f);

		if (gamePiece != null && ReferenceEquals(this.selectedGamePiece, gamePiece))
		{
			this.selectedGamePiece = null;
			Debug.Log("Block is null");
		}
	}

	private void LoadBackground()
	{
		// Get the index of what the next stage should be
		int remainder;
		int stageIndex = Math.DivRem(this.Level, this.LevelSelectCreator.LevelsPerStage, out remainder);
		stageIndex -= remainder == 0 ? 1 : 0;

		// Get the background it should be
		GameObject newBackground = this.StageBackgroundMappings[stageIndex];

		// Set the background music
		SoundManager.Instance.PlayBackgroundMusic(this.MusicMappings[stageIndex], true);

		if (this.currentBackground == null)
		{
			// Set the anchors of the background
			UIWidget uiWidget = newBackground.GetComponent<UIWidget>();
			if (uiWidget != null)
			{
				uiWidget.SetAnchor(this.ParentPanel.gameObject, -10, -10, 10, 10);
			}

			// The background hasn't been set before, so create it
			this.currentBackground = NGUITools.AddChild(this.ParentGameBoard, newBackground);
		}
		else if (stageIndex != this.currentStageIndex)
		{
			// The stage has been set before, so remove the old one
			if (this.currentBackground != null)
			{
				NGUITools.Destroy(this.currentBackground);
			}

			// Set the anchors of the background
			UIWidget uiWidget = newBackground.GetComponent<UIWidget>();
			if (uiWidget != null)
			{
				uiWidget.SetAnchor(this.ParentPanel.gameObject, -10, -10, 10, 10);
			}

			this.currentBackground = NGUITools.AddChild(this.ParentGameBoard, newBackground);
		}

		this.currentStageIndex = stageIndex;
	}

	private void LoadPuzzleIntoTable()
	{
		// Clear the animation queue
		this.setAnimationQueue.Clear();

		// Clear the history
		this.movesHistory = new Stack<BlockMovement>();

		// Fudge Factor so it doesn't use up the whole space, just most of it
		const float FudgeFactor = .95f;

		string puzzleName = this.specialStageMode
								? this.SpecialStage.LevelMapping.Mapping[this.Level]
								: this.levelMapping.Mapping[this.Level];
		this.GameBoard = GameBoardXmlIo.ReadGameBoard(puzzleName);

		TutorialMessage tutorial = null;
		if (!this.specialStageMode)
		{
			tutorial = this.Tutorials.FirstOrDefault(t => t.Level == this.Level);
		}

		// Set the number of columns
		IGameBlock[,] trimmedBoard = this.GameBoard.GetTrimmedBoard();
		int rows = trimmedBoard.GetLength(0);
		int columns = trimmedBoard.GetLength(1);
		this.Table.columns = columns;

		GamePiece[,] gamePieces = new GamePiece[rows,columns];

		// Calculate what is available for width and height
		float availableWidth = this.TableLimits.localSize.x * FudgeFactor;
		float availableHeight = this.TableLimits.localSize.y * FudgeFactor;

		// Show the tutorial
		if (tutorial != null)
		{
			// Account for the size of the tutorial tip
			availableHeight -= this.TutorialSize;
			this.TutorialTipStripObject.SetActive(true);
			this.TutorialTipStripMessage.text = tutorial.Message;
		}
		else
		{
			this.TutorialTipStripObject.SetActive(false);
		}

		availableWidth /= columns;
		availableHeight /= rows;

		// Choose the smallest size otherwise it will be too big to fit
		this.BlockSize = Math.Min((int)Math.Floor(Math.Min(availableWidth, availableHeight)), this.MaxBlockSize);

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				IGameBlock gameBlock = trimmedBoard[i, j];
				GamePiece block = NGUITools.AddChild(this.Table.gameObject, this.GamePiece.gameObject).GetComponent<GamePiece>();
				block.GameBlock = gameBlock;
				block.SetBlockSize(this.BlockSize);
				block.SetAnimationMappings(this.animationMappingsDictionary);
				block.LoadImage();
				block.SubscribeToGameBlockEvents();

				if (gameBlock != null)
				{
					block.name = string.Format("{0}-{1}: {2}", gameBlock.IndexRow, gameBlock.IndexColumn, gameBlock.GetBlockString());
				}
				else
				{
					block.name = "Null Block";
				}

				this.Table.children.Add(block.transform);
				gamePieces[i, j] = block;
			}
		}

		// Center the table
		float heightOffset = (float)(rows * this.BlockSize) / 2;
		float widthOffset = -(float)(columns * this.BlockSize) / 2;
		this.Table.transform.localPosition = new Vector3(widthOffset, heightOffset + this.VerticalOffset);

		this.Table.enabled = true;
		this.Table.repositionNow = true;

		this.firstMove = true;

		// Show the indicator that this is the first time the level is played
		string key = this.specialStageMode
						 ? string.Format("{0}_{1}_{2}", SharedResources.TimesLevelPlayedPrefix, this.SpecialStage.StageId, this.Level)
						 : string.Format("{0}_{1}", SharedResources.TimesLevelPlayedPrefix, this.Level);

		int timesPlayed = PlayerPrefsFast.GetInt(key, 0);
		this.FirstTimeIndicator.SetActive(timesPlayed == 0);

		this.CurrentLevelText.text = string.Format("Level {0}", this.Level);

		if (!this.specialStageMode)
		{
			foreach (TutorialAnimation tutorialAnimation in this.TutorialAnimations)
			{
				if (tutorialAnimation.Level == this.Level)
				{
					NGUITools.AddChild(gamePieces[tutorialAnimation.Row, tutorialAnimation.Column].gameObject, tutorialAnimation.Animation);
				}
			}
		}
	}

	private void LoadSpecialStageBackground()
	{
		// Get the background it should be
		IIdkyKeyValuePair<string, GameObject> backgroundKeyValuePair = this.SpecialStageBackgroundMappings.GetPair(
			this.SpecialStage.StageId);
		GameObject newBackground = backgroundKeyValuePair.GetValue();

		// Set the background music
		IIdkyKeyValuePair<string, AudioClip> musicKeyValuePair = this.SpecialStageMusicMappings.GetPair(this.SpecialStage.StageId);
		SoundManager.Instance.PlayBackgroundMusic(musicKeyValuePair.GetValue(), true);

		if (this.currentBackground == null)
		{
			// Set the anchors of the background
			UIWidget uiWidget = newBackground.GetComponent<UIWidget>();
			if (uiWidget != null)
			{
				uiWidget.SetAnchor(this.ParentPanel.gameObject, -10, -10, 10, 10);
			}

			// The background hasn't been set before, so create it
			this.currentBackground = NGUITools.AddChild(this.ParentGameBoard, newBackground);
		}
		else if (this.SpecialStage.StageId != this.currentSpecialStageId)
		{
			// The stage has been set before, so remove the old one
			if (this.currentBackground != null)
			{
				NGUITools.Destroy(this.currentBackground);
			}

			// Set the anchors of the background
			UIWidget uiWidget = newBackground.GetComponent<UIWidget>();
			if (uiWidget != null)
			{
				uiWidget.SetAnchor(this.ParentPanel.gameObject, -10, -10, 10, 10);
			}

			this.currentBackground = NGUITools.AddChild(this.ParentGameBoard, newBackground);
		}

		this.currentSpecialStageId = this.SpecialStage.StageId;
	}

	private void OnDisable()
	{
		this.setAnimationQueue.Clear();
		this.goToNextLevel = false;
	}

	private void OnEnable()
	{
		foreach (GameObject o in this.DisableOnStart)
		{
			o.SetActive(false);
		}
	}

	private void RemoveTableChildren()
	{
		List<Transform> children = new List<Transform>();

		foreach (Transform child in this.Table.children)
		{
			children.Add(child);
		}

		foreach (Transform child in children)
		{
			NGUITools.Destroy(child.gameObject);
		}
	}

	private void ShowRateUsDialog()
	{
		// Don't show it again if it's already shown
		if (this.shownRateUsDialog)
		{
			return;
		}

		this.levelsCompleted++;

		// Only show it after complete a certain number of levels
		if (this.levelsCompleted >= this.LevelsCompleteBeforeShowingRateUs)
		{
			SetRateOptionOnClick.RateOptions rateOption = (SetRateOptionOnClick.RateOptions)PlayerPrefsFast.GetInt(this.RateOptionKey, 0);

			// Only show it if it's not been rated yet
			switch (rateOption)
			{
				case SetRateOptionOnClick.RateOptions.NotRated:
					this.RateUsDialog.SetActive(true);
					break;

				case SetRateOptionOnClick.RateOptions.Rated:
				case SetRateOptionOnClick.RateOptions.DoNotShowAgain:
					break;
			}

			// Set the flag so it doesn't try to show the Rate Us dialog again
			this.shownRateUsDialog = true;
		}
	}

	private void Start()
	{
		// Cache the full screen ad
		AdManager.Instance.CacheAd(AdManager.AdNetwork.ChartBoost, AdManager.DisplayedAdType.FullScreen, CBLocation.LevelStart);
	}

	private void Update()
	{
		// Using the queue to transition between animations
		float delta = Time.time - this.lastAnimationTime;

		if (delta > this.MinimumTimeBetweenAnimations && this.setAnimationQueue.Count > 0)
		{
			Action action = this.setAnimationQueue.Dequeue();
			this.lastAnimationTime = Time.time;
			action.Invoke();
		}

		if (delta > this.TimeBeforeShowingResults && this.setAnimationQueue.Count == 0 && this.goToNextLevel)
		{
			// Reset the Tweens in the Result Screen
			foreach (UITweener tweener in this.ResultsScreenTweeners)
			{
				tweener.enabled = true;
				tweener.ResetToBeginning();
			}
			
			Dictionary<int, string> mapping = this.specialStageMode ? this.SpecialStage.LevelMapping.Mapping : this.levelMapping.Mapping;
			
			this.FirstTimeIndicator.SetActive(false);
			this.TutorialTipStripObject.SetActive(false);
			this.ClearedLevelText.text = string.Format("Level {0} Cleared!", this.Level);
			this.ResultsScreen.SetActive(true);
			this.ResultsScreenNextButton.SetActive(this.Level != mapping.Count);
			this.goToNextLevel = false;
			this.QuestDialog.SetEndDialog(this.Level, this.LevelSelectCreator.LevelsPerStage, this.specialStageMode, this.SpecialStage);
			this.ShowRateUsDialog();
			this.RestartLevelButton.SetActive(false);
			this.SkipLevelButton.SetActive(false);
			this.UndoButton.SetActive(false);

			Debug.Log("Enabling Results Screen");
		}
	}

	#endregion
}