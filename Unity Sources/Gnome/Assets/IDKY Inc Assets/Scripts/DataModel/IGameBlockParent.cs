// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    public interface IGameBlockParent : IGameBlock
    {
        #region Public Properties

        int AvailableMoves { get; set; }

        int PreferredMaxMoves { get; set; }

        #endregion

        #region Public Methods and Operators

        BlockAnimationType GetPreparingMoveAnimation(MovementDirection direction);

        void Undo(int originalAvailableMoves);

        #endregion
    }
}