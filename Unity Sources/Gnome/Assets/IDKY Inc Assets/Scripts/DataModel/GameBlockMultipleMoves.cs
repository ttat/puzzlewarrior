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

    using UnityEngine;

    [Serializable]
    public class GameBlockMultipleMoves : GameBlockBase, IGameBlockDestination
    {
        #region Constants

        private const BlockAnimationType MoveCanceledAnimation = BlockAnimationType.MultipleMovesCanceled;

        private const BlockAnimationType SmashDownAnimation = BlockAnimationType.MultipleMovesSmashDown;

        private const BlockAnimationType SmashLeftAnimation = BlockAnimationType.MultipleMovesSmashLeft;

        private const BlockAnimationType SmashRightAnimation = BlockAnimationType.MultipleMovesSmashRight;

        private const BlockAnimationType SmashUpAnimation = BlockAnimationType.MultipleMovesSmashUp;

        #endregion

        #region Fields

        private MovementDirection lastApplyMoveDirection;

        private IGameBlockParent lastApplyMoveParentBlock;

        private int numberOfMovesApplied;

        private int numberOfMovesNeeded;

        /// <summary>
        /// Used for CountUsedBlocks().  Since this block can have multiple blocks available, it'll need
        /// to add an additional block each time until all of it is used.
        /// </summary>
        private int tempCountUsedBlocks;

        #endregion

        #region Constructors and Destructors
        
        /// <summary>
        /// For XML Serialization.
        /// </summary>
        private GameBlockMultipleMoves()
            : base()
        {
        }

        public GameBlockMultipleMoves(GameBoard gameBoard, int indexRow, int indexColumn)
            : base(gameBoard, indexRow, indexColumn)
        {
        }

        public GameBlockMultipleMoves(int indexRow, int indexColumn)
            : base(indexRow, indexColumn)
        {
        }

        public GameBlockMultipleMoves(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            this.NumberOfMovesNeeded = info.GetInt32("NumberOfMovesNeeded");
            this.NumberOfMovesApplied = info.GetInt32("NumberOfMovesApplied");
        }

        #endregion

        #region Public Properties

        [XmlIgnore]
        public override BlockAnimationType IdleAnimation
        {
            get
            {
                return this.GetCurrentMoveAppliedAnimation();
            }
        }

        [XmlIgnore]
        public bool IsAvailable
        {
            get
            {
                return this.numberOfMovesApplied != this.numberOfMovesNeeded;
            }
        }

        [XmlIgnore]
        public bool IsFullyAvailable
        {
            get
            {
                return this.numberOfMovesApplied == 0;
            }
        }

        [XmlIgnore]
        public int NumberAvailable
        {
            get
            {
                return this.numberOfMovesNeeded - this.numberOfMovesApplied;
            }
        }

        [XmlIgnore]
        public int NumberUsed
        {
            get
            {
                return this.numberOfMovesApplied;
            }
        }

        [XmlAttribute("NumberOfMovesApplied")]
        public int NumberOfMovesApplied
        {
            get
            {
                return this.numberOfMovesApplied;
            }
            set
            {
                this.numberOfMovesApplied = value;
            }
        }

        [XmlAttribute("NumberOfMovesNeeded")]
        public int NumberOfMovesNeeded
        {
            get
            {
                return this.numberOfMovesNeeded;
            }
            set
            {
                this.numberOfMovesNeeded = value;
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
            if (this.IsAvailable)
            {
                this.NotifySetAnimation(this.GetCurrentMoveStartedAnimation(), true, false);

                this.SetAvailability(false);
                parentBlock.AvailableMoves--;
                usedThisBlock = true;

                // No more moves left, so we're done
                if (parentBlock.AvailableMoves == 0)
                {
                    this.NotifySetAnimation(this.GetCurrentMoveAppliedAnimation(), false, false);
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
                this.SetAvailability(true);
                parentBlock.AvailableMoves++;
            }

            if (usedThisBlock)
            {
                // If it used this block, either show the block as applied, or as canceled
                if (moveApplied)
                {
                    move.IntermediateBlocks.Insert(0, this);

                    // Applied, so show the block as applied
                    this.NotifySetAnimation(this.GetCurrentMoveAppliedAnimation(), false, false);
                }
                else
                {
                    // If it was canceled, show the canceled animation, then back to the previous animation
                    this.NotifySetAnimation(MoveCanceledAnimation, false, false);
                    this.NotifySetAnimation(this.GetCurrentMoveAppliedAnimation(), false, true);
                }
            }
            else
            {
                this.NotifySetAnimation(this.GetCurrentMoveAppliedAnimation(), false, true);
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
            if (destinationBlock != null && this.numberOfMovesApplied == 0)
            {
                return false;
            }

            // If this is the destination
            bool thisIsFinalMove = destinationBlock == null;
            int availableBlocksToUse = this.CountUsedBlocks(directionInReverse, new Dictionary<IGameBlock, IGameBlock>());

            // See if there are enough blocks to do this move
            if (thisIsFinalMove && availableBlocksToUse < numberOfBlocksToMove - 1)
            {
                return false;
            }

            bool isThisBlockUsedInLoop = ReferenceEquals(destinationBlock, this) || blockHistory.Contains(this);

            blockHistory.Enqueue(this);

            // Use this block as the destination block if destinationBlock is null
            IGameBlockDestination destination = destinationBlock ?? this;

            bool moveApplied;
            switch (directionInReverse)
            {
                default:
                case MovementDirection.Up:
                    if (this.numberOfMovesApplied == numberOfMovesNeeded && !isThisBlockUsedInLoop)
                    {
                        if (this.Top == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Top.ApplyReverseMoveForLoop(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }

                        // Loop failed, so try non-looped
                        if (!moveApplied && this.Bottom != null)
                        {
                            moveApplied = this.Bottom.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }
                    else
                    {
                        if (this.Bottom == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Bottom.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }

                    break;

                case MovementDirection.Down:
                    if (this.numberOfMovesApplied == numberOfMovesNeeded && !isThisBlockUsedInLoop)
                    {
                        if (this.Bottom == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Bottom.ApplyReverseMoveForLoop(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }

                        // Loop failed, so try non-looped
                        if (!moveApplied && this.Top != null)
                        {
                            moveApplied = this.Top.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }
                    else
                    {
                        if (this.Top == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Top.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }

                    break;

                case MovementDirection.Left:
                    if (this.numberOfMovesApplied == numberOfMovesNeeded && !isThisBlockUsedInLoop)
                    {
                        if (this.Left == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Left.ApplyReverseMoveForLoop(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }

                        // Loop failed, so try non-looped
                        if (!moveApplied && this.Right != null)
                        {
                            moveApplied = this.Right.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }
                    else
                    {
                        if (this.Right == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Right.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }

                    break;

                case MovementDirection.Right:
                    if (this.numberOfMovesApplied == numberOfMovesNeeded && !isThisBlockUsedInLoop)
                    {
                        if (this.Right == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Right.ApplyReverseMoveForLoop(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }

                        // Loop failed, so try non-looped
                        if (!moveApplied && this.Left != null)
                        {
                            moveApplied = this.Left.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }
                    else
                    {
                        if (this.Left == null)
                        {
                            moveApplied = false;
                        }
                        else
                        {
                            moveApplied = this.Left.ApplyReverseMove(
                                destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                        }
                    }

                    break;
            }

            if (moveApplied && !this.IsFullyAvailable && move != null)
            {
                this.SetAvailability(true);
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

            // Starting with a non-filled block isn't valid
            if (destinationBlock != null && this.numberOfMovesApplied == 0)
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

            // Use this block as the destination block if destinationBlock is null
            IGameBlockDestination destination = destinationBlock ?? this;

            blockHistory.Enqueue(this);

            bool moveApplied;
            switch (directionInReverse)
            {
                default:
                case MovementDirection.Up:
                    if (this.Top == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Top.ApplyReverseMoveForLoop(
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                    }
                    break;

                case MovementDirection.Down:
                    if (this.Bottom == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Bottom.ApplyReverseMoveForLoop(
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
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
                            destination, directionInReverse, numberOfBlocksToMove, out move, this, this, blockHistory);
                    }
                    break;
            }

            if (moveApplied && !this.IsFullyAvailable && move != null)
            {
                this.SetAvailability(true);
                move.InsertDestinationBlock(this);
                move.SourceBlock.AvailableMoves++;
            }

            return moveApplied;
        }

        public override void Copy(IGameBlock source)
        {
            GameBlockMultipleMoves gameBlockMultipleMoves = source as GameBlockMultipleMoves;

            if (gameBlockMultipleMoves != null)
            {
                this.numberOfMovesApplied = gameBlockMultipleMoves.numberOfMovesApplied;
                this.numberOfMovesNeeded = gameBlockMultipleMoves.numberOfMovesNeeded;
            }
        }

        public override object Clone()
        {
            GameBlockMultipleMoves clone = new GameBlockMultipleMoves(this.IndexRow, this.IndexColumn);
            clone.numberOfMovesApplied = this.numberOfMovesApplied;
            clone.numberOfMovesNeeded = this.numberOfMovesNeeded;

            return clone;
        }

        public override int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            int countUsed = this.numberOfMovesApplied;

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
            return string.Format("{0}{1}", BlockStrings.MultipleMoveBlock, this.NumberOfMovesNeeded);
        }

        public override string GetBlockStringFull()
        {
            return string.Format("{0}{1}{2}", BlockStrings.MultipleMoveBlock, this.NumberOfMovesNeeded, this.NumberOfMovesApplied);
        }

#if UNITY_IOS
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("NumberOfMovesNeeded", this.NumberOfMovesNeeded);
            info.AddValue("NumberOfMovesApplied", this.NumberOfMovesApplied);
        }
#endif

        public bool CanBeRemovedIntermediate()
        {
            return this.numberOfMovesApplied == 1;
        }

        public bool IsOrphaned()
        {
            if (!this.IsAvailable)
            {
                return false;
            }

            bool leftNotAvailable = this.Left == null || this.Left is GameBlockNull || (this.Left is IGameBlockDestination && !(this.Left as IGameBlockDestination).IsAvailable) || (this.Left is IGameBlockParent && (this.Left as IGameBlockParent).AvailableMoves > 0);
            bool rightNotAvailable = this.Right == null || this.Right is GameBlockNull || (this.Right is IGameBlockDestination && !(this.Right as IGameBlockDestination).IsAvailable) || (this.Right is IGameBlockParent && (this.Right as IGameBlockParent).AvailableMoves > 0);
            bool bottomNotAvailable = this.Bottom == null || this.Bottom is GameBlockNull || (this.Bottom is IGameBlockDestination && !(this.Bottom as IGameBlockDestination).IsAvailable) || (this.Bottom is IGameBlockParent && (this.Bottom as IGameBlockParent).AvailableMoves > 0);
            bool topNotAvailable = this.Top == null || this.Top is GameBlockNull || (this.Top is IGameBlockDestination && !(this.Top as IGameBlockDestination).IsAvailable) || (this.Top is IGameBlockParent && (this.Top as IGameBlockParent).AvailableMoves > 0);

            return leftNotAvailable && rightNotAvailable && bottomNotAvailable && topNotAvailable;
            //return (this.Left == null) && (this.Right == null) && (this.Bottom == null) && (this.Top == null);
        }

        public void SetAvailability(bool available)
        {
            if (available)
            {
                if (this.numberOfMovesApplied > 0)
                {
                    this.numberOfMovesApplied--;
                }
            }
            else
            {
                if (this.numberOfMovesApplied < this.numberOfMovesNeeded)
                {
                    this.numberOfMovesApplied++;
                }
            }
        }

        public override bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            // This block can't reverse solve if it's currently available.
            if (this.IsFullyAvailable)
            {
                return false;
            }

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
                blocksCanReverseMove[this] = this;
                return true;
            }

            return false;
        }

        public void Undo()
        {
            this.SetAvailability(true);

            this.NotifySetAnimation(this.GetCurrentMoveAppliedAnimation(), true, true);
        }

        #endregion

        #region Methods

        private BlockAnimationType GetCurrentMoveAppliedAnimation()
        {
            BlockAnimationType blockAnimationType;
            int current = this.numberOfMovesNeeded - this.numberOfMovesApplied;

            switch (current)
            {
                default:
                case 0:
                    blockAnimationType = BlockAnimationType.MultipleMovesAppliedNeed0;
                    break;

                case 1:
                    blockAnimationType = BlockAnimationType.MultipleMovesAppliedNeed1;
                    break;

                case 2:
                    blockAnimationType = BlockAnimationType.MultipleMovesAppliedNeed2;
                    break;

                case 3:
                    blockAnimationType = BlockAnimationType.MultipleMovesAppliedNeed3;
                    break;

                case 4:
                    blockAnimationType = BlockAnimationType.MultipleMovesAppliedNeed4;
                    break;

                case 5:
                    blockAnimationType = BlockAnimationType.MultipleMovesAppliedNeed5;
                    break;
            }

            return blockAnimationType;
        }

        private BlockAnimationType GetCurrentMoveStartedAnimation()
        {
            BlockAnimationType blockAnimationType;
            int current = this.numberOfMovesNeeded - this.numberOfMovesApplied;
            
            switch (current)
            {
                case 1:
                    blockAnimationType = BlockAnimationType.MultipleMovesStartedNeed1;
                    break;

                case 2:
                    blockAnimationType = BlockAnimationType.MultipleMovesStartedNeed2;
                    break;

                case 3:
                    blockAnimationType = BlockAnimationType.MultipleMovesStartedNeed3;
                    break;

                case 4:
                    blockAnimationType = BlockAnimationType.MultipleMovesStartedNeed4;
                    break;

                case 5:
                    blockAnimationType = BlockAnimationType.MultipleMovesStartedNeed5;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return blockAnimationType;
        }

        #endregion
    }
}