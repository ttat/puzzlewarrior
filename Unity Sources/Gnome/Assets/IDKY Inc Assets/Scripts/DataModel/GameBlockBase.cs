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
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(GameBlockChangeDirection))]
    [XmlInclude(typeof(GameBlockExtraMove))]
    [XmlInclude(typeof(GameBlockMultipleMoves))]
    [XmlInclude(typeof(GameBlockNormal))]
    [XmlInclude(typeof(GameBlockNull))]
    [XmlInclude(typeof(GameBlockPlayer))]
    public abstract class GameBlockBase : IGameBlock
#if UNITY_IOS
        , ISerializable
#endif
    {
        #region Fields

        [field: NonSerialized]
        private GameBoard gameBoard;

        private int indexColumn;

        private int indexRow;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// For XML Serialization.
        /// </summary>
        protected GameBlockBase()
        {
        }

        protected GameBlockBase(int indexRow, int indexColumn)
        {
            this.gameBoard = null;
            this.indexRow = indexRow;
            this.indexColumn = indexColumn;
        }

        protected GameBlockBase(GameBoard gameBoard, int indexRow, int indexColumn)
        {
            this.gameBoard = gameBoard;
            this.gameBoard.GameBlocks[indexRow, indexColumn] = this;
            this.indexRow = indexRow;
            this.indexColumn = indexColumn;
        }

        protected GameBlockBase(SerializationInfo info, StreamingContext ctxt)
        {
            this.indexColumn = info.GetInt32("IndexColumn");
            this.indexRow = info.GetInt32("IndexRow");
        }

        #endregion

        #region Public Events

        [field: NonSerialized]
        public event EventHandler<GameBlockSetAnimationEventArgs> GameBlockSetAnimationEvent;

        #endregion

        #region Public Properties

        [XmlIgnore]
        public IGameBlock Bottom
        {
            get
            {
                if (this.indexRow == this.gameBoard.GameBlocks.GetLength(0) - 1)
                {
                    return null;
                }
                else
                {
                    return this.gameBoard.GameBlocks[this.indexRow + 1, this.indexColumn];
                }
            }
        }

        [XmlIgnore]
        public GameBoard GameBoard
        {
            get
            {
                return this.gameBoard;
            }

            set
            {
                if (this.gameBoard != null)
                {
                    this.gameBoard.GameBlocks[this.indexRow, this.indexColumn] = null;
                }

                this.gameBoard = value;
                this.gameBoard.GameBlocks[this.indexRow, this.indexColumn] = this;
            }
        }

        [XmlIgnore]
        public abstract BlockAnimationType IdleAnimation { get; }

        [XmlAttribute("IndexColumn")]
        public int IndexColumn
        {
            get
            {
                return this.indexColumn;
            }

            set
            {
                this.indexColumn = value;
            }
        }

        [XmlAttribute("IndexRow")]
        public int IndexRow
        {
            get
            {
                return this.indexRow;
            }

            set
            {
                this.indexRow = value;
            }
        }

        [XmlIgnore]
        public IGameBlock Left
        {
            get
            {
                if (this.indexColumn == 0)
                {
                    return null;
                }
                else
                {
                    return this.gameBoard.GameBlocks[this.indexRow, this.indexColumn - 1];
                }
            }
        }

        [XmlIgnore]
        public IGameBlock Right
        {
            get
            {
                if (this.indexColumn == this.gameBoard.GameBlocks.GetLength(1) - 1)
                {
                    return null;
                }
                else
                {
                    return this.gameBoard.GameBlocks[this.indexRow, this.indexColumn + 1];
                }
            }
        }

        [XmlIgnore]
        public abstract string Text { get; }

        [XmlIgnore]
        public IGameBlock Top
        {
            get
            {
                if (this.indexRow == 0)
                {
                    return null;
                }
                else
                {
                    return this.gameBoard.GameBlocks[this.indexRow - 1, this.indexColumn];
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public abstract bool ApplyMove(IGameBlockParent parentBlock, MovementDirection direction, BlockMovement move);

        public abstract bool ApplyReverseMove(
            IGameBlockDestination destinationBlock,
            MovementDirection directionInReverse,
            int numberOfBlocksToMove,
            out BlockMovement move,
            IGameBlock previousBlock,
            IGameBlockDestination previousDestinationBlock,
            Queue<IGameBlock> blockHistory);

        public abstract bool ApplyReverseMoveForLoop(
            IGameBlockDestination destinationBlock,
            MovementDirection directionInReverse,
            int numberOfBlocksToMove,
            out BlockMovement move,
            IGameBlock previousBlock,
            IGameBlockDestination previousDestination,
            Queue<IGameBlock> blockHistory);

        public virtual void Copy(IGameBlock source)
        {
        }

        public abstract object Clone();

        public abstract int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks);

        public abstract string GetBlockString();

        public abstract string GetBlockStringFull();

        public abstract bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks);

#if UNITY_IOS
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IndexRow", this.indexRow);
            info.AddValue("IndexColumn", this.indexColumn);
        }
#endif

        protected void NotifySetAnimation(BlockAnimationType blockAnimationType, bool canOverride, bool disableSounds)
        {
            if (this.GameBlockSetAnimationEvent != null)
            {
                this.GameBlockSetAnimationEvent(this, new GameBlockSetAnimationEventArgs(blockAnimationType, canOverride, disableSounds));
            }
        }

        #endregion
    }
}