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

    public interface IGameBlock : ICloneable
    {
        #region Public Events

        event EventHandler<GameBlockSetAnimationEventArgs> GameBlockSetAnimationEvent;

        #endregion

        #region Public Properties

        IGameBlock Bottom { get; }

        GameBoard GameBoard { get; set; }

        BlockAnimationType IdleAnimation { get; }

        int IndexColumn { get; }

        int IndexRow { get; }

        IGameBlock Left { get; }

        IGameBlock Right { get; }

        string Text { get; }

        IGameBlock Top { get; }

        #endregion

        #region Public Methods and Operators

        bool ApplyMove(IGameBlockParent parentBlock, MovementDirection direction, BlockMovement move);

        /// <summary>
        /// Applies the reverse move.
        /// </summary>
        /// <param name="destinationBlock">The destination block.</param>
        /// <param name="directionInReverse">The direction in reverse.  For example if this is "Up," when the
        /// solution is actually used, the user will want to up from the block that is directly below this block.</param>
        /// <param name="numberOfBlocksToMove">The number of blocks to move.</param>
        /// <param name="move">The move.</param>
        /// <param name="previousBlock">The previous block.</param>
        /// <param name="previousDestination">The previous destination block.</param>
        /// <param name="blockHistory">The block history.</param>
        /// <returns></returns>
        bool ApplyReverseMove(
            IGameBlockDestination destinationBlock,
            MovementDirection directionInReverse,
            int numberOfBlocksToMove,
            out BlockMovement move,
            IGameBlock previousBlock,
            IGameBlockDestination previousDestination,
            Queue<IGameBlock> blockHistory);

        /// <summary>
        /// Applies the reverse move but with a looped move as the primary goal.
        /// </summary>
        /// <param name="destinationBlock">The destination block.</param>
        /// <param name="directionInReverse">The direction in reverse.  For example if this is "Up," when the
        /// solution is actually used, the user will want to up from the block that is directly below this block.</param>
        /// <param name="numberOfBlocksToMove">The number of blocks to move.</param>
        /// <param name="move">The move.</param>
        /// <param name="previousBlock">The previous block.</param>
        /// <param name="previousDestination">The previous destination block.</param>
        /// <param name="blockHistory">The block history.</param>
        /// <returns></returns>
        bool ApplyReverseMoveForLoop(
            IGameBlockDestination destinationBlock,
            MovementDirection directionInReverse,
            int numberOfBlocksToMove,
            out BlockMovement move,
            IGameBlock previousBlock,
            IGameBlockDestination previousDestination,
            Queue<IGameBlock> blockHistory);

        /// <summary>
        /// Copies the values from the source.
        /// </summary>
        /// <param name="source">The source.</param>
        void Copy(IGameBlock source);

        /// <summary>
        /// Counts the number of blocks used (i.e. !IsAvailable). This is used for finding reverse solutions.
        /// </summary>
        /// <param name="directionInReverse">The direction in reverse.</param>
        /// <param name="previousBlocks"> </param>
        /// <returns></returns>
        int CountUsedBlocks(MovementDirection directionInReverse, Dictionary<IGameBlock, IGameBlock> previousBlocks);

        string GetBlockString();

        string GetBlockStringFull();

        /// <summary>
        /// Tests to see if a reverse move is possible for this block.
        /// </summary>
        /// <param name="blocksCanReverseMove">The blocks that can reverse move.</param>
        /// <param name="previousBlocks"> </param>
        /// <returns></returns>
        bool TestReverseMoves(
            Dictionary<IGameBlockDestination, IGameBlockDestination> blocksCanReverseMove, Dictionary<IGameBlock, IGameBlock> previousBlocks);

        #endregion
    }
}