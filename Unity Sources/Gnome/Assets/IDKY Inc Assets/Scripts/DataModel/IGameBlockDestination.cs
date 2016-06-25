// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    public interface IGameBlockDestination : IGameBlock
    {
        #region Public Properties

        bool IsAvailable { get; }

        bool IsFullyAvailable { get; }

        int NumberAvailable { get; }

        int NumberUsed { get; }

        #endregion

        #region Public Methods and Operators

        bool CanBeRemovedIntermediate();

        bool IsOrphaned();

        void SetAvailability(bool available);

        void Undo();

        #endregion
    }
}