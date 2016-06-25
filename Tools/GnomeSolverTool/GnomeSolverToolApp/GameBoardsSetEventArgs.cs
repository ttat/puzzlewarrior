// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace GnomeSolverToolApp
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GameBoardsSetEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public GameBoardsSetEventArgs(bool aborted)
        {
            this.Aborted = aborted;
        }

        #endregion

        #region Public Properties

        public bool Aborted { get; set; }

        #endregion
    }
}