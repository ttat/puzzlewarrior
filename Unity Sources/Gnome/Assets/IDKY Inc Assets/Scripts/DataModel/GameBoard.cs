// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class GameBoard : ICloneable
#if UNITY_IOS
        , ISerializable
#endif
    {
        #region Fields

        private int failures;

        private IGameBlock[,] gameBlocks;

        [field: NonSerialized]
        private BlockMovement solutionStartMove;

        private int successes;

        #endregion

        #region Constructors and Destructors

        public GameBoard()
            : this(0, 0)
        {
        }

        public GameBoard(int rows, int columns)
        {
            this.gameBlocks = new IGameBlock[rows,columns];

            // Initialize the blocks to null blocks
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this.CreateGameBlock(GameBlockType.Null, i, j);
                }
            }

            this.solutionStartMove = null;
        }

        public GameBoard(SerializationInfo info, StreamingContext ctxt)
        {
            this.gameBlocks = (IGameBlock[,])info.GetValue("GameBlocks", typeof(IGameBlock[,]));
            this.successes = info.GetInt32("Successes");
            this.failures = info.GetInt32("Failures");
        }

        #endregion

        #region Enums

        public enum GameBlockType
        {
            Null,

            Normal,

            Player,

            ChangeDirection,

            ExtraMove,

            MultipleMoves
        }

        #endregion

        #region Public Properties

        public int Failures
        {
            get
            {
                return this.failures;
            }

            set
            {
                this.failures = value;
            }
        }

        [XmlIgnore]
        public IGameBlock[,] GameBlocks
        {
            get
            {
                return this.gameBlocks;
            }

            set
            {
                this.gameBlocks = value;
            }
        }

        [XmlArray("GameBlocks")]
        [XmlArrayItem("GameBlock", typeof(GameBlockBase[]))]
        public GameBlockBase[][] GameBlocksJagged
        {
            get
            {
                return this.gameBlocks.MultidimensionToJaggedArray<IGameBlock, GameBlockBase>();
            }

            set
            {
                this.gameBlocks = value.JaggedArrayToMultidimension<GameBlockBase, IGameBlock>();
            }

        }
        
        [XmlIgnore]
        public BlockMovement SolutionStartMove
        {
            get
            {
                return this.solutionStartMove;
            }
            
            set
            {
                this.solutionStartMove = value;
            }
        }

        public int Successes
        {
            get
            {
                return this.successes;
            }
            set
            {
                this.successes = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public double GetDifficulty()
        {
            IDifficultyCalculator difficultyCalculator = DifficultyCalculatorFactory.GetCalculator(this.GetContainingBlockTypes());

            return difficultyCalculator.CalculateDifficulty(this);
        }

        public object Clone()
        {
            GameBoard clone = new GameBoard();
            clone.gameBlocks = new IGameBlock[this.gameBlocks.GetLength(0),this.gameBlocks.GetLength(1)];

            foreach (IGameBlock gameBlock in this.gameBlocks)
            {
                if (gameBlock != null)
                {
                    IGameBlock block = (IGameBlock)gameBlock.Clone();
                    block.GameBoard = clone;
                }
            }

            return clone;
        }

        public void Copy(GameBoard source)
        {
            for (int i = 0; i < this.gameBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < this.gameBlocks.GetLength(1); j++)
                {
                    this.gameBlocks[i, j].Copy(source.gameBlocks[i, j]);
                }
            }
        }

        public IGameBlock CreateGameBlock(
            GameBlockType gameBlockType,
            int row,
            int column,
            MovementDirection changeDirection = MovementDirection.Down,
            int numberOfMultipleMoves = 2,
            int preferredMaxMoves = 0)
        {
            IGameBlock gameBlock;

            switch (gameBlockType)
            {
                case GameBlockType.Null:
                    gameBlock = new GameBlockNull(this, row, column);
                    break;

                case GameBlockType.Normal:
                    gameBlock = new GameBlockNormal(this, row, column);
                    break;

                case GameBlockType.Player:
                    gameBlock = new GameBlockPlayer(this, row, column, preferredMaxMoves);
                    break;

                case GameBlockType.ChangeDirection:
                    gameBlock = new GameBlockChangeDirection(this, row, column);
                    (gameBlock as GameBlockChangeDirection).ForceDirection = changeDirection;
                    break;

                case GameBlockType.ExtraMove:
                    gameBlock = new GameBlockExtraMove(this, row, column);
                    break;

                case GameBlockType.MultipleMoves:
                    gameBlock = new GameBlockMultipleMoves(this, row, column);
                    (gameBlock as GameBlockMultipleMoves).NumberOfMovesNeeded = numberOfMultipleMoves;
                    (gameBlock as GameBlockMultipleMoves).NumberOfMovesApplied = numberOfMultipleMoves;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("gameBlockType");
            }

            return gameBlock;
        }

        public string GetBoardString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = -1; i < this.gameBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < this.gameBlocks.GetLength(1); j++)
                {
                    if (i == -1)
                    {
                        if (j == 0)
                        {
                            stringBuilder.Append(j.ToString(BlockStrings.NullBlockBoardString));
                        }

                        stringBuilder.Append(j.ToString("D2"));
                    }
                    else
                    {
                        if (j == 0)
                        {
                            stringBuilder.Append(i.ToString("D2"));
                        }

                        IGameBlock gameBlock = this.gameBlocks[i, j];
                        if (gameBlock == null)
                        {
                            stringBuilder.Append("  ");
                        }
                        else
                        {
                            stringBuilder.Append(gameBlock.GetBlockString());
                        }
                    }
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public string GetBoardStringFull()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = -1; i < this.gameBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < this.gameBlocks.GetLength(1); j++)
                {
                    if (i == -1)
                    {
                        if (j == 0)
                        {
                            stringBuilder.Append(j.ToString(BlockStrings.NullBlockBoardString));
                        }

                        stringBuilder.Append(j.ToString("D2"));
                    }
                    else
                    {
                        if (j == 0)
                        {
                            stringBuilder.Append(i.ToString("D2"));
                        }

                        IGameBlock gameBlock = this.gameBlocks[i, j];
                        if (gameBlock == null)
                        {
                            stringBuilder.Append("  ");
                        }
                        else
                        {
                            stringBuilder.Append(gameBlock.GetBlockStringFull());
                        }
                    }
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public string GetBoardStringFullHashed()
        {
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            sha.Initialize();

            string longString = this.GetBoardStringFull();
            byte[] bytes = Encoding.ASCII.GetBytes(longString);
            byte[] hash = sha.ComputeHash(bytes);

            StringBuilder shortString = new StringBuilder();

            // Just take the first 8 bytes.  It's good enough.
            for (int i = 0; i < 8; i++)
            {
                byte b = hash[i];
                shortString.Append(b.ToString("X2"));
            }

            return shortString.ToString();
        }

#if UNITY_IOS
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GameBlocks", this.gameBlocks);
            info.AddValue("Successes", this.successes);
            info.AddValue("Failures", this.failures);
        }
#endif

        public string GetBoardStringLong()
        {
            StringBuilder stringBuilder = new StringBuilder();

            IGameBlock[,] trimmedBoard = this.GetTrimmedBoard();

            for (int i = 0; i < trimmedBoard.GetLength(0); i++)
            {
                for (int j = 0; j < trimmedBoard.GetLength(1); j++)
                {
                    IGameBlock gameBlock = trimmedBoard[i, j];
                    if (gameBlock == null)
                    {
                        stringBuilder.Append(BlockStrings.NullBlock);
                    }
                    else
                    {
                        stringBuilder.Append(gameBlock.GetBlockString());
                    }
                }
            }

            return stringBuilder.ToString();
        }

        public string GetBoardDestinationStringLong()
        {
            StringBuilder stringBuilder = new StringBuilder();

            IGameBlock[,] trimmedBoard = this.GetTrimmedBoard();

            for (int i = 0; i < trimmedBoard.GetLength(0); i++)
            {
                for (int j = 0; j < trimmedBoard.GetLength(1); j++)
                {
                    IGameBlock gameBlock = trimmedBoard[i, j];
                    if (gameBlock == null || gameBlock is GameBlockPlayer)
                    {
                        stringBuilder.Append(BlockStrings.NullBlock);
                    }
                    else
                    {
                        stringBuilder.Append(gameBlock.GetBlockString());
                    }
                }
            }

            return stringBuilder.ToString();
        }
        
        public string GetBoardDestinationStringShort()
        {
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            sha.Initialize();

            string longString = this.GetBoardDestinationStringLong();
            byte[] bytes = Encoding.ASCII.GetBytes(longString);
            byte[] hash = sha.ComputeHash(bytes);

            StringBuilder shortString = new StringBuilder();

            // Just take the first 8 bytes.  It's good enough.
            for (int i = 0; i < 8; i++)
            {
                byte b = hash[i];
                shortString.Append(b.ToString("X2"));
            }

            return shortString.ToString();
        }

        public string GetBoardStringShort()
        {
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            sha.Initialize();

            string longString = this.GetBoardStringLong();
            byte[] bytes = Encoding.ASCII.GetBytes(longString);
            byte[] hash = sha.ComputeHash(bytes);

            StringBuilder shortString = new StringBuilder();

            // Just take the first 8 bytes.  It's good enough.
            for (int i = 0; i < 8; i++)
            {
                byte b = hash[i];
                shortString.Append(b.ToString("X2"));
            }

            return shortString.ToString();
        }

        public HashSet<Type> GetContainingBlockTypes()
        {
            HashSet<Type> blockTypes = new HashSet<Type>();

            foreach (IGameBlock gameBlock in gameBlocks)
            {
                if (gameBlock == null)
                {
                    continue;
                }

                Type type = gameBlock.GetType();

                if (!blockTypes.Contains(type))
                {
                    blockTypes.Add(type);
                }
            }

            return blockTypes;
        }

        public string GetContainBlockTypesString()
        {
            StringBuilder typeString = new StringBuilder();

            foreach (Type type in this.GetContainingBlockTypes().OrderBy(t => t.Name))
            {
                // Ignore these types
                if (type == typeof(GameBlockPlayer) || type == typeof(GameBlockNull) || type == typeof(GameBlockNormal))
                {
                    continue;
                }

                GameBlockBase instance = Activator.CreateInstance(type, 0, 0) as GameBlockBase;
                if (instance != null)
                {
                    string name = instance.GetBlockString();

                    if (typeString.Length == 0)
                    {
                        typeString.Append(name);
                    }
                    else
                    {
                        typeString.AppendFormat(", {0}", name);
                    }
                }
            }

            return typeString.ToString();
        }

        public string GetSolutionString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BlockMovement current = this.solutionStartMove;

            while (current != null)
            {
                if (current.SourceBlock != null)
                {
                    stringBuilder.AppendLine(
                        string.Format(
                            "Position: {0}, {1}  Number of Moves: {2}  Direction: {3}",
                            current.SourceBlock.IndexRow,
                            current.SourceBlock.IndexColumn,
                            current.SourceBlock.AvailableMoves,
                            current.Direction));
                }

                current = current.NextMove;
            }

            return stringBuilder.ToString();
        }

        public IGameBlock[,] GetTrimmedBoard()
        {
            // Don't use full null rows and columns
            int startingRow = this.gameBlocks.GetLength(0);
            int endingRow = 0;
            int startingColumn = this.gameBlocks.GetLength(1);
            int endingColumn = 0;

            for (int i = 0; i < this.gameBlocks.GetLength(0); i++)
            {
                bool allNull = true;
                for (int j = 0; j < this.gameBlocks.GetLength(1); j++)
                {
                    IGameBlock gameBlock = this.gameBlocks[i, j];
                    if (gameBlock != null && !(gameBlock is GameBlockNull))
                    {
                        allNull = false;
                        break;
                    }
                }

                if (!allNull)
                {
                    startingRow = i;
                    break;
                }
            }

            for (int i = this.gameBlocks.GetLength(0) - 1; i >= 0; i--)
            {
                bool allNull = true;
                for (int j = 0; j < this.gameBlocks.GetLength(1); j++)
                {
                    IGameBlock gameBlock = this.gameBlocks[i, j];
                    if (gameBlock != null && !(gameBlock is GameBlockNull))
                    {
                        allNull = false;
                        break;
                    }
                }

                if (!allNull)
                {
                    endingRow = i + 1;
                    break;
                }
            }

            for (int i = 0; i < this.gameBlocks.GetLength(1); i++)
            {
                bool allNull = true;
                for (int j = 0; j < this.gameBlocks.GetLength(0); j++)
                {
                    IGameBlock gameBlock = this.gameBlocks[j, i];
                    if (gameBlock != null && !(gameBlock is GameBlockNull))
                    {
                        allNull = false;
                        break;
                    }
                }

                if (!allNull)
                {
                    startingColumn = i;
                    break;
                }
            }

            for (int i = this.gameBlocks.GetLength(1) - 1; i >= 0; i--)
            {
                bool allNull = true;
                for (int j = 0; j < this.gameBlocks.GetLength(0); j++)
                {
                    IGameBlock gameBlock = this.gameBlocks[j, i];
                    if (gameBlock != null && !(gameBlock is GameBlockNull))
                    {
                        allNull = false;
                        break;
                    }
                }

                if (!allNull)
                {
                    endingColumn = i + 1;
                    break;
                }
            }

            int totalRows = endingRow - startingRow;
            int totalColumns = endingColumn - startingColumn;
            IGameBlock[,] blocks = new IGameBlock[totalRows,totalColumns];

            for (int i = startingRow, iNew = 0; i < endingRow; i++, iNew++)
            {
                for (int j = startingColumn, jNew = 0; j < endingColumn; j++, jNew++)
                {
                    blocks[iNew, jNew] = this.gameBlocks[i, j];
                }
            }

            return blocks;
        }
        
        public bool HasOrphan()
        {
            bool hasOrphan = false;

            foreach (IGameBlock gameBlock in this.gameBlocks)
            {
                IGameBlockDestination gameBlockNormal = gameBlock as IGameBlockDestination;

                if (gameBlockNormal != null && gameBlockNormal.IsOrphaned())
                {
                    hasOrphan = true;
                    break;
                }
            }

            return hasOrphan;
        }

        public bool IsSolved()
        {
            return this.gameBlocks.OfType<IGameBlockDestination>().All(b => !b.IsAvailable);
        }

        public bool IsUntouchedPuzzle()
        {
            return this.gameBlocks.OfType<IGameBlockDestination>().All(b => b.IsFullyAvailable);
        }

        public void NullZeroPlayers()
        {
            for (int i = 0; i < this.gameBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < this.gameBlocks.GetLength(1); j++)
                {
                    IGameBlockParent gameBlockParent = this.gameBlocks[i, j] as IGameBlockParent;
                    if (gameBlockParent != null && gameBlockParent.AvailableMoves == 0)
                    {
                        this.gameBlocks[i, j] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Plays the solution.
        /// </summary>
        /// <returns>True if the solution solved it correctly.</returns>
        public bool PlaySolution()
        {
            BlockMovement current = this.solutionStartMove;

            while (current != null)
            {
                if (current.SourceBlock != null)
                {
                    BlockMovement move = new BlockMovement(current.SourceBlock, current.Direction);
                    if (!current.SourceBlock.ApplyMove(null, current.Direction, move))
                    {
                        // Invalid move?  Break out.
                        break;
                    }
                }

                current = current.NextMove;
            }

            // Finished applying all moves, so test if it's actually solved
            return this.IsSolved();
        }

        /// <summary>
        /// Makes sure the GameBlock's GameBoard reference is this object.
        /// </summary>
        public void SyncWithGameBlocks()
        {
            foreach (IGameBlock gameBlock in this.gameBlocks)
            {
                if (gameBlock != null)
                {
                    gameBlock.GameBoard = this;
                }
            }
        }

        public bool TestCanReverseSolve()
        {
            if (this.HasOrphan())
            {
                return false;
            }

            else
            {
                return true;
            }

            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove =
                new Dictionary<IGameBlockDestination, IGameBlockDestination>();

            List<IGameBlockDestination> gameBlockDestinations =
                this.GameBlocks.OfType<IGameBlockDestination>().Where(g => !g.IsFullyAvailable).ToList();

            foreach (IGameBlockDestination destination in gameBlockDestinations)
            {
                if (!blocksCanReverseMove.ContainsKey(destination))
                {
                    Dictionary<IGameBlock, IGameBlock> previousBlocks = new Dictionary<IGameBlock, IGameBlock>();
                    if (!destination.TestReverseMoves(blocksCanReverseMove, previousBlocks))
                    {
                        // If one of the reverse moves fails, we don't need to test it anymore since
                        // 1 failure means the whole puzzle can't reverse solve.
                        break;
                    }
                }
            }

            return blocksCanReverseMove.Count >= gameBlockDestinations.Count;
        }

        public override string ToString()
        {
            return this.GetBoardStringShort();
        }

        #endregion
    }
}