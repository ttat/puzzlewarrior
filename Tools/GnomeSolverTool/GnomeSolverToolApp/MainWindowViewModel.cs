// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace GnomeSolverToolApp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    using Idky;

    using Microsoft.Practices.Prism.Commands;

    using Soomla.Store;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constants

        private const string UnityArchivesFolder = @"../../../../Unity Sources/Gnome/Assets/Archives/Gameboards/";

        private const string UnityResourcesFolder = @"../../../../Unity Sources/Gnome/Assets/Resources/Gameboards/";

        #endregion

        #region Fields

        private bool analyzeAfterGeneratingPuzzle;

        private ObservableCollection<ObservableCollection<GameBlockWrapper>> archivedGameBoardPuzzle;

        private ObservableCollection<GameBoard> archivedGameBoards;

        private int autoGenerateAttempted;

        private int autoGenerateAttempts;

        private float autoSaveMinimumDifficulty;

        private int changeDirectionHigherValue;

        private int changeDirectionLowerValue;

        private int countSolutionsAnalyzed;

        private int countSolutionsFailed;

        private int countSolutionsFound;

        private int extraBlockHigherValue;

        private int extraBlockLowerValue;

        private ObservableCollection<ObservableCollection<GameBlockWrapper>> gameBoardPuzzle;

        private ObservableCollection<GameBoard> gameBoards;

        private bool hideMessages;

        private bool isBusy;

        private LevelMapping levelMapping;

        private int maxNumberOfBlocksHigherValue;

        private int maxNumberOfBlocksLowerValue;

        private int maxNumberOfIntermediateCombinations;

        private int maxNumberOfMoves;

        private int maxNumberOfSolutions;

        private int milliSecondsTimeout;

        private int multiBlockHigherValue;

        private int multiBlockLowerValue;

        private int normalBlocksHigherValue;

        private int normalBlocksLowerValue;

        private int numberOfColumns;

        private int numberOfLevelsToMap;

        private int numberOfRows;

        private int playerBlocksHigherValue;

        private int playerBlocksLowerValue;

        private int puzzlesSaved;

        private Random random;

        private int rowColumnsHigherValue;

        private int rowColumnsLowerValue;

        private GameBoard selectedGameBoard;

        private GameBoard selectedGameBoardArchive;

        private KeyValuePair<int, string> selectedMapping;

        private SpecialStage selectedSpecialStage;

        private ObservableCollection<ObservableCollection<GameBlockSelector>> solvedGameBoard;

        private ObservableCollection<SpecialStage> specialStages;

        private bool testCanReverseSolve;

        private int variability;

        private string verificationResults;

        #endregion

        #region Constructors and Destructors

        public MainWindowViewModel()
        {
            this.numberOfColumns = 4;
            this.numberOfRows = 4;
            this.maxNumberOfMoves = 5;
            this.maxNumberOfSolutions = 25;
            this.milliSecondsTimeout = 30000;
            this.levelMapping = new LevelMapping();
            this.numberOfLevelsToMap = 36;
            this.rowColumnsLowerValue = 4;
            this.rowColumnsHigherValue = 7;
            this.random = new Random();
            this.maxNumberOfBlocksLowerValue = 15;
            this.maxNumberOfBlocksHigherValue = 40;
            this.playerBlocksLowerValue = 8;
            this.playerBlocksHigherValue = 16;
            this.normalBlocksLowerValue = 10;
            this.normalBlocksHigherValue = 20;
            this.changeDirectionLowerValue = 2;
            this.changeDirectionHigherValue = 8;
            this.extraBlockLowerValue = 0;
            this.extraBlockHigherValue = 3;
            this.multiBlockLowerValue = 0;
            this.multiBlockHigherValue = 3;
            this.autoSaveMinimumDifficulty = 40;
            this.autoGenerateAttempts = 100;
            this.AnalyzeAfterGeneratingPuzzle = true;
            this.InitializeCommands();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler RefreshDataGridEvent;

        #endregion

        #region Events

        private event EventHandler<GameBoardsSetEventArgs> GameBoardsSetEvent;

        #endregion

        #region Public Properties

        public bool AnalyzeAfterGeneratingPuzzle
        {
            get
            {
                return this.analyzeAfterGeneratingPuzzle;
            }
            set
            {
                if (this.analyzeAfterGeneratingPuzzle != value)
                {
                    this.analyzeAfterGeneratingPuzzle = value;
                    GameBoardReverseSolver.SetAnalyzeImmediately(value);
                    this.NotifyPropertyChanged("AnalyzeAfterGeneratingPuzzle");
                }
            }
        }

        public ObservableCollection<ObservableCollection<GameBlockWrapper>> ArchivedGameBoardPuzzle
        {
            get
            {
                return this.archivedGameBoardPuzzle;
            }
            set
            {
                if (this.archivedGameBoardPuzzle != value)
                {
                    this.archivedGameBoardPuzzle = value;
                    this.NotifyPropertyChanged("ArchivedGameBoardPuzzle");
                }
            }
        }

        public ObservableCollection<GameBoard> ArchivedGameBoards
        {
            get
            {
                return this.archivedGameBoards;
            }
            set
            {
                if (this.archivedGameBoards != value)
                {
                    this.archivedGameBoards = value;
                    this.NotifyPropertyChanged("ArchivedGameBoards");
                }
            }
        }

        public int AutoGenerateAttempted
        {
            get
            {
                return this.autoGenerateAttempted;
            }
            set
            {
                if (this.autoGenerateAttempted != value)
                {
                    this.autoGenerateAttempted = value;
                    this.NotifyPropertyChanged("AutoGenerateAttempted");
                }
            }
        }

        public int AutoGenerateAttempts
        {
            get
            {
                return this.autoGenerateAttempts;
            }
            set
            {
                if (this.autoGenerateAttempts != value)
                {
                    this.autoGenerateAttempts = value;
                    this.NotifyPropertyChanged("AutoGenerateAttempts");
                }
            }
        }

        public float AutoSaveMinimumDifficulty
        {
            get
            {
                return this.autoSaveMinimumDifficulty;
            }
            set
            {
                if (this.autoSaveMinimumDifficulty != value)
                {
                    this.autoSaveMinimumDifficulty = value;
                    this.NotifyPropertyChanged("AutoSaveMinimumDifficulty");
                }
            }
        }

        public int ChangeDirectionHigherValue
        {
            get
            {
                return this.changeDirectionHigherValue;
            }
            set
            {
                if (this.changeDirectionHigherValue != value)
                {
                    this.changeDirectionHigherValue = value;
                    this.NotifyPropertyChanged("ChangeDirectionHigherValue");
                }
            }
        }

        public int ChangeDirectionLowerValue
        {
            get
            {
                return this.changeDirectionLowerValue;
            }
            set
            {
                if (this.changeDirectionLowerValue != value)
                {
                    this.changeDirectionLowerValue = value;
                    this.NotifyPropertyChanged("ChangeDirectionLowerValue");
                }
            }
        }

        public DelegateCommand CommandAddLevelMapping { get; private set; }

        public DelegateCommand CommandAddSpecialStageLevelMapping { get; private set; }

        public ICommand CommandAnalyzeSolutions { get; private set; }

        public DelegateCommand<IEnumerable<GameBoard>> CommandArchiveLevel { get; private set; }

        public ICommand CommandAutoGenerateGameBoards { get; private set; }

        public ICommand CommandClearGameBoard { get; private set; }

        public ICommand CommandClearLevelMapping { get; private set; }

        public ICommand CommandClearSpecialStageLevelMapping { get; private set; }

        public ICommand CommandFixDifficulty { get; private set; }

        public ICommand CommandFixFilenames { get; private set; }

        public ICommand CommandForceFixDifficulty { get; private set; }

        public ICommand CommandGenerateGameBoard { get; private set; }

        public ICommand CommandGenerateSolutions { get; private set; }

        public ICommand CommandInitializeGameBoard { get; private set; }

        public ICommand CommandInitializeMapping { get; private set; }

        public ICommand CommandInitializeSpecialStageMapping { get; private set; }

        public ICommand CommandLoadAndClear { get; private set; }

        public ICommand CommandLoadGameBoards { get; private set; }

        public ICommand CommandLoadLevelMapping { get; private set; }

        public ICommand CommandRemoveLevelMapping { get; private set; }

        public ICommand CommandRemoveMappedLevels { get; private set; }

        public ICommand CommandRemoveSpecialStageLevelMapping { get; private set; }

        public ICommand CommandRequestAbortSolver { get; private set; }

        public ICommand CommandSaveAllGameBoards { get; private set; }

        public ICommand CommandSaveLevelMapping { get; private set; }

        public ICommand CommandSaveSelectedGameBoard { get; private set; }

        public DelegateCommand<IEnumerable<GameBoard>> CommandUnarchiveLevels { get; private set; }

        public DelegateCommand<IEnumerable<GameBoard>> CommandDeleteLevels { get; private set; }

        public ICommand CommandVerifyLevelMapping { get; private set; }

        public int CountSolutionsAnalyzed
        {
            get
            {
                return this.countSolutionsAnalyzed;
            }
            set
            {
                if (this.countSolutionsAnalyzed != value)
                {
                    this.countSolutionsAnalyzed = value;
                    this.NotifyPropertyChanged("CountSolutionsAnalyzed");
                }
            }
        }

        public int CountSolutionsFailed
        {
            get
            {
                return this.countSolutionsFailed;
            }
            set
            {
                if (this.countSolutionsFailed != value)
                {
                    this.countSolutionsFailed = value;
                    this.NotifyPropertyChanged("CountSolutionsFailed");
                }
            }
        }

        public int CountSolutionsFound
        {
            get
            {
                return this.countSolutionsFound;
            }
            set
            {
                if (this.countSolutionsFound != value)
                {
                    this.countSolutionsFound = value;
                    this.NotifyPropertyChanged("CountSolutionsFound");
                }
            }
        }

        public int ExtraBlockHigherValue
        {
            get
            {
                return this.extraBlockHigherValue;
            }
            set
            {
                if (this.extraBlockHigherValue != value)
                {
                    this.extraBlockHigherValue = value;
                    this.NotifyPropertyChanged("ExtraBlockHigherValue");
                }
            }
        }

        public int ExtraBlockLowerValue
        {
            get
            {
                return this.extraBlockLowerValue;
            }
            set
            {
                if (this.extraBlockLowerValue != value)
                {
                    this.extraBlockLowerValue = value;
                    this.NotifyPropertyChanged("ExtraBlockLowerValue");
                }
            }
        }

        public ObservableCollection<ObservableCollection<GameBlockWrapper>> GameBoardPuzzle
        {
            get
            {
                return this.gameBoardPuzzle;
            }
            set
            {
                if (this.gameBoardPuzzle != value)
                {
                    this.gameBoardPuzzle = value;
                    this.NotifyPropertyChanged("GameBoardPuzzle");
                }
            }
        }

        public ObservableCollection<GameBoard> GameBoards
        {
            get
            {
                return this.gameBoards;
            }
            set
            {
                if (this.gameBoards != value)
                {
                    this.gameBoards = value;
                    this.NotifyPropertyChanged("GameBoards");
                }
            }
        }

        public bool HideMessages
        {
            get
            {
                return this.hideMessages;
            }
            set
            {
                if (this.hideMessages != value)
                {
                    this.hideMessages = value;
                    this.NotifyPropertyChanged("HideMessages");
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.NotifyPropertyChanged("IsBusy");
                }
            }
        }

        public LevelMapping LevelMapping
        {
            get
            {
                return this.levelMapping;
            }
            set
            {
                if (this.levelMapping != value)
                {
                    this.levelMapping = value;
                    this.NotifyPropertyChanged("LevelMapping");
                }
            }
        }

        public int MaxNumberOfBlocksHigherValue
        {
            get
            {
                return this.maxNumberOfBlocksHigherValue;
            }
            set
            {
                if (this.maxNumberOfBlocksHigherValue != value)
                {
                    this.maxNumberOfBlocksHigherValue = value;
                    this.NotifyPropertyChanged("MaxNumberOfBlocksHigherValue");
                }
            }
        }

        public int MaxNumberOfBlocksLowerValue
        {
            get
            {
                return this.maxNumberOfBlocksLowerValue;
            }
            set
            {
                if (this.maxNumberOfBlocksLowerValue != value)
                {
                    this.maxNumberOfBlocksLowerValue = value;
                    this.NotifyPropertyChanged("MaxNumberOfBlocksLowerValue");
                }
            }
        }

        public int MaxNumberOfIntermediateCombinations
        {
            get
            {
                return this.maxNumberOfIntermediateCombinations;
            }
            set
            {
                if (this.maxNumberOfIntermediateCombinations != value)
                {
                    this.maxNumberOfIntermediateCombinations = value;
                    GameBoardReverseSolver.SetMaxCombinationsSolve(value);
                    this.NotifyPropertyChanged("MaxNumberOfIntermediateCombinations");
                }
            }
        }

        public int MaxNumberOfMoves
        {
            get
            {
                return this.maxNumberOfMoves;
            }
            set
            {
                if (this.maxNumberOfMoves != value)
                {
                    this.maxNumberOfMoves = value;
                    this.NotifyPropertyChanged("MaxNumberOfMoves");
                }
            }
        }

        public int MaxNumberOfSolutions
        {
            get
            {
                return this.maxNumberOfSolutions;
            }
            set
            {
                if (this.maxNumberOfSolutions != value)
                {
                    this.maxNumberOfSolutions = value;
                    this.NotifyPropertyChanged("MaxNumberOfSolutions");
                }
            }
        }

        public int MilliSecondsTimeout
        {
            get
            {
                return this.milliSecondsTimeout;
            }
            set
            {
                if (this.milliSecondsTimeout != value)
                {
                    this.milliSecondsTimeout = value;
                    this.NotifyPropertyChanged("MilliSecondsTimeout");
                }
            }
        }

        public int MultiBlockHigherValue
        {
            get
            {
                return this.multiBlockHigherValue;
            }
            set
            {
                if (this.multiBlockHigherValue != value)
                {
                    this.multiBlockHigherValue = value;
                    this.NotifyPropertyChanged("MultiBlockHigherValue");
                }
            }
        }

        public int MultiBlockLowerValue
        {
            get
            {
                return this.multiBlockLowerValue;
            }
            set
            {
                if (this.multiBlockLowerValue != value)
                {
                    this.multiBlockLowerValue = value;
                    this.NotifyPropertyChanged("MultiBlockLowerValue");
                }
            }
        }

        public int NormalBlocksHigherValue
        {
            get
            {
                return this.normalBlocksHigherValue;
            }
            set
            {
                if (this.normalBlocksHigherValue != value)
                {
                    this.normalBlocksHigherValue = value;
                    this.NotifyPropertyChanged("NormalBlocksHigherValue");
                }
            }
        }

        public int NormalBlocksLowerValue
        {
            get
            {
                return this.normalBlocksLowerValue;
            }
            set
            {
                if (this.normalBlocksLowerValue != value)
                {
                    this.normalBlocksLowerValue = value;
                    this.NotifyPropertyChanged("NormalBlocksLowerValue");
                }
            }
        }

        public int NumberOfColumns
        {
            get
            {
                return this.numberOfColumns;
            }
            set
            {
                if (this.numberOfColumns != value)
                {
                    this.numberOfColumns = value;
                    this.NotifyPropertyChanged("NumberOfColumns");
                }
            }
        }

        public int NumberOfLevelsToMap
        {
            get
            {
                return this.numberOfLevelsToMap;
            }
            set
            {
                if (this.numberOfLevelsToMap != value)
                {
                    this.numberOfLevelsToMap = value;
                    this.NotifyPropertyChanged("NumberOfLevelsToMap");
                }
            }
        }

        public int NumberOfRows
        {
            get
            {
                return this.numberOfRows;
            }
            set
            {
                if (this.numberOfRows != value)
                {
                    this.numberOfRows = value;
                    this.NotifyPropertyChanged("NumberOfRows");
                }
            }
        }

        public int PlayerBlocksHigherValue
        {
            get
            {
                return this.playerBlocksHigherValue;
            }
            set
            {
                if (this.playerBlocksHigherValue != value)
                {
                    this.playerBlocksHigherValue = value;
                    this.NotifyPropertyChanged("PlayerBlocksHigherValue");
                }
            }
        }

        public int PlayerBlocksLowerValue
        {
            get
            {
                return this.playerBlocksLowerValue;
            }
            set
            {
                if (this.playerBlocksLowerValue != value)
                {
                    this.playerBlocksLowerValue = value;
                    this.NotifyPropertyChanged("PlayerBlocksLowerValue");
                }
            }
        }

        public int PuzzlesSaved
        {
            get
            {
                return this.puzzlesSaved;
            }
            set
            {
                if (this.puzzlesSaved != value)
                {
                    this.puzzlesSaved = value;
                    this.NotifyPropertyChanged("PuzzlesSaved");
                }
            }
        }

        public int RowColumnsHigherValue
        {
            get
            {
                return this.rowColumnsHigherValue;
            }
            set
            {
                if (this.rowColumnsHigherValue != value)
                {
                    this.rowColumnsHigherValue = value;
                    this.NotifyPropertyChanged("RowColumnsHigherValue");
                }
            }
        }

        public int RowColumnsLowerValue
        {
            get
            {
                return this.rowColumnsLowerValue;
            }
            set
            {
                if (this.rowColumnsLowerValue != value)
                {
                    this.rowColumnsLowerValue = value;
                    this.NotifyPropertyChanged("RowColumnsLowerValue");
                }
            }
        }

        public GameBoard SelectedGameBoard
        {
            get
            {
                return this.selectedGameBoard;
            }
            set
            {
                if (this.selectedGameBoard != value)
                {
                    this.selectedGameBoard = value;
                    this.NotifyPropertyChanged("SelectedGameBoard");
                    this.SetSelectedGameBoard(value);
                    this.CommandAddLevelMapping.RaiseCanExecuteChanged();
                    this.CommandAddSpecialStageLevelMapping.RaiseCanExecuteChanged();
                    this.CommandArchiveLevel.RaiseCanExecuteChanged();
                }
            }
        }

        public GameBoard SelectedGameBoardArchive
        {
            get
            {
                return this.selectedGameBoardArchive;
            }
            set
            {
                if (this.selectedGameBoardArchive != value)
                {
                    this.selectedGameBoardArchive = value;
                    this.NotifyPropertyChanged("SelectedGameBoardArchive");
                    this.SetSelectedGameBoardArchive(value);
                }
            }
        }

        public KeyValuePair<int, string> SelectedMapping
        {
            get
            {
                return this.selectedMapping;
            }
            set
            {
                this.selectedMapping = value;
                this.NotifyPropertyChanged("SelectedMapping");

                this.LoadFromMapping(value);
            }
        }

        public SpecialStage SelectedSpecialStage
        {
            get
            {
                return this.selectedSpecialStage;
            }
            set
            {
                if (this.selectedSpecialStage != value)
                {
                    this.selectedSpecialStage = value;
                    this.NotifyPropertyChanged("SelectedSpecialStage");
                }
            }
        }

        public ObservableCollection<ObservableCollection<GameBlockSelector>> SolvedGameBoard
        {
            get
            {
                return this.solvedGameBoard;
            }
            set
            {
                if (this.solvedGameBoard != value)
                {
                    this.solvedGameBoard = value;
                    this.NotifyPropertyChanged("SolvedGameBoard");
                }
            }
        }

        public ObservableCollection<SpecialStage> SpecialStages
        {
            get
            {
                return this.specialStages;
            }
            set
            {
                if (this.specialStages != value)
                {
                    this.specialStages = value;
                    this.NotifyPropertyChanged("SpecialStages");
                }
            }
        }

        public bool TestCanReverseSolve
        {
            get
            {
                return this.testCanReverseSolve;
            }
            set
            {
                if (this.testCanReverseSolve != value)
                {
                    this.testCanReverseSolve = value;
                    GameBoardReverseSolver.SetTestCanReverseSolve(value);
                    this.NotifyPropertyChanged("TestCanReverseSolve");
                }
            }
        }

        public int Variability
        {
            get
            {
                return this.variability;
            }
            set
            {
                if (this.variability != value)
                {
                    this.variability = value;
                    this.NotifyPropertyChanged("Variability");
                }
            }
        }

        public string VerificationResults
        {
            get
            {
                return this.verificationResults;
            }
            private set
            {
                if (this.verificationResults != value)
                {
                    this.verificationResults = value;
                    this.NotifyPropertyChanged("VerificationResults");
                }
            }
        }

        #endregion

        #region Methods

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AddBlockTypes(List<GameBoard.GameBlockType> blockTypes, int min, int max, GameBoard.GameBlockType type)
        {
            int next = this.random.Next(min, max + 1);

            for (int i = 0; i < next; i++)
            {
                blockTypes.Add(type);
            }
        }

        private void GameBoardReverseSolverOnAnalyzedSolutionEvent(object sender, EventArgs eventArgs)
        {
            Interlocked.Increment(ref this.countSolutionsAnalyzed);

            Application.Current.Dispatcher.Invoke(new Action(() => this.NotifyPropertyChanged("CountSolutionsAnalyzed")));
        }

        private void GameBoardReverseSolverOnSolutionFailedEvent(object sender, EventArgs eventArgs)
        {
            int increment = Interlocked.Increment(ref this.countSolutionsFailed);

            if (increment % 1000 == 0)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => this.NotifyPropertyChanged("CountSolutionsFailed")));
            }
        }

        private void GameBoardReverseSolverOnSolutionFoundEvent(object sender, EventArgs eventArgs)
        {
            Interlocked.Increment(ref this.countSolutionsFound);

            Application.Current.Dispatcher.Invoke(new Action(() => this.NotifyPropertyChanged("CountSolutionsFound")));
        }

        private void GameBoardReverseSolverOnSolveSolutionEndedEvent(object sender, EventArgs eventArgs)
        {
            this.IsBusy = false;
        }

        private void GameBoardReverseSolverOnSolveSolutionStartedEvent(object sender, EventArgs eventArgs)
        {
            this.IsBusy = true;
        }

        private List<GameBlockSelector> GetAllNeighbors(GameBlockSelector gameBlockSelector)
        {
            List<GameBlockSelector> nullNeighbor = new List<GameBlockSelector>();

            int row = gameBlockSelector.Row - 1;
            int column = gameBlockSelector.Column;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector top = this.solvedGameBoard[row][column];
                nullNeighbor.Add(top);
            }

            row = gameBlockSelector.Row + 1;
            column = gameBlockSelector.Column;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector bottom = this.solvedGameBoard[row][column];
                nullNeighbor.Add(bottom);
            }

            row = gameBlockSelector.Row;
            column = gameBlockSelector.Column - 1;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector left = this.solvedGameBoard[row][column];
                nullNeighbor.Add(left);
            }

            row = gameBlockSelector.Row;
            column = gameBlockSelector.Column + 1;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector right = this.solvedGameBoard[row][column];
                nullNeighbor.Add(right);
            }

            return nullNeighbor;
        }

        private GameBlockSelector GetNeighbor(GameBlockSelector gameBlockSelector, MovementDirection direction)
        {
            GameBlockSelector neighbor = null;
            int row;
            int column;

            switch (direction)
            {
                case MovementDirection.Up:
                    row = gameBlockSelector.Row - 1;
                    column = gameBlockSelector.Column;

                    if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
                    {
                        neighbor = this.solvedGameBoard[row][column];
                    }

                    break;

                case MovementDirection.Down:
                    row = gameBlockSelector.Row + 1;
                    column = gameBlockSelector.Column;

                    if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
                    {
                        neighbor = this.solvedGameBoard[row][column];
                    }

                    break;

                case MovementDirection.Left:
                    row = gameBlockSelector.Row;
                    column = gameBlockSelector.Column - 1;

                    if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
                    {
                        neighbor = this.solvedGameBoard[row][column];
                    }
                    break;

                case MovementDirection.Right:
                    row = gameBlockSelector.Row;
                    column = gameBlockSelector.Column + 1;

                    if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
                    {
                        neighbor = this.solvedGameBoard[row][column];
                    }
                    break;
            }

            return neighbor;
        }

        private GameBlockSelector GetNextBlock(
            List<GameBlockSelector> untouchedBlocks,
            List<GameBlockSelector> usedBlocks,
            List<GameBlockSelector> fullyUsedBlocks,
            GameBoard.GameBlockType blockType)
        {
            GameBlockSelector nextBlock = null;
            MovementDirection directionForChangeDirection = this.GetRandomDirection();

            if (blockType == GameBoard.GameBlockType.Player)
            {
                // Player blocks can go anywhere
                int next = 0;
                int neighborsCount = 4;

                while (neighborsCount == 4)
                {
                    next = this.random.Next(0, untouchedBlocks.Count);
                    nextBlock = untouchedBlocks[next];

                    List<GameBlockSelector> neighbors = this.GetAllNeighbors(nextBlock);

                    // Can't put a player in the middle of 4 players
                    neighborsCount = neighbors.Count(b => b.BlockType == GameBoard.GameBlockType.Player);
                }

                untouchedBlocks.RemoveAt(next);
                usedBlocks.Add(nextBlock);
            }
            else
            {
                List<GameBlockSelector> connected = new List<GameBlockSelector>(usedBlocks);
                this.Shuffle(connected, connected.Count);
                List<Tuple<GameBlockSelector, List<GameBlockSelector>>> usableNeighbors =
                    new List<Tuple<GameBlockSelector, List<GameBlockSelector>>>();

                foreach (GameBlockSelector gameBlockSelector in connected)
                {
                    List<GameBlockSelector> usableNeighbor = this.GetNullNeighbors(gameBlockSelector);
                    usableNeighbors.Add(new Tuple<GameBlockSelector, List<GameBlockSelector>>(gameBlockSelector, usableNeighbor));
                }

                foreach (Tuple<GameBlockSelector, List<GameBlockSelector>> usableNeighbor in
                    usableNeighbors.OrderByDescending(n => n.Item2.Count))
                {
                    if (usableNeighbor.Item2.Count == 0)
                    {
                        // Already fully used this block to find usable neighbors
                        usedBlocks.Remove(usableNeighbor.Item1);
                        fullyUsedBlocks.Add(usableNeighbor.Item1);
                    }
                    else
                    {
                        this.Shuffle(usableNeighbor.Item2, usableNeighbor.Item2.Count);

                        foreach (GameBlockSelector neighbor in usableNeighbor.Item2)
                        {
                            // Trying to create a block of "blockType".  Make sure we can place it here.
                            switch (blockType)
                            {
                                case GameBoard.GameBlockType.ChangeDirection:
                                    // Can't point at null, a player, or at a direction that is pointing at itself
                                    GameBlockSelector neighborOfChangeDirection = this.GetNeighbor(neighbor, directionForChangeDirection);

                                    if (neighborOfChangeDirection == null
                                        || neighborOfChangeDirection.BlockType == GameBoard.GameBlockType.Null
                                        || neighborOfChangeDirection.BlockType == GameBoard.GameBlockType.Player)
                                    {
                                        continue;
                                    }
                                    else if (neighborOfChangeDirection.BlockType == GameBoard.GameBlockType.ChangeDirection)
                                    {
                                        GameBlockSelector possibleMirror = this.GetNeighbor(
                                            neighborOfChangeDirection, neighborOfChangeDirection.Direction);

                                        // Two change directions pointing at each other
                                        if (ReferenceEquals(neighbor, possibleMirror))
                                        {
                                            continue;
                                        }
                                    }

                                    break;

                                case GameBoard.GameBlockType.MultipleMoves:
                                case GameBoard.GameBlockType.ExtraMove:
                                    // Can't be by itself
                                    if (this.GetNullNeighbors(neighbor).Count == 4)
                                    {
                                        continue;
                                    }

                                    // It can't have all neighbors as player and null types
                                    if (
                                        this.GetAllNeighbors(neighbor).All(
                                            n =>
                                            n.BlockType == GameBoard.GameBlockType.Null || n.BlockType == GameBoard.GameBlockType.Player))
                                    {
                                        continue;
                                    }
                                    break;

                                case GameBoard.GameBlockType.Normal:
                                    // A normal block just can't be by itself
                                    if (this.GetNullNeighbors(neighbor).Count == 4)
                                    {
                                        continue;
                                    }
                                    break;

                                case GameBoard.GameBlockType.Player:
                                    List<GameBlockSelector> neighbors = this.GetAllNeighbors(neighbor);

                                    // Can't put a player in the middle of 4 players
                                    if (neighbors.Count(b => b.BlockType == GameBoard.GameBlockType.Player) == 4)
                                    {
                                        continue;
                                    }
                                    break;

                                default:
                                case GameBoard.GameBlockType.Null:
                                    // Shouldn't be here
                                    throw new Exception("Trying to place a null block");
                                    break;
                            }

                            nextBlock = neighbor;
                            untouchedBlocks.Remove(neighbor);
                            usedBlocks.Add(neighbor);
                            break;
                        }
                    }

                    if (nextBlock != null)
                    {
                        break;
                    }
                }
            }

            if (nextBlock != null)
            {
                nextBlock.BlockType = blockType;

                switch (blockType)
                {
                    case GameBoard.GameBlockType.ChangeDirection:
                        nextBlock.Direction = directionForChangeDirection;
                        break;

                    case GameBoard.GameBlockType.MultipleMoves:
                        nextBlock.NumberOfTimes = 2;
                        break;

                    default:
                        break;
                }
            }

            return nextBlock;
        }

        private List<GameBlockSelector> GetNullNeighbors(GameBlockSelector gameBlockSelector)
        {
            List<GameBlockSelector> nullNeighbor = new List<GameBlockSelector>();

            int row = gameBlockSelector.Row - 1;
            int column = gameBlockSelector.Column;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector top = this.solvedGameBoard[row][column];
                if (top.BlockType == GameBoard.GameBlockType.Null)
                {
                    nullNeighbor.Add(top);
                }
            }

            row = gameBlockSelector.Row + 1;
            column = gameBlockSelector.Column;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector bottom = this.solvedGameBoard[row][column];
                if (bottom.BlockType == GameBoard.GameBlockType.Null)
                {
                    nullNeighbor.Add(bottom);
                }
            }

            row = gameBlockSelector.Row;
            column = gameBlockSelector.Column - 1;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector left = this.solvedGameBoard[row][column];
                if (left.BlockType == GameBoard.GameBlockType.Null)
                {
                    nullNeighbor.Add(left);
                }
            }

            row = gameBlockSelector.Row;
            column = gameBlockSelector.Column + 1;

            if (row >= 0 && row < this.solvedGameBoard.Count && column >= 0 && column < this.solvedGameBoard[0].Count)
            {
                GameBlockSelector right = this.solvedGameBoard[row][column];
                if (right.BlockType == GameBoard.GameBlockType.Null)
                {
                    nullNeighbor.Add(right);
                }
            }

            return nullNeighbor;
        }

        private MovementDirection GetRandomDirection()
        {
            return (MovementDirection)this.random.Next(0, 4);
        }

        private void InitializeCommands()
        {
            this.CommandInitializeGameBoard = new DelegateCommand(this.OnInitializeGameBoard);
            this.CommandClearGameBoard = new DelegateCommand(this.OnClearGameBoard);
            this.CommandGenerateSolutions = new DelegateCommand(this.OnGenerateSolutions);
            this.CommandSaveSelectedGameBoard = new DelegateCommand(this.OnSaveSelectedGameBoard);
            this.CommandSaveAllGameBoards = new DelegateCommand(this.OnSaveAllGameBoards);
            this.CommandLoadGameBoards = new DelegateCommand(this.OnLoadGameBoards);
            this.CommandInitializeMapping = new DelegateCommand(this.OnInitializeMapping);
            this.CommandAddLevelMapping = new DelegateCommand(this.OnAddLevelMapping, () => this.selectedGameBoard != null);
            this.CommandRemoveLevelMapping = new DelegateCommand<List<KeyValuePair<int, string>>>(this.OnRemoveLevelMapping);
            this.CommandRemoveSpecialStageLevelMapping =
                new DelegateCommand<List<KeyValuePair<int, string>>>(this.OnRemoveSpecialStageLevelMapping);
            this.CommandClearLevelMapping = new DelegateCommand(this.OnClearLevelMapping);
            this.CommandVerifyLevelMapping = new DelegateCommand(this.OnVerifyLevelMapping);
            this.CommandRemoveMappedLevels = new DelegateCommand(this.OnRemoveMappedLevels);
            this.CommandSaveLevelMapping = new DelegateCommand(this.OnSaveLevelMapping);
            this.CommandLoadLevelMapping = new DelegateCommand(this.OnLoadLevelMapping);
            this.CommandRequestAbortSolver = new DelegateCommand(GameBoardReverseSolver.RequestAbortSolver);
            this.CommandLoadAndClear = new DelegateCommand(
                () =>
                    {
                        this.CommandLoadGameBoards.Execute(null);
                        this.CommandRemoveMappedLevels.Execute(null);
                    });
            this.CommandFixFilenames = new DelegateCommand(this.OnFixedFilesnames);
            this.CommandFixDifficulty = new DelegateCommand(this.OnFixDifficulty);
            this.CommandForceFixDifficulty = new DelegateCommand(this.OnForceFixDifficulty);
            this.CommandArchiveLevel = new DelegateCommand<IEnumerable<GameBoard>>(this.OnArchiveLevel, g => this.selectedGameBoard != null);
            this.CommandUnarchiveLevels = new DelegateCommand<IEnumerable<GameBoard>>(
                this.OnUnarchiveLevel, g => this.selectedGameBoardArchive != null);
            this.CommandDeleteLevels = new DelegateCommand<IEnumerable<GameBoard>>(
                this.OnDeleteLevels, g => this.selectedGameBoardArchive != null);

            this.CommandInitializeSpecialStageMapping = new DelegateCommand(this.OnInitializeSpecialStagesMapping);
            this.CommandClearSpecialStageLevelMapping = new DelegateCommand(this.OnClearSpecialStageLevelMapping);
            this.CommandAddSpecialStageLevelMapping = new DelegateCommand(
                this.OnAddSpecialStageLevelMapping, () => this.selectedGameBoard != null);

            this.CommandGenerateGameBoard = new DelegateCommand(this.OnGenerateGameBoard);
            this.CommandAutoGenerateGameBoards = new DelegateCommand(this.OnAutoGenerateGameBoards);
            this.CommandAnalyzeSolutions = new DelegateCommand(this.OnAnalyzeSolutions);
        }

        private void LoadFromMapping(KeyValuePair<int, string> mapping)
        {
            if (!string.IsNullOrEmpty(mapping.Value))
            {
                GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(mapping.Value, UnityResourcesFolder);

                if (gameBoard != null)
                {
                    this.SelectedGameBoard = gameBoard;
                }
                else
                {
                    MessageBox.Show(
                        Application.Current.MainWindow,
                        string.Format("Failed to read {0} from file", mapping.Value),
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void NotifyGameBoardsSet(bool aborted)
        {
            EventHandler<GameBoardsSetEventArgs> onGameBoardsSetEvent = this.GameBoardsSetEvent;

            if (onGameBoardsSetEvent != null)
            {
                onGameBoardsSetEvent(this, new GameBoardsSetEventArgs(aborted));
            }
        }

        private void NotifyRefreshDataGridEvent()
        {
            if (this.RefreshDataGridEvent != null)
            {
                this.RefreshDataGridEvent(this, new EventArgs());
            }
        }

        private void OnAddLevelMapping()
        {
            this.levelMapping.Mapping[this.selectedMapping.Key] = this.selectedGameBoard.ToString();
            this.NotifyRefreshDataGridEvent();
        }

        private void OnAddSpecialStageLevelMapping()
        {
            this.selectedSpecialStage.LevelMapping.Mapping[this.selectedMapping.Key] = this.selectedGameBoard.ToString();
            this.NotifyRefreshDataGridEvent();
        }

        private void OnAnalyzeSolutions()
        {
            this.IsBusy = true;

            Action action = () =>
                                {
                                    foreach (GameBoard gameBoard in this.gameBoards)
                                    {
                                        int failures;
                                        int successes;

                                        GameBoardSolver.Solve(gameBoard, this.milliSecondsTimeout, out failures, out successes);

                                        gameBoard.Failures = failures;
                                        gameBoard.Successes = successes;

                                        this.CountSolutionsAnalyzed++;
                                    }

                                    Application.Current.Dispatcher.Invoke(new Action(() => { this.IsBusy = false; }));
                                };

            action.BeginInvoke(
                ar =>
                    {
                        Action asyncState = (Action)ar.AsyncState;
                        asyncState.EndInvoke(ar);
                    },
                action);
        }

        private void OnArchiveLevel(IEnumerable<GameBoard> gameBoardsToArchive)
        {
            List<GameBoard> boards = gameBoardsToArchive.ToList();

            foreach (GameBoard gameBoard in boards)
            {
                if (GameBoardXmlIo.ArchiveGameBoard(gameBoard.ToString(), UnityResourcesFolder, UnityArchivesFolder))
                {
                    this.gameBoards.Remove(gameBoard);
                    this.archivedGameBoards.Add(gameBoard);
                }
            }
        }

        private void OnAutoGenerateGameBoards()
        {
            this.GameBoardsSetEvent += this.OnAutoGenerateGameBoardsSetEvent;

            this.AutoGenerateAttempted = 0;
            this.PuzzlesSaved = 0;
            this.AnalyzeAfterGeneratingPuzzle = true;
            this.HideMessages = true;
            this.CommandGenerateGameBoard.Execute(null);
            this.CommandGenerateSolutions.Execute(null);
        }

        private void OnAutoGenerateGameBoardsSetEvent(object sender, GameBoardsSetEventArgs eventArgs)
        {
            bool requestAbort = eventArgs.Aborted;

            foreach (GameBoard gameBoard in this.gameBoards)
            {
                if (gameBoard.GetDifficulty() >= this.autoSaveMinimumDifficulty)
                {
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                    this.PuzzlesSaved++;
                }
            }

            this.AutoGenerateAttempted++;

            // Continue the loop
            if (!requestAbort && this.autoGenerateAttempted < this.autoGenerateAttempts)
            {
                this.CommandGenerateGameBoard.Execute(null);
                this.CommandGenerateSolutions.Execute(null);
            }
            else
            {
                this.GameBoardsSetEvent -= this.OnAutoGenerateGameBoardsSetEvent;
                this.HideMessages = false;
            }
        }

        private void OnClearGameBoard()
        {
            if (this.solvedGameBoard != null)
            {
                foreach (ObservableCollection<GameBlockSelector> rows in this.solvedGameBoard)
                {
                    foreach (GameBlockSelector gameBlockSelector in rows)
                    {
                        gameBlockSelector.BlockType = GameBoard.GameBlockType.Null;
                    }
                }
            }
        }

        private void OnClearLevelMapping()
        {
            this.LevelMapping = new LevelMapping();
        }

        private void OnClearSpecialStageLevelMapping()
        {
            this.selectedSpecialStage.LevelMapping = new LevelMapping();
        }

        private void OnFixDifficulty()
        {
            foreach (GameBoard gameBoard in this.gameBoards)
            {
                // Difficulty not set yet
                if (gameBoard != null && (gameBoard.Successes == 0 || gameBoard.Failures == 0))
                {
                    int successes;
                    int failures;

                    GameBoardSolver.Solve(gameBoard, this.MilliSecondsTimeout, out failures, out successes);

                    gameBoard.Failures = failures;
                    gameBoard.Successes = successes;
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                }
                else if (gameBoard != null)
                {
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                }
            }

            foreach (KeyValuePair<int, string> pair in this.levelMapping.Mapping)
            {
                // Read the gameabord
                GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(pair.Value, UnityResourcesFolder);

                // Difficulty not set yet
                if (gameBoard != null && (gameBoard.Successes == 0 || gameBoard.Failures == 0))
                {
                    int successes;
                    int failures;

                    GameBoardSolver.Solve(gameBoard, this.MilliSecondsTimeout, out failures, out successes);

                    gameBoard.Failures = failures;
                    gameBoard.Successes = successes;
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                }
                else if (gameBoard != null)
                {
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                }
            }

            foreach (SpecialStage specialStage in this.specialStages)
            {
                foreach (KeyValuePair<int, string> pair in specialStage.LevelMapping.Mapping)
                {
                    // Read the gameabord
                    GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(pair.Value, UnityResourcesFolder);

                    // Difficulty not set yet
                    if (gameBoard != null && (gameBoard.Successes == 0 || gameBoard.Failures == 0))
                    {
                        int successes;
                        int failures;

                        GameBoardSolver.Solve(gameBoard, this.MilliSecondsTimeout, out failures, out successes);

                        gameBoard.Failures = failures;
                        gameBoard.Successes = successes;
                        GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                    }
                    else if (gameBoard != null)
                    {
                        GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                    }
                }
            }
        }

        private void OnFixedFilesnames()
        {
            List<KeyValuePair<int, string>> remapped = new List<KeyValuePair<int, string>>();
            foreach (KeyValuePair<int, string> pair in this.levelMapping.Mapping)
            {
                // Read the gameabord
                GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(pair.Value, UnityResourcesFolder);

                // The filename isn't the right number of characters
                if (!string.IsNullOrEmpty(pair.Value) && pair.Value.Length != 16)
                {
                    if (gameBoard != null)
                    {
                        // Write it back with the correct number of characters
                        GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);

                        // Delete the old file with the bad filename
                        GameBoardXmlIo.DeleteGameBoard(pair.Value, UnityResourcesFolder);

                        // Make sure it's mapped correctly
                        remapped.Add(new KeyValuePair<int, string>(pair.Key, gameBoard.ToString()));
                    }
                }
                    // The filename has the wrong hash string
                else if (gameBoard != null && gameBoard.ToString() != pair.Value)
                {
                    // Write it back with the correct number of characters
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);

                    // Delete the old file with the bad filename
                    GameBoardXmlIo.DeleteGameBoard(pair.Value, UnityResourcesFolder);

                    // Make sure it's mapped correctly
                    remapped.Add(new KeyValuePair<int, string>(pair.Key, gameBoard.ToString()));
                }
            }

            foreach (KeyValuePair<int, string> keyValuePair in remapped)
            {
                this.levelMapping.Mapping[keyValuePair.Key] = keyValuePair.Value;
            }

            LevelMappingXmlIo.WriteLevelMapping(this.levelMapping, UnityResourcesFolder);

            foreach (SpecialStage specialStage in this.specialStages)
            {
                remapped = new List<KeyValuePair<int, string>>();
                foreach (KeyValuePair<int, string> pair in specialStage.LevelMapping.Mapping)
                {
                    // Read the gameabord
                    GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(pair.Value, UnityResourcesFolder);

                    // The filename isn't the right number of characters
                    if (!string.IsNullOrEmpty(pair.Value) && pair.Value.Length != 16)
                    {
                        if (gameBoard != null)
                        {
                            // Write it back with the correct number of characters
                            GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);

                            // Delete the old file with the bad filename
                            GameBoardXmlIo.DeleteGameBoard(pair.Value, UnityResourcesFolder);

                            // Make sure it's mapped correctly
                            remapped.Add(new KeyValuePair<int, string>(pair.Key, gameBoard.ToString()));
                        }
                    }
                        // The filename has the wrong hash string
                    else if (gameBoard != null && gameBoard.ToString() != pair.Value)
                    {
                        // Write it back with the correct number of characters
                        GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);

                        // Delete the old file with the bad filename
                        GameBoardXmlIo.DeleteGameBoard(pair.Value, UnityResourcesFolder);

                        // Make sure it's mapped correctly
                        remapped.Add(new KeyValuePair<int, string>(pair.Key, gameBoard.ToString()));
                    }
                }

                foreach (KeyValuePair<int, string> keyValuePair in remapped)
                {
                    specialStage.LevelMapping.Mapping[keyValuePair.Key] = keyValuePair.Value;
                }

                SpecialStageXmlIo.WriteSpecialStage(specialStage, UnityResourcesFolder);
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnForceFixDifficulty()
        {
            foreach (GameBoard gameBoard in this.gameBoards)
            {
                // Difficulty not set yet
                if (gameBoard != null)
                {
                    int successes;
                    int failures;

                    GameBoardSolver.Solve(gameBoard, this.MilliSecondsTimeout, out failures, out successes);

                    gameBoard.Failures = failures;
                    gameBoard.Successes = successes;
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                }
            }

            foreach (KeyValuePair<int, string> pair in this.levelMapping.Mapping)
            {
                // Read the gameabord
                GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(pair.Value, UnityResourcesFolder);

                if (gameBoard != null)
                {
                    int successes;
                    int failures;

                    GameBoardSolver.Solve(gameBoard, this.MilliSecondsTimeout, out failures, out successes);

                    gameBoard.Failures = failures;
                    gameBoard.Successes = successes;
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                }
            }

            foreach (SpecialStage specialStage in this.specialStages)
            {
                foreach (KeyValuePair<int, string> pair in specialStage.LevelMapping.Mapping)
                {
                    // Read the gameabord
                    GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(pair.Value, UnityResourcesFolder);

                    if (gameBoard != null)
                    {
                        int successes;
                        int failures;

                        GameBoardSolver.Solve(gameBoard, this.MilliSecondsTimeout, out failures, out successes);

                        gameBoard.Failures = failures;
                        gameBoard.Successes = successes;
                        GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                    }
                }
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnGenerateGameBoard()
        {
            // Create the GameBoard
            this.NumberOfRows = this.random.Next(this.rowColumnsLowerValue, this.rowColumnsHigherValue + 1);
            this.NumberOfColumns = this.random.Next(this.rowColumnsLowerValue, this.rowColumnsHigherValue + 1);

            this.CommandInitializeGameBoard.Execute(null);
            this.CommandClearGameBoard.Execute(null);

            // Generate a list of possible block types
            List<GameBoard.GameBlockType> blockTypes = new List<GameBoard.GameBlockType>();
            List<GameBoard.GameBlockType> playerBlock = new List<GameBoard.GameBlockType>();

            this.AddBlockTypes(playerBlock, this.playerBlocksLowerValue, this.playerBlocksHigherValue, GameBoard.GameBlockType.Player);
            this.AddBlockTypes(blockTypes, this.normalBlocksLowerValue, this.normalBlocksHigherValue, GameBoard.GameBlockType.Normal);
            this.AddBlockTypes(
                blockTypes, this.changeDirectionLowerValue, this.changeDirectionHigherValue, GameBoard.GameBlockType.ChangeDirection);
            this.AddBlockTypes(blockTypes, this.extraBlockLowerValue, this.extraBlockHigherValue, GameBoard.GameBlockType.ExtraMove);
            this.AddBlockTypes(blockTypes, this.multiBlockLowerValue, this.multiBlockHigherValue, GameBoard.GameBlockType.MultipleMoves);

            // Get a random number of blocks to create
            int numberOfBlocksToCreate = this.random.Next(this.maxNumberOfBlocksLowerValue, this.maxNumberOfBlocksHigherValue + 1);

            // Keep track of the blocks that have been untouched.  They can be used for the next block to set.
            List<GameBlockSelector> untouchedBlocks = new List<GameBlockSelector>();

            foreach (ObservableCollection<GameBlockSelector> row in this.solvedGameBoard)
            {
                foreach (GameBlockSelector item in row)
                {
                    untouchedBlocks.Add(item);
                }
            }

            int totalBlocks = untouchedBlocks.Count;

            // Keep track of blocks that have been used
            List<GameBlockSelector> usedBlocks = new List<GameBlockSelector>();
            List<GameBlockSelector> fullyUsedBlocks = new List<GameBlockSelector>();

            // Do Player Blocks first
            foreach (GameBoard.GameBlockType gameBlockType in playerBlock)
            {
                GameBlockSelector gameBlockSelector = this.GetNextBlock(untouchedBlocks, usedBlocks, fullyUsedBlocks, gameBlockType);
            }

            for (int i = 0; i < numberOfBlocksToCreate && i < totalBlocks; i++)
            {
                if (blockTypes.Count == 0)
                {
                    break;
                }
                GameBlockSelector gameBlockSelector;

                List<GameBoard.GameBlockType> failedToUse = new List<GameBoard.GameBlockType>();

                do
                {
                    // Get randomly one of the block types
                    int next = this.random.Next(0, blockTypes.Count);
                    GameBoard.GameBlockType nextBlockType = blockTypes[next];
                    blockTypes.RemoveAt(next);

                    gameBlockSelector = this.GetNextBlock(untouchedBlocks, usedBlocks, fullyUsedBlocks, nextBlockType);

                    if (gameBlockSelector == null)
                    {
                        failedToUse.Add(nextBlockType);
                    }
                }
                while (gameBlockSelector == null && blockTypes.Count > 0);

                blockTypes.AddRange(failedToUse);
            }

            // Surround any empty spots with a player
            foreach (GameBlockSelector gameBlockSelector in usedBlocks)
            {
                foreach (GameBlockSelector blockSelector in this.GetNullNeighbors(gameBlockSelector))
                {
                    blockSelector.BlockType = GameBoard.GameBlockType.Player;
                }
            }
        }

        private void OnGenerateSolutions()
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;

            this.CountSolutionsFound = 0;
            this.CountSolutionsFailed = 0;
            this.CountSolutionsAnalyzed = 0;

            GameBoard gameBoard = new GameBoard(this.numberOfRows, this.numberOfColumns);

            for (int i = 0; i < this.numberOfRows; i++)
            {
                for (int j = 0; j < this.numberOfColumns; j++)
                {
                    gameBoard.CreateGameBlock(
                        this.solvedGameBoard[i][j].BlockType,
                        i,
                        j,
                        this.solvedGameBoard[i][j].Direction,
                        preferredMaxMoves: this.solvedGameBoard[i][j].PreferredMax);
                }
            }

            Action solverAction = () =>
                                      {
                                          GameBoardReverseSolver.SolutionFoundEvent += this.GameBoardReverseSolverOnSolutionFoundEvent;
                                          GameBoardReverseSolver.SolutionFailedEvent += this.GameBoardReverseSolverOnSolutionFailedEvent;
                                          GameBoardReverseSolver.SolveSolutionStartedEvent +=
                                              this.GameBoardReverseSolverOnSolveSolutionStartedEvent;
                                          GameBoardReverseSolver.SolveSolutionEndedEvent +=
                                              this.GameBoardReverseSolverOnSolveSolutionEndedEvent;
                                          GameBoardReverseSolver.AnalyzedSolutionEvent += this.GameBoardReverseSolverOnAnalyzedSolutionEvent;

                                          bool aborted;
                                          List<GameBoard> reverseSolve = GameBoardReverseSolver.ReverseSolve(
                                              gameBoard,
                                              this.maxNumberOfMoves,
                                              this.maxNumberOfSolutions,
                                              this.variability,
                                              this.milliSecondsTimeout,
                                              out aborted);
                                          Application.Current.Dispatcher.Invoke(
                                              new Action(
                                                  () =>
                                                      {
                                                          this.GameBoards = new ObservableCollection<GameBoard>(reverseSolve);
                                                          this.NotifyGameBoardsSet(aborted);
                                                      }));
                                      };

            solverAction.BeginInvoke(
                (result) =>
                    {
                        try
                        {
                            Action action = (Action)result.AsyncState;
                            action.EndInvoke(result);

                            Application.Current.Dispatcher.Invoke(
                                new Action(
                                    () =>
                                        {
                                            Application.Current.MainWindow.Cursor = Cursors.Arrow;

                                            if (!this.hideMessages)
                                            {
                                                string message = this.gameBoards.Count == 0
                                                                     ? "No solutions found"
                                                                     : "Finished generating solutions";
                                                MessageBox.Show(
                                                    Application.Current.MainWindow,
                                                    message,
                                                    "Solutions",
                                                    MessageBoxButton.OK,
                                                    MessageBoxImage.Information);
                                            }
                                        }));
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke(
                                new Action(
                                    () =>
                                        {
                                            Application.Current.MainWindow.Cursor = Cursors.Arrow;
                                            MessageBox.Show(
                                                Application.Current.MainWindow,
                                                string.Format("Error generating solutions: {0}", ex.Message),
                                                "Error",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                                        }));
                        }
                        finally
                        {
                            GameBoardReverseSolver.SolutionFoundEvent -= this.GameBoardReverseSolverOnSolutionFoundEvent;
                            GameBoardReverseSolver.SolutionFailedEvent -= this.GameBoardReverseSolverOnSolutionFailedEvent;
                            GameBoardReverseSolver.SolveSolutionStartedEvent -= this.GameBoardReverseSolverOnSolveSolutionStartedEvent;
                            GameBoardReverseSolver.SolveSolutionEndedEvent -= this.GameBoardReverseSolverOnSolveSolutionEndedEvent;
                            GameBoardReverseSolver.AnalyzedSolutionEvent -= this.GameBoardReverseSolverOnAnalyzedSolutionEvent;
                        }
                    },
                solverAction);
        }

        private void OnInitializeGameBoard()
        {
            ObservableCollection<ObservableCollection<GameBlockSelector>> oldBoard = this.solvedGameBoard;
            int oldRows = oldBoard == null ? 0 : oldBoard.Count;
            int oldColumns = oldRows > 0 ? oldBoard[0].Count : 0;

            this.SolvedGameBoard = new ObservableCollection<ObservableCollection<GameBlockSelector>>();

            for (int i = 0; i < this.numberOfRows; i++)
            {
                ObservableCollection<GameBlockSelector> row = new ObservableCollection<GameBlockSelector>();

                for (int j = 0; j < this.numberOfColumns; j++)
                {
                    if (oldRows > i && oldColumns > j)
                    {
                        row.Add(oldBoard[i][j]);
                    }
                    else
                    {
                        GameBlockSelector gameBlockSelector = new GameBlockSelector(GameBoard.GameBlockType.Null);
                        gameBlockSelector.Row = i;
                        gameBlockSelector.Column = j;
                        row.Add(gameBlockSelector);
                    }
                }

                this.solvedGameBoard.Add(row);
            }
        }

        private void OnInitializeMapping()
        {
            this.levelMapping.GenerateLevelKeys(this.numberOfLevelsToMap);
            this.NotifyRefreshDataGridEvent();
        }

        private void OnInitializeSpecialStagesMapping()
        {
            this.selectedSpecialStage.LevelMapping.GenerateLevelKeys(this.numberOfLevelsToMap);
            this.NotifyRefreshDataGridEvent();
        }

        private void OnLoadGameBoards()
        {
            List<GameBoard> gameBoardsOnFile = GameBoardXmlIo.ReadAllGameBoards(UnityResourcesFolder);

            this.GameBoards = new ObservableCollection<GameBoard>(gameBoardsOnFile);

            List<GameBoard> archivedGameBoardsOnFile = GameBoardXmlIo.ReadAllGameBoards(UnityArchivesFolder);

            this.ArchivedGameBoards = new ObservableCollection<GameBoard>(archivedGameBoardsOnFile);

            this.NotifyRefreshDataGridEvent();
        }

        private void OnLoadLevelMapping()
        {
            this.LevelMapping = LevelMappingXmlIo.ReadLevelMapping(UnityResourcesFolder);

            this.SpecialStages = new ObservableCollection<SpecialStage>();

            foreach (VirtualGood virtualGood in GnomeStoreAssets.GetGoodsStatic())
            {
                SpecialStage readSpecialStage = SpecialStageXmlIo.ReadSpecialStage(virtualGood.ItemId, UnityResourcesFolder);

                if (readSpecialStage.StageId != virtualGood.ItemId)
                {
                    readSpecialStage.StageId = virtualGood.ItemId;
                    readSpecialStage.Description = virtualGood.Description;

                    SpecialStageXmlIo.WriteSpecialStage(readSpecialStage, UnityResourcesFolder);
                }

                this.specialStages.Add(readSpecialStage);
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnRemoveLevelMapping(List<KeyValuePair<int, string>> mappingsToRemove)
        {
            foreach (KeyValuePair<int, string> mapping in mappingsToRemove)
            {
                this.levelMapping.Mapping[mapping.Key] = string.Empty;
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnRemoveMappedLevels()
        {
            // Create a dictionary using the Gameboard as the key to find which Gameboards have been mapped to a level already
            Dictionary<string, int> gameboardToLevelDictionary = new Dictionary<string, int>();

            foreach (KeyValuePair<int, string> keyValuePair in this.levelMapping.Mapping)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                {
                    gameboardToLevelDictionary.Add(keyValuePair.Value, keyValuePair.Key);
                }
            }

            if (this.specialStages != null)
            {
                foreach (SpecialStage specialStage in this.specialStages)
                {
                    foreach (KeyValuePair<int, string> keyValuePair in specialStage.LevelMapping.Mapping)
                    {
                        if (!string.IsNullOrEmpty(keyValuePair.Value))
                        {
                            gameboardToLevelDictionary.Add(keyValuePair.Value, keyValuePair.Key);
                        }
                    }
                }
            }

            // Find the Gameboard that has been used already
            List<GameBoard> remove = new List<GameBoard>();
            foreach (GameBoard gameBoard in this.gameBoards)
            {
                if (gameboardToLevelDictionary.ContainsKey(gameBoard.ToString()))
                {
                    remove.Add(gameBoard);
                }
            }

            // And remove it
            foreach (GameBoard gameBoard in remove)
            {
                this.gameBoards.Remove(gameBoard);
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnRemoveSpecialStageLevelMapping(List<KeyValuePair<int, string>> mappingsToRemove)
        {
            foreach (KeyValuePair<int, string> mapping in mappingsToRemove)
            {
                this.selectedSpecialStage.LevelMapping.Mapping[mapping.Key] = string.Empty;
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnSaveAllGameBoards()
        {
            if (this.gameBoards != null)
            {
                foreach (GameBoard gameBoard in this.gameBoards)
                {
                    GameBoardXmlIo.WriteGameBoard(gameBoard, UnityResourcesFolder);
                }

                MessageBox.Show(
                    Application.Current.MainWindow,
                    "Saved all Game Boards",
                    "Saved Game Board",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void OnSaveLevelMapping()
        {
            LevelMappingXmlIo.WriteLevelMapping(this.levelMapping, UnityResourcesFolder);

            foreach (SpecialStage specialStage in this.specialStages)
            {
                SpecialStageXmlIo.WriteSpecialStage(specialStage, UnityResourcesFolder);
            }
        }

        private void OnSaveSelectedGameBoard()
        {
            if (this.selectedGameBoard != null)
            {
                string savedPath = GameBoardXmlIo.WriteGameBoard(this.selectedGameBoard, UnityResourcesFolder);
                MessageBox.Show(
                    Application.Current.MainWindow,
                    string.Format("Saved the Game Board to the following path: {0}", savedPath),
                    "Saved Game Board",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void OnUnarchiveLevel(IEnumerable<GameBoard> gameBoardsToArchive)
        {
            List<GameBoard> boards = gameBoardsToArchive.ToList();

            foreach (GameBoard gameBoard in boards)
            {
                if (GameBoardXmlIo.ArchiveGameBoard(gameBoard.ToString(), UnityArchivesFolder, UnityResourcesFolder))
                {
                    this.archivedGameBoards.Remove(gameBoard);
                    this.gameBoards.Add(gameBoard);
                }
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnDeleteLevels(IEnumerable<GameBoard> gameBoardsToDelete)
        {
            List<GameBoard> boards = gameBoardsToDelete.ToList();

            foreach (GameBoard gameBoard in boards)
            {
                if (GameBoardXmlIo.DeleteGameBoard(gameBoard.ToString(), UnityArchivesFolder))
                {
                    this.archivedGameBoards.Remove(gameBoard);
                }
            }

            this.NotifyRefreshDataGridEvent();
        }

        private void OnVerifyLevelMapping()
        {
            // Create a dictionary using the Gameboard as the key to find which Gameboards have been mapped to a level already
            Dictionary<string, int> gameboardToLevelDictionary = new Dictionary<string, int>();
            List<string> duplicateMappings = new List<string>();

            foreach (KeyValuePair<int, string> keyValuePair in this.levelMapping.Mapping)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                {
                    if (gameboardToLevelDictionary.ContainsKey(keyValuePair.Value))
                    {
                        int levelMapped = gameboardToLevelDictionary[keyValuePair.Value];
                        duplicateMappings.Add(
                            string.Format(
                                "Gameboard already mapped: Gameboard - {0}, Level Trying to Map - {1}, Already Mapped to {2}",
                                keyValuePair.Value,
                                keyValuePair.Key,
                                levelMapped));
                    }
                    else
                    {
                        gameboardToLevelDictionary.Add(keyValuePair.Value, keyValuePair.Key);
                    }
                }
            }

            foreach (SpecialStage specialStage in this.specialStages)
            {
                foreach (KeyValuePair<int, string> keyValuePair in specialStage.LevelMapping.Mapping)
                {
                    if (!string.IsNullOrEmpty(keyValuePair.Value))
                    {
                        if (gameboardToLevelDictionary.ContainsKey(keyValuePair.Value))
                        {
                            int levelMapped = gameboardToLevelDictionary[keyValuePair.Value];
                            duplicateMappings.Add(
                                string.Format(
                                    "Gameboard already mapped in {0}: Gameboard - {1}, Level Trying to Map - {2}, Already Mapped to {3}",
                                    specialStage.StageId,
                                    keyValuePair.Value,
                                    keyValuePair.Key,
                                    levelMapped));
                        }
                        else
                        {
                            gameboardToLevelDictionary.Add(keyValuePair.Value, keyValuePair.Key);
                        }
                    }
                }
            }

            List<KeyValuePair<int, string>> noLevels = this.levelMapping.Mapping.Where(m => string.IsNullOrEmpty(m.Value)).ToList();

            List<Tuple<string, List<KeyValuePair<int, string>>>> specialStageNoLevels =
                new List<Tuple<string, List<KeyValuePair<int, string>>>>();

            foreach (SpecialStage specialStage in this.specialStages)
            {
                List<KeyValuePair<int, string>> noLevelsMapped =
                    specialStage.LevelMapping.Mapping.Where(m => string.IsNullOrEmpty(m.Value)).ToList();
                if (noLevelsMapped.Count > 0)
                {
                    specialStageNoLevels.Add(new Tuple<string, List<KeyValuePair<int, string>>>(specialStage.StageId, noLevelsMapped));
                }
            }

            List<string> failedToRead = new List<string>();

            foreach (KeyValuePair<int, string> keyValuePair in this.levelMapping.Mapping)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                {
                    GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(keyValuePair.Value, UnityResourcesFolder);

                    if (gameBoard == null)
                    {
                        failedToRead.Add(
                            string.Format("Gameboard failed to load: Level - {0}, Gameboard - {1}", keyValuePair.Key, keyValuePair.Value));
                    }
                }
            }

            foreach (SpecialStage specialStage in this.specialStages)
            {
                foreach (KeyValuePair<int, string> keyValuePair in specialStage.LevelMapping.Mapping)
                {
                    if (!string.IsNullOrEmpty(keyValuePair.Value))
                    {
                        GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(keyValuePair.Value, UnityResourcesFolder);

                        if (gameBoard == null)
                        {
                            failedToRead.Add(
                                string.Format(
                                    "Gameboard failed to load from {0}: Level - {1}, Gameboard - {2}",
                                    specialStage.StageId,
                                    keyValuePair.Key,
                                    keyValuePair.Value));
                        }
                    }
                }
            }

            if (duplicateMappings.Count > 0 || noLevels.Count > 0 || specialStageNoLevels.Count > 0 || failedToRead.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (string duplicateMapping in duplicateMappings)
                {
                    stringBuilder.AppendLine(duplicateMapping);
                }

                foreach (KeyValuePair<int, string> noLevel in noLevels)
                {
                    stringBuilder.AppendLine(string.Format("Level has no Gameboard mapped: Level - {0}", noLevel.Key));
                }

                foreach (Tuple<string, List<KeyValuePair<int, string>>> specialStageNoLevel in specialStageNoLevels)
                {
                    foreach (KeyValuePair<int, string> noLevel in specialStageNoLevel.Item2)
                    {
                        stringBuilder.AppendLine(
                            string.Format("Level on stage {0} has no Gameboard mapped: Level - {1}", specialStageNoLevel.Item1, noLevel.Key));
                    }
                }

                foreach (string failedReading in failedToRead)
                {
                    stringBuilder.AppendLine(failedReading);
                }

                this.VerificationResults = stringBuilder.ToString();
            }
            else
            {
                this.VerificationResults = "No problems found!";
            }
        }

        private void SetSelectedGameBoard(GameBoard gameBoard)
        {
            ObservableCollection<ObservableCollection<GameBlockWrapper>> newGameBoard =
                new ObservableCollection<ObservableCollection<GameBlockWrapper>>();

            if (gameBoard != null)
            {
                for (int i = 0; i < gameBoard.GameBlocks.GetLength(0); i++)
                {
                    ObservableCollection<GameBlockWrapper> row = new ObservableCollection<GameBlockWrapper>();

                    for (int j = 0; j < gameBoard.GameBlocks.GetLength(1); j++)
                    {
                        row.Add(new GameBlockWrapper(gameBoard.GameBlocks[i, j]));
                    }

                    newGameBoard.Add(row);
                }
            }

            this.GameBoardPuzzle = newGameBoard;
        }

        private void SetSelectedGameBoardArchive(GameBoard gameBoard)
        {
            ObservableCollection<ObservableCollection<GameBlockWrapper>> newGameBoard =
                new ObservableCollection<ObservableCollection<GameBlockWrapper>>();

            if (gameBoard != null)
            {
                for (int i = 0; i < gameBoard.GameBlocks.GetLength(0); i++)
                {
                    ObservableCollection<GameBlockWrapper> row = new ObservableCollection<GameBlockWrapper>();

                    for (int j = 0; j < gameBoard.GameBlocks.GetLength(1); j++)
                    {
                        row.Add(new GameBlockWrapper(gameBoard.GameBlocks[i, j]));
                    }

                    newGameBoard.Add(row);
                }
            }

            this.ArchivedGameBoardPuzzle = newGameBoard;
        }

        private void Shuffle(List<GameBlockSelector> numbers, int timesToShuffle)
        {
            int maxIndex = numbers.Count - 1;

            for (int i = 0; i < timesToShuffle; i++)
            {
                int index1 = this.random.Next(0, maxIndex);
                int index2 = this.random.Next(0, maxIndex);

                GameBlockSelector temp = numbers[index1];
                numbers[index1] = numbers[index2];
                numbers[index2] = temp;
            }
        }

        #endregion
    }
}