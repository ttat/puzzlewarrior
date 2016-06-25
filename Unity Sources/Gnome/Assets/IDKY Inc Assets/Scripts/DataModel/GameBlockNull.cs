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
    public class GameBlockNull : GameBlockBase
    {
        #region Constructors and Destructors

        public GameBlockNull(GameBoard gameBoard, int indexRow, int indexColumn)
            : base(gameBoard, indexRow, indexColumn)
        {
        }

        public GameBlockNull(int indexRow, int indexColumn)
            : base(indexRow, indexColumn)
        {
        }

        public GameBlockNull(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
        }

        /// <summary>
        /// For XML Serialization.
        /// </summary>
        private GameBlockNull()
        {
        }

        #endregion

        #region Public Properties

        [XmlIgnore]
        public override BlockAnimationType IdleAnimation
        {
            get
            {
                return BlockAnimationType.None;
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
            return false;
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

            return false;
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

            return false;
        }

        public override object Clone()
        {
            GameBlockNull clone = new GameBlockNull(this.IndexRow, this.IndexColumn);

            return clone;
        }

        public override int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            return 0;
        }

        public override string GetBlockString()
        {
            return BlockStrings.NullBlock;
        }

        public override string GetBlockStringFull()
        {
            return BlockStrings.NullBlock;
        }

        public override bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks)
        {
            return false;
        }

        #endregion
    }
}