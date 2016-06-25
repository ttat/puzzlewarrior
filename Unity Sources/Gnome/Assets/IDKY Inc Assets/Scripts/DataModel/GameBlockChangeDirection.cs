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
    public class GameBlockChangeDirection : GameBlockBase
    {
        #region Fields

        private bool applyingMove;

        private MovementDirection forceDirection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// For XML Serialization.
        /// </summary>
        private GameBlockChangeDirection()
            : base()
        {
        }

        public GameBlockChangeDirection(GameBoard gameBoard, int indexRow, int indexColumn)
            : base(gameBoard, indexRow, indexColumn)
        {
        }

        public GameBlockChangeDirection(int indexRow, int indexColumn)
            : base(indexRow, indexColumn)
        {
        }

        public GameBlockChangeDirection(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            this.ForceDirection = (MovementDirection)info.GetInt32("ForceDirection");
        }

        #endregion

        #region Public Properties

        [XmlAttribute("ForceDirection")]
        public MovementDirection ForceDirection
        {
            get
            {
                return this.forceDirection;
            }

            set
            {
                this.forceDirection = value;
            }
        }

        [XmlIgnore]
        public override BlockAnimationType IdleAnimation
        {
            get
            {
                BlockAnimationType animationType;

                switch (this.forceDirection)
                {
                    default:
                    case MovementDirection.Up:
                        animationType = BlockAnimationType.ChangeDirectionUp;
                        break;
                    case MovementDirection.Down:
                        animationType = BlockAnimationType.ChangeDirectionDown;
                        break;
                    case MovementDirection.Left:
                        animationType = BlockAnimationType.ChangeDirectionLeft;
                        break;
                    case MovementDirection.Right:
                        animationType = BlockAnimationType.ChangeDirectionRight;
                        break;
                }

                return animationType;
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
            // If it's already applying the move, this is a loop
            if (this.applyingMove)
            {
                return false;
            }

            this.applyingMove = true;

            // Move to the next block, always in the forced direction
            bool moveApplied;

            switch (this.forceDirection)
            {
                default:
                case MovementDirection.Up:
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionMoveStartedUp, true, false);
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionUp, false, false);

                    if (this.Top == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Top.ApplyMove(parentBlock, this.forceDirection, move);
                    }
                    break;

                case MovementDirection.Down:
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionMoveStartedDown, true, false);
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionDown, false, false);

                    if (this.Bottom == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Bottom.ApplyMove(parentBlock, this.forceDirection, move);
                    }
                    break;

                case MovementDirection.Left:
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionMoveStartedLeft, true, false);
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionLeft, false, false);

                    if (this.Left == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Left.ApplyMove(parentBlock, this.forceDirection, move);
                    }
                    break;

                case MovementDirection.Right:
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionMoveStartedRight, true, false);
                    this.NotifySetAnimation(BlockAnimationType.ChangeDirectionRight, false, false);

                    if (this.Right == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Right.ApplyMove(parentBlock, this.forceDirection, move);
                    }
                    break;
            }
            
            this.applyingMove = false;

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

            // This block can't be the destination
            if (destinationBlock == null)
            {
                return false;
            }

            // This block forces the next move to be a certain direction, so depending on what "forceDirection" is,
            // the next move is always going to be the one in that direction.  As a reverse move, previousBlock will
            // need to be the correct block based on this requirement.
            IGameBlock forcedPreviousBlock;
            switch (this.forceDirection)
            {
                default:
                case MovementDirection.Up:
                    forcedPreviousBlock = this.Top;
                    break;

                case MovementDirection.Down:
                    forcedPreviousBlock = this.Bottom;
                    break;

                case MovementDirection.Left:
                    forcedPreviousBlock = this.Left;
                    break;

                case MovementDirection.Right:
                    forcedPreviousBlock = this.Right;
                    break;
            }

            if (!ReferenceEquals(forcedPreviousBlock, previousBlock))
            {
                return false;
            }

            blockHistory.Enqueue(this);

            // It can be any direction except for the forced direction, so try any one of the three others
            // until one of them works (at random)
            List<MovementDirection> directions = new List<MovementDirection>
                                                     {
                                                         MovementDirection.Down,
                                                         MovementDirection.Up,
                                                         MovementDirection.Left,
                                                         MovementDirection.Right
                                                     };

            switch (this.forceDirection)
            {
                case MovementDirection.Up:
                    directions.Remove(MovementDirection.Down);
                    break;
                case MovementDirection.Down:
                    directions.Remove(MovementDirection.Up);
                    break;
                case MovementDirection.Left:
                    directions.Remove(MovementDirection.Right);
                    break;
                case MovementDirection.Right:
                    directions.Remove(MovementDirection.Left);
                    break;
            }

            // Shuffle
            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                MovementDirection remove = directions[random.Next(0, 2)];
                directions.Remove(remove);
                directions.Add(remove);
            }

            bool moveApplied = false;

            foreach (MovementDirection movementDirectionInReverse in directions)
            {
                switch (movementDirectionInReverse)
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
                                movementDirectionInReverse,
                                numberOfBlocksToMove,
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
                                movementDirectionInReverse,
                                numberOfBlocksToMove,
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
                                movementDirectionInReverse,
                                numberOfBlocksToMove,
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
                                movementDirectionInReverse,
                                numberOfBlocksToMove,
                                out move,
                                this,
                                previousDestinationBlock,
                                blockHistory);
                        }
                        break;
                }

                // One of the moves worked, so break out
                if (moveApplied)
                {
                    break;
                }
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

            // This block can't be the destination
            if (destinationBlock == null)
            {
                return false;
            }

            // This block forces the next move to be a certain direction, so depending on what "forceDirection" is,
            // the next move is always going to be the one in that direction.  As a reverse move, previousBlock will
            // need to be the correct block based on this requirement.
            IGameBlock forcedPreviousBlock;
            switch (this.forceDirection)
            {
                default:
                case MovementDirection.Up:
                    forcedPreviousBlock = this.Top;
                    break;

                case MovementDirection.Down:
                    forcedPreviousBlock = this.Bottom;
                    break;

                case MovementDirection.Left:
                    forcedPreviousBlock = this.Left;
                    break;

                case MovementDirection.Right:
                    forcedPreviousBlock = this.Right;
                    break;
            }

            if (!ReferenceEquals(forcedPreviousBlock, previousBlock))
            {
                return false;
            }

            bool isPossibleLoopDirection;

            // This is trying to make a loop, so it has to be the opposite direction
            switch (this.forceDirection)
            {
                case MovementDirection.Up:
                    isPossibleLoopDirection = directionInReverse == MovementDirection.Down;
                    break;

                case MovementDirection.Down:
                    isPossibleLoopDirection = directionInReverse == MovementDirection.Up;
                    break;

                case MovementDirection.Left:
                    isPossibleLoopDirection = directionInReverse == MovementDirection.Right;
                    break;

                case MovementDirection.Right:
                    isPossibleLoopDirection = directionInReverse == MovementDirection.Left;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!isPossibleLoopDirection)
            {
                return false;
            }

            blockHistory.Enqueue(this);

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
                            numberOfBlocksToMove,
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
                            numberOfBlocksToMove,
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
                            numberOfBlocksToMove,
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
                            numberOfBlocksToMove,
                            out move,
                            this,
                            previousDestinationBlock,
                            blockHistory);
                    }
                    break;
            }

            return moveApplied;
        }

        public override object Clone()
        {
            GameBlockChangeDirection clone = new GameBlockChangeDirection(this.IndexRow, this.IndexColumn);
            clone.forceDirection = this.forceDirection;

            return clone;
        }

        public override int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            int countUsed = 0;
            int bottomCount;
            int rightCount;
            int leftCount;
            int topCount;

            if (previousBlocks == null)
            {
                previousBlocks = new Dictionary<IGameBlock, IGameBlock>();
            }
            else if (previousBlocks.ContainsKey(this))
            {
                return 0;
            }

            previousBlocks.Add(this, this);

            switch (this.forceDirection)
            {
                case MovementDirection.Up:
                    bottomCount = this.Bottom == null || previousBlocks.ContainsKey(this.Bottom)
                                      ? 0
                                      : this.Bottom.CountUsedBlocks(MovementDirection.Up, previousBlocks);
                    rightCount = this.Right == null || previousBlocks.ContainsKey(this.Right)
                                     ? 0
                                     : this.Right.CountUsedBlocks(MovementDirection.Left, previousBlocks);
                    leftCount = this.Left == null || previousBlocks.ContainsKey(this.Left)
                                    ? 0
                                    : this.Left.CountUsedBlocks(MovementDirection.Right, previousBlocks);
                    countUsed += Math.Max(bottomCount, Math.Max(rightCount, leftCount));
                    break;
                case MovementDirection.Down:
                    rightCount = this.Right == null || previousBlocks.ContainsKey(this.Right)
                                     ? 0
                                     : this.Right.CountUsedBlocks(MovementDirection.Left, previousBlocks);
                    leftCount = this.Left == null || previousBlocks.ContainsKey(this.Left)
                                    ? 0
                                    : this.Left.CountUsedBlocks(MovementDirection.Right, previousBlocks);
                    topCount = this.Top == null || previousBlocks.ContainsKey(this.Top)
                                   ? 0
                                   : this.Top.CountUsedBlocks(MovementDirection.Down, previousBlocks);
                    countUsed += Math.Max(topCount, Math.Max(rightCount, leftCount));
                    break;
                case MovementDirection.Left:
                    bottomCount = this.Bottom == null || previousBlocks.ContainsKey(this.Bottom)
                                      ? 0
                                      : this.Bottom.CountUsedBlocks(MovementDirection.Up, previousBlocks);
                    rightCount = this.Right == null || previousBlocks.ContainsKey(this.Right)
                                     ? 0
                                     : this.Right.CountUsedBlocks(MovementDirection.Left, previousBlocks);
                    topCount = this.Top == null || previousBlocks.ContainsKey(this.Top)
                                   ? 0
                                   : this.Top.CountUsedBlocks(MovementDirection.Down, previousBlocks);
                    countUsed += Math.Max(rightCount, Math.Max(bottomCount, topCount));
                    break;
                case MovementDirection.Right:
                    bottomCount = this.Bottom == null || previousBlocks.ContainsKey(this.Bottom)
                                      ? 0
                                      : this.Bottom.CountUsedBlocks(MovementDirection.Up, previousBlocks);
                    leftCount = this.Left == null || previousBlocks.ContainsKey(this.Left)
                                    ? 0
                                    : this.Left.CountUsedBlocks(MovementDirection.Right, previousBlocks);
                    topCount = this.Top == null || previousBlocks.ContainsKey(this.Top)
                                   ? 0
                                   : this.Top.CountUsedBlocks(MovementDirection.Down, previousBlocks);
                    countUsed += Math.Max(leftCount, Math.Max(bottomCount, topCount));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("directionInReverse");
            }

            return countUsed;
        }

        public override string GetBlockString()
        {
            string directionString;

            switch (this.forceDirection)
            {
                default:
                case MovementDirection.Up:
                    directionString = BlockStrings.ChangeDirectionUp;
                    break;

                case MovementDirection.Down:
                    directionString = BlockStrings.ChangeDirectionDown;
                    break;

                case MovementDirection.Left:
                    directionString = BlockStrings.ChangeDirectionLeft;
                    break;

                case MovementDirection.Right:
                    directionString = BlockStrings.ChangeDirectionRight;
                    break;
            }

            return directionString;
        }
        
        public override string GetBlockStringFull()
        {
            return this.GetBlockString();
        }

        public override bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            // This block might return loops, so just return true
            if (previousBlocks.ContainsKey(this))
            {
                return true;
            }

            previousBlocks[this] = this;

            if ((this.Top != null && this.Top.TestReverseMoves(blocksCanReverseMove, previousBlocks))
                || (this.Left != null && this.Left.TestReverseMoves(blocksCanReverseMove, previousBlocks))
                || (this.Right != null && this.Right.TestReverseMoves(blocksCanReverseMove, previousBlocks))
                || (this.Bottom != null && this.Bottom.TestReverseMoves(blocksCanReverseMove, previousBlocks)))
            {
                return true;
            }

            return false;
        }

#if UNITY_IOS
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ForceDirection", this.forceDirection);
        }
#endif

        #endregion
    }
}