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
    public class GameBlockExtraMove : GameBlockBase
    {
        #region Constructors and Destructors
        
        /// <summary>
        /// For XML Serialization.
        /// </summary>
        private GameBlockExtraMove()
            : base()
        {
        }

        public GameBlockExtraMove(GameBoard gameBoard, int indexRow, int indexColumn)
            : base(gameBoard, indexRow, indexColumn)
        {
        }

        public GameBlockExtraMove(int indexRow, int indexColumn)
            : base(indexRow, indexColumn)
        {
        }

        public GameBlockExtraMove(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
        }

        #endregion

        #region Public Properties

        [XmlIgnore]
        public override BlockAnimationType IdleAnimation
        {
            get
            {
                return BlockAnimationType.ExtraBlockIdle;
            }
        }

        [XmlIgnore]
        public override string Text
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override bool ApplyMove(IGameBlockParent parentBlock, MovementDirection direction, BlockMovement move)
        {
            // No moves available, so return false (it shouldn't even reach here under normal circumstances)
            if (parentBlock.AvailableMoves == 0)
            {
                return false;
            }

            bool moveApplied;

            // Add an extra move
            parentBlock.AvailableMoves++;

            this.NotifySetAnimation(BlockAnimationType.ExtraBlockMovedStarted, true, false);

            // Then try to apply the move
            switch (direction)
            {
                default:
                case MovementDirection.Up:
                    if (this.Top == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Top.ApplyMove(parentBlock, direction, move);
                    }
                    break;

                case MovementDirection.Down:
                    if (this.Bottom == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Bottom.ApplyMove(parentBlock, direction, move);
                    }
                    break;

                case MovementDirection.Left:
                    if (this.Left == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Left.ApplyMove(parentBlock, direction, move);
                    }
                    break;

                case MovementDirection.Right:
                    if (this.Right == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Right.ApplyMove(parentBlock, direction, move);
                    }
                    break;
            }

            // Undo the addition of the extra move if the move wasn't applied
            if (!moveApplied)
            {
                this.NotifySetAnimation(BlockAnimationType.ExtraBlockMovedCanceled, false, false);
                parentBlock.AvailableMoves--;
            }

            this.NotifySetAnimation(BlockAnimationType.ExtraBlockIdle, false, false);

            return moveApplied;
        }

        public override bool ApplyReverseMove(
            IGameBlockDestination destinationBlock,
            MovementDirection directionInReverse,
            int numberOfBlocksToMove,
            out BlockMovement move,
            IGameBlock previousBlock,
            IGameBlockDestination previousDestinationBlock,
            Queue<IGameBlock> blockHistory)
        {
            move = null;

            // This can't be the final destination block
            if (destinationBlock == null)
            {
                return false;
            }

            // It needs at least 2 previous blocks
            if (previousDestinationBlock == null)
            {
                return false;
            }

            if (destinationBlock == previousDestinationBlock)
            {
                return false;
            }

            bool moveApplied;
            switch (directionInReverse)
            {
                default:
                case MovementDirection.Up:
                    if (this.Bottom == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Bottom.ApplyReverseMove(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;

                case MovementDirection.Down:
                    if (this.Top == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Top.ApplyReverseMove(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;

                case MovementDirection.Left:
                    if (this.Right == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Right.ApplyReverseMove(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;

                case MovementDirection.Right:
                    if (this.Left == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Left.ApplyReverseMove(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;
            }

            if (moveApplied)
            {
                // It requires 1 less move if this block is used
                move.SourceBlock.AvailableMoves--;
            }

            return moveApplied;
        }

        public override bool ApplyReverseMoveForLoop(
            IGameBlockDestination destinationBlock,
            MovementDirection directionInReverse,
            int numberOfBlocksToMove,
            out BlockMovement move,
            IGameBlock previousBlock,
            IGameBlockDestination previousDestinationBlock,
            Queue<IGameBlock> blockHistory)
        {
            move = null;

            // This can't be the final destination block
            if (destinationBlock == null)
            {
                return false;
            }

            // It needs at least 2 previous blocks
            if (previousDestinationBlock == null)
            {
                return false;
            }

            if (destinationBlock == previousDestinationBlock)
            {
                return false;
            }

            bool moveApplied;
            switch (directionInReverse)
            {
                default:
                case MovementDirection.Down:
                    if (this.Bottom == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Bottom.ApplyReverseMoveForLoop(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;

                case MovementDirection.Up:
                    if (this.Top == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Top.ApplyReverseMoveForLoop(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;

                case MovementDirection.Right:
                    if (this.Right == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Right.ApplyReverseMoveForLoop(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;

                case MovementDirection.Left:
                    if (this.Left == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Left.ApplyReverseMoveForLoop(
                            destinationBlock,
                            directionInReverse,
                            numberOfBlocksToMove - 1,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;
            }

            if (moveApplied)
            {
                // It requires 1 less move if this block is used
                move.SourceBlock.AvailableMoves--;
            }

            return moveApplied;
        }

        public override object Clone()
        {
            GameBlockExtraMove clone = new GameBlockExtraMove(this.IndexRow, this.IndexColumn);

            return clone;
        }

        public override int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            int countUsed = 0;

            if (previousBlocks == null)
            {
                previousBlocks = new Dictionary<IGameBlock, IGameBlock>();
            }
            else if (previousBlocks.ContainsKey(this))
            {
                return 0;
            }

            previousBlocks.Add(this, this);

            switch (directionInReverse)
            {
                case MovementDirection.Up:
                    countUsed += this.Bottom == null ? 0 : this.Bottom.CountUsedBlocks(directionInReverse, previousBlocks);
                    break;
                case MovementDirection.Down:
                    countUsed += this.Top == null ? 0 : this.Top.CountUsedBlocks(directionInReverse, previousBlocks);
                    break;
                case MovementDirection.Left:
                    countUsed += this.Right == null ? 0 : this.Right.CountUsedBlocks(directionInReverse, previousBlocks);
                    break;
                case MovementDirection.Right:
                    countUsed += this.Left == null ? 0 : this.Left.CountUsedBlocks(directionInReverse, previousBlocks);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("directionInReverse");
            }

            return countUsed;
        }

        public override string GetBlockString()
        {
            return BlockStrings.ExtraMoveBlock;
        }

        public override string GetBlockStringFull()
        {
            return BlockStrings.ExtraMoveBlock;
        }

        public override bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            // If this gets tested again, maybe there's a loop.  Return true just in case.
            if (previousBlocks.ContainsKey(this))
            {
                return true;
            }

            previousBlocks[this] = this;

            if ((this.Top != null && !previousBlocks.ContainsKey(this.Top)
                 && this.Top.TestReverseMoves(blocksCanReverseMove, previousBlocks))
                ||
                (this.Left != null && !previousBlocks.ContainsKey(this.Left)
                 && this.Left.TestReverseMoves(blocksCanReverseMove, previousBlocks))
                ||
                (this.Right != null && !previousBlocks.ContainsKey(this.Right)
                 && this.Right.TestReverseMoves(blocksCanReverseMove, previousBlocks))
                ||
                (this.Bottom != null && !previousBlocks.ContainsKey(this.Bottom)
                 && this.Bottom.TestReverseMoves(blocksCanReverseMove, previousBlocks)))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}