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

    public class BlockMovement : ICloneable
    {
        #region Fields

        private IGameBlockDestination destinationBlock;

        private MovementDirection direction;

        private BlockMovement head;

        private List<IGameBlockDestination> intermediateBlocks;

        private BlockMovement nextMove;

        private int originalAvailableMoves;

        private BlockMovement previousMove;

        private IGameBlockParent sourceBlock;

        #endregion

        #region Constructors and Destructors

        public BlockMovement(IGameBlockParent sourceBlock, MovementDirection direction)
        {
            this.sourceBlock = sourceBlock;
            this.destinationBlock = null;
            this.direction = direction;
            this.intermediateBlocks = new List<IGameBlockDestination>();

            this.originalAvailableMoves = sourceBlock == null ? 0 : sourceBlock.AvailableMoves;
        }

        #endregion

        #region Public Properties

        public IGameBlockDestination DestinationBlock
        {
            get
            {
                return this.destinationBlock;
            }

            set
            {
                this.destinationBlock = value;
            }
        }

        public MovementDirection Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                this.direction = value;
            }
        }

        public BlockMovement Head
        {
            get
            {
                return this.head;
            }
            set
            {
                this.head = value;
            }
        }

        public List<IGameBlockDestination> IntermediateBlocks
        {
            get
            {
                return this.intermediateBlocks;
            }
            set
            {
                this.intermediateBlocks = value;
            }
        }

        public BlockMovement NextMove
        {
            get
            {
                return this.nextMove;
            }

            set
            {
                this.nextMove = value;
            }
        }

        public BlockMovement PreviousMove
        {
            get
            {
                return this.previousMove;
            }

            set
            {
                this.previousMove = value;
            }
        }

        public IGameBlockParent SourceBlock
        {
            get
            {
                return this.sourceBlock;
            }

            set
            {
                this.sourceBlock = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public object Clone()
        {
            BlockMovement clone = new BlockMovement(this.sourceBlock, this.direction);
            clone.intermediateBlocks.AddRange(this.intermediateBlocks);
            clone.destinationBlock = this.destinationBlock;
            clone.previousMove = this.previousMove;
            clone.head = this.head;
            clone.originalAvailableMoves = this.originalAvailableMoves;

            if (this.nextMove != null)
            {
                clone.nextMove = (BlockMovement)this.nextMove.Clone();
            }

            return clone;
        }

        public BlockMovement CloneFromGameBoard(GameBoard gameBoard, BlockMovement previous)
        {
            BlockMovement clone =
                new BlockMovement(
                    gameBoard.GameBlocks[this.sourceBlock.IndexRow, this.sourceBlock.IndexColumn] as IGameBlockParent, this.direction);

            if (this.destinationBlock != null)
            {
                clone.destinationBlock =
                    gameBoard.GameBlocks[this.destinationBlock.IndexRow, this.destinationBlock.IndexColumn] as IGameBlockDestination;
            }

            foreach (IGameBlockDestination intermediateBlock in this.intermediateBlocks)
            {
                if (intermediateBlock != null)
                {
                    clone.intermediateBlocks.Add(
                        gameBoard.GameBlocks[intermediateBlock.IndexRow, intermediateBlock.IndexColumn] as IGameBlockDestination);
                }
            }

            clone.previousMove = previous;
            clone.head = previous == null ? this : previous.head;
            clone.originalAvailableMoves = this.originalAvailableMoves;

            if (this.nextMove != null)
            {
                clone.nextMove = this.nextMove.CloneFromGameBoard(gameBoard, clone);
            }

            return clone;
        }

        public void Enqueue(BlockMovement blockMovement)
        {
            BlockMovement lastMovement = this.GetLastMovement();
            blockMovement.PreviousMove = lastMovement;
            lastMovement.NextMove = blockMovement;
        }

        public BlockMovement GetLastMovement()
        {
            BlockMovement last = this;

            while (last.NextMove != null)
            {
                last = last.nextMove;
            }

            return last;
        }

        public void InsertDestinationBlock(IGameBlockDestination destination)
        {
            // Move the destinationBlock into the intermediateBlocks
            if (this.destinationBlock != null)
            {
                this.intermediateBlocks.Add(this.destinationBlock);
            }

            this.destinationBlock = destination;
        }

        public void UndoMove()
        {
            this.destinationBlock.Undo();

            foreach (IGameBlockDestination gameBlockDestination in IntermediateBlocks)
            {
                gameBlockDestination.Undo();
            }

            this.sourceBlock.Undo(this.originalAvailableMoves);
        }

        #endregion
    }
}