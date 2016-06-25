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

    public class FacebookProfileEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public FacebookProfileEventArgs(FacebookProfile profile)
        {
            this.Profile = profile;
        }

        #endregion

        #region Public Properties

        public FacebookProfile Profile { get; private set; }

        #endregion
    }
}