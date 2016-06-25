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
    public class GameBlockPlayer : GameBlockBase, IGameBlockParent
    {
        #region Fields

        private int availableMoves;

        private int preferredMaxMoves;

        #endregion

        #region Constructors and Destructors
        
        /// <summary>
        /// For XML Serialization.
        /// </summary>
        private GameBlockPlayer()
            : base()
        {
        }

        public GameBlockPlayer(GameBoard gameBoard, int indexRow, int indexColumn, int preferredMaxMoves)
            : base(gameBoard, indexRow, indexColumn)
        {
            this.preferredMaxMoves = preferredMaxMoves;
        }

        public GameBlockPlayer(GameBoard gameBoard, int indexRow, int indexColumn)
            : base(gameBoard, indexRow, indexColumn)
        {
        }

        public GameBlockPlayer(int indexRow, int indexColumn)
            : base(indexRow, indexColumn)
        {
        }

        public GameBlockPlayer(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            this.availableMoves = info.GetInt32("AvailableMoves");
        }

        #endregion

        #region Public Properties

        [XmlAttribute("AvailableMoves")]
        public int AvailableMoves
        {
            get
            {
                return this.availableMoves;
            }

            set
            {
                this.availableMoves = value;
            }
        }

        [XmlIgnore]
        public override BlockAnimationType IdleAnimation
        {
            get
            {
                return BlockAnimationType.RedGnomeIdle;
            }
        }

        [XmlIgnore]
        public int PreferredMaxMoves
        {
            get
            {
                return this.preferredMaxMoves;
            }

            set
            {
                this.preferredMaxMoves = value;
            }
        }

        [XmlIgnore]
        public override string Text
        {
            get
            {
                return string.Format("x{0}", this.availableMoves);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override bool ApplyMove(IGameBlockParent parentBlock, MovementDirection direction, BlockMovement move)
        {
            // These blocks do the moves, so it has to be the first one selected.
            if (parentBlock != null)
            {
                return false;
            }

            // Out of moves
            if (this.availableMoves == 0)
            {
                return false;
            }

            BlockAnimationType animation;

            switch (direction)
            {
                default:
                case MovementDirection.Up:
                    animation = BlockAnimationType.RedGnomeSmashUp;
                    break;

                case MovementDirection.Down:
                    animation = BlockAnimationType.RedGnomeSmashDown;
                    break;

                case MovementDirection.Left:
                    animation = BlockAnimationType.RedGnomeSmashLeft;
                    break;

                case MovementDirection.Right:
                    animation = BlockAnimationType.RedGnomeSmashRight;
                    break;
            }

            this.NotifySetAnimation(animation, true, false);

            bool moveApplied;

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
                        moveApplied = this.Top.ApplyMove(this, direction, move);
                    }
                    break;

                case MovementDirection.Down:
                    if (this.Bottom == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Bottom.ApplyMove(this, direction, move);
                    }
                    break;

                case MovementDirection.Left:
                    if (this.Left == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Left.ApplyMove(this, direction, move);
                    }
                    break;

                case MovementDirection.Right:
                    if (this.Right == null)
                    {
                        moveApplied = false;
                    }
                    else
                    {
                        moveApplied = this.Right.ApplyMove(this, direction, move);
                    }
                    break;
            }

            this.NotifySetAnimation(
                moveApplied ? BlockAnimationType.RedGnomeSmashSuccessful : BlockAnimationType.RedGnomeSmashFailed, false, false);

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
            // Already used
            if (this.AvailableMoves > 0)
            {
                move = null;
                return false;
            }

            // This block is a valid "starting" point
            move = new BlockMovement(this, directionInReverse);
            return true;
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
            // Failed to find a looped reverse move
            move = null;

            return false;
        }

        public override void Copy(IGameBlock source)
        {
            GameBlockPlayer gameBlockPlayer = source as GameBlockPlayer;

            if (gameBlockPlayer != null)
            {
                this.availableMoves = gameBlockPlayer.availableMoves;
            }
        }

        public override object Clone()
        {
            GameBlockPlayer clone = new GameBlockPlayer(this.IndexRow, this.IndexColumn);
            clone.availableMoves = this.availableMoves;
            clone.preferredMaxMoves = this.preferredMaxMoves;

            return clone;
        }

        public override int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            return 0;
        }

        public override string GetBlockString()
        {
            return BlockStrings.NormalPlayer + this.AvailableMoves;
        }

        public override string GetBlockStringFull()
        {
            return BlockStrings.NormalPlayer + this.AvailableMoves;
        }

#if UNITY_IOS
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("AvailableMoves", this.availableMoves);
        }
#endif

        public override bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            return this.availableMoves == 0;
        }

        public BlockAnimationType GetPreparingMoveAnimation(MovementDirection direction)
        {
            BlockAnimationType animation;

            switch (direction)
            {
                default:
                case MovementDirection.Up:
                    animation = BlockAnimationType.RedGnomePrepareSmashUp;
                    break;

                case MovementDirection.Down:
                    animation = BlockAnimationType.RedGnomePrepareSmashDown;
                    break;

                case MovementDirection.Left:
                    animation = BlockAnimationType.RedGnomePrepareSmashLeft;
                    break;

                case MovementDirection.Right:
                    animation = BlockAnimationType.RedGnomePrepareSmashRight;
                    break;
            }

            return animation;
        }

        public void Undo(int originalAvailableMoves)
        {
            this.availableMoves = originalAvailableMoves;

            this.NotifySetAnimation(this.IdleAnimation, true, true);
        }

        #endregion
    }
}