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

    public class FacebookAppRequestDeletedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public FacebookAppRequestDeletedEventArgs(FacebookAppRequest deletedAppRequest)
        {
            this.DeletedAppRequest = deletedAppRequest;
        }

        #endregion

        #region Public Properties

        public FacebookAppRequest DeletedAppRequest { get; set; }

        #endregion
    }
}