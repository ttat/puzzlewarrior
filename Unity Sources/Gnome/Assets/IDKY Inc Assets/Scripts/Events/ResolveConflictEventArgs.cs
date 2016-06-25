// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;

    public class ResolveConflictEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public ResolveConflictEventArgs(string resolveString)
        {
            this.ResolveString = resolveString;
        }

        #endregion

        #region Public Properties

        public string ResolveString { get; set; }

        #endregion
    }
}