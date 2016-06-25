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
    public class GameBlockNormal : GameBlockBase, IGameBlockDestination
    {
        #region Constants

        private const BlockAnimationType MoveAppliedAnimation = BlockAnimationType.NormalBlockMovedApplied;

        private const BlockAnimationType MoveCanceledAnimation = BlockAnimationType.NormalBlockMovedCanceled;

        private const BlockAnimationType MoveStartedAnimation = BlockAnimationType.NormalBlockMovedStarted;

        private const BlockAnimationType SmashDownAnimation = BlockAnimationType.NormalBlockSmashDown;

        private const BlockAnimationType SmashLeftAnimation = BlockAnimationType.NormalBlockSmashLeft;

        private const BlockAnimationType SmashRightAnimation = BlockAnimationType.NormalBlockSmashRight;

        private const BlockAnimationType SmashUpAnimation = BlockAnimationType.NormalBlockSmashUp;

        #endregion

        #region Fields

        private bool isAvailable;

        private MovementDirection lastApplyMoveDirection;

        private IGameBlockParent lastApplyMoveParentBlock;

        #endregion

        #region Constructors and Destructors
        
        /// <summary>
        /// For XML Serialization.
        /// </summary>
        private GameBlockNormal()
            : base()
        {
        }

        public GameBlockNormal(GameBoard gameBoard, int indexRow, int indexColumn)
            : base(gameBoard, indexRow, indexColumn)
        {
        }

        public GameBlockNormal(int indexRow, int indexColumn)
            : base(indexRow, indexColumn)
        {
        }

        public GameBlockNormal(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            this.isAvailable = info.GetBoolean("IsAvailable");
        }

        #endregion

        #region Public Properties

        [XmlIgnore]
        public override BlockAnimationType IdleAnimation
        {
            get
            {
                return BlockAnimationType.NormalBlockIdle;
            }
        }

        [XmlAttribute("IsAvailable")]
        public bool IsAvailable
        {
            get
            {
                return this.isAvailable;
            }

            set
            {
                this.isAvailable = value;
            }
        }

        [XmlIgnore]
        public bool IsFullyAvailable
        {
            get
            {
                return this.IsAvailable;
            }
        }

        [XmlIgnore]
        public int NumberAvailable
        {
            get
            {
                return this.isAvailable ? 1 : 0;
            }
        }

        [XmlIgnore]
        public int NumberUsed
        {
            get
            {
                return this.isAvailable ? 0 : 1;
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

            // If it's trying to do the same move again on this block, it's probably stuck in a loop.
            if (this.lastApplyMoveParentBlock == parentBlock && this.lastApplyMoveDirection == direction)
            {
                return false;
            }

            this.lastApplyMoveParentBlock = parentBlock;
            this.lastApplyMoveDirection = direction;

            bool usedThisBlock = false;

            // This block is available, so fill it
            if (this.isAvailable)
            {
                this.NotifySetAnimation(MoveStartedAnimation, true, false);

                this.IsAvailable = false;
                parentBlock.AvailableMoves--;
                usedThisBlock = true;

                // No more moves left, so we're done
                if (parentBlock.AvailableMoves == 0)
                {
                    this.NotifySetAnimation(MoveAppliedAnimation, false, false);
                    this.lastApplyMoveParentBlock = null;
                    move.DestinationBlock = this;
                    return true;
                }
            }

            // Otherwise, move to the next block
            bool moveApplied;

            switch (direction)
            {
                default:
                case MovementDirection.Up:
                    if (!usedThisBlock)
                    {
                        this.NotifySetAnimation(SmashUpAnimation, true, false);
                    }

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
                    if (!usedThisBlock)
                    {
                        this.NotifySetAnimation(SmashDownAnimation, true, false);
                    }

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
                    if (!usedThisBlock)
                    {
                        this.NotifySetAnimation(SmashLeftAnimation, true, false);
                    }

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
                    if (!usedThisBlock)
                    {
                        this.NotifySetAnimation(SmashRightAnimation, true, false);
                    }

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

            // Used this block, but failed the child blocks, so undo the move
            if (usedThisBlock && !moveApplied)
            {
                this.IsAvailable = true;
                parentBlock.AvailableMoves++;
            }

            if (usedThisBlock)
            {
                // If it used this block, either show the block as applied, or as canceled
                this.NotifySetAnimation(moveApplied ? MoveAppliedAnimation : MoveCanceledAnimation, false, false);

                if (moveApplied)
                {
                    move.IntermediateBlocks.Insert(0, this);
                }
            }
            else
            {
                // It didn't use this block, so it was already applied.  Put back the applied animation.
                this.NotifySetAnimation(MoveAppliedAnimation, false, true);
            }

            this.lastApplyMoveParentBlock = null;
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

            // Starting with a non-filled block isn't valid
            if (destinationBlock != null && this.isAvailable)
            {
                return false;
            }

            // If this is the destination
            bool thisIsFinalMove = destinationBlock == null;
            int availableBlocksToUse = this.CountUsedBlocks(directionInReverse, new Dictionary<IGameBlock, IGameBlock>());

            // See if there are enough blocks to do this move
            if (thisIsFinalMove && availableBlocksToUse < numberOfBlocksToMove)
            {
                return false;
            }
            else if (destinationBlock == this)
            {
                // If it came back around to this block again, then it's in a loop.  It needs to break out.
                return false;
            }
            else if (blockHistory.Contains(this))
            {
                // If it came back around to this block again, then it's in a loop.  It needs to break out.
                return false;
            }

            blockHistory.Enqueue(this);

            // Use this block as the destination block if destinationBlock is null
            IGameBlockDestination destination = destinationBlock ?? this;

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
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                    }
                    break;
            }

            if (moveApplied && !this.isAvailable && move != null)
            {
                this.IsAvailable = true;
                move.InsertDestinationBlock(this);
                move.SourceBlock.AvailableMoves++;
            }

            return moveApplied;
        }

        public override bool ApplyReverseMoveForLoop(
            IGameBlockDestination destinationBlock,
            MovementDirection directionInReverse,
            int numberOfBlocksToMove,
            out BlockMovement move,
            IGameBlock previousBlock,
            IGameBlockDestination previousDestination,
            Queue<IGameBlock> blockHistory)
        {
            move = null;

            // There should already be a destination block
            if (destinationBlock == null)
            {
                return false;
            }

            blockHistory.Enqueue(this);

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
                            destinationBlock, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destinationBlock, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destinationBlock, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destinationBlock, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                    }
                    break;
            }

            if (moveApplied && !this.isAvailable && move != null)
            {
                this.IsAvailable = true;
                move.InsertDestinationBlock(this);
                move.SourceBlock.AvailableMoves++;
            }

            return moveApplied;
        }

        public override void Copy(IGameBlock source)
        {
            GameBlockNormal gameBlockNormal = source as GameBlockNormal;

            if (gameBlockNormal != null)
            {
                this.isAvailable = gameBlockNormal.isAvailable;
            }
        }

        public override object Clone()
        {
            GameBlockNormal clone = new GameBlockNormal(this.IndexRow, this.IndexColumn);
            clone.isAvailable = this.isAvailable;

            return clone;
        }

        public override int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            int countUsed = this.IsAvailable ? 0 : 1;

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
            return BlockStrings.NormalBlock;
        }

        public override string GetBlockStringFull()
        {
            return BlockStrings.NormalBlock + (this.IsAvailable ? "1" : "0");
        }

        public override bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            // This block can't reverse solve if it's currently available.
            if (this.IsAvailable || previousBlocks.ContainsKey(this))
            {
                return false;
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
                blocksCanReverseMove[this] = this;
                return true;
            }

            return false;
        }

#if UNITY_IOS
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("IsAvailable", this.isAvailable);
        }
#endif

        public bool CanBeRemovedIntermediate()
        {
            return true;
        }

        public bool IsOrphaned()
        {
            bool leftNotAvailable = this.Left == null || this.Left is GameBlockNull
                                    ||
                                    (!this.isAvailable && this.Left is IGameBlockDestination
                                     && (this.Left as IGameBlockDestination).IsAvailable)
                                    ||
                                    (!this.isAvailable && this.Left is IGameBlockParent
                                     && (this.Left as IGameBlockParent).AvailableMoves > 0);
            bool rightNotAvailable = this.Right == null || this.Right is GameBlockNull
                                     ||
                                     (!this.isAvailable && this.Right is IGameBlockDestination
                                      && (this.Right as IGameBlockDestination).IsAvailable)
                                     ||
                                     (!this.isAvailable && this.Right is IGameBlockParent
                                      && (this.Right as IGameBlockParent).AvailableMoves > 0);
            bool bottomNotAvailable = this.Bottom == null || this.Bottom is GameBlockNull
                                      ||
                                      (!this.isAvailable && this.Bottom is IGameBlockDestination
                                       && (this.Bottom as IGameBlockDestination).IsAvailable)
                                      ||
                                      (!this.isAvailable && this.Bottom is IGameBlockParent
                                       && (this.Bottom as IGameBlockParent).AvailableMoves > 0);
            bool topNotAvailable = this.Top == null || this.Top is GameBlockNull
                                   ||
                                   (!this.isAvailable && this.Top is IGameBlockDestination
                                    && (this.Top as IGameBlockDestination).IsAvailable)
                                   ||
                                   (!this.isAvailable && this.Top is IGameBlockParent && (this.Top as IGameBlockParent).AvailableMoves > 0);

            return leftNotAvailable && rightNotAvailable && bottomNotAvailable && topNotAvailable;
        }

        public virtual void SetAvailability(bool available)
        {
            this.IsAvailable = available;
        }

        public void Undo()
        {
            this.SetAvailability(true);

            this.NotifySetAnimation(this.IdleAnimation, true, true);
        }

        #endregion
    }
}