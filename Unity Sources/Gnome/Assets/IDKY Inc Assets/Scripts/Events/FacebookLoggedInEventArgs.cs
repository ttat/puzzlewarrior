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

    public class FacebookLoggedInEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public FacebookLoggedInEventArgs(FBResult result)
        {
            this.Result = result;
        }

        #endregion

        #region Public Properties

        public FBResult Result { get; private set; }

        #endregion
    }
}