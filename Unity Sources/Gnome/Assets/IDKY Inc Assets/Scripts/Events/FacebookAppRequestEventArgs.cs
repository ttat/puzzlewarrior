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
    using System.Collections.Generic;

    public class FacebookAppRequestEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public FacebookAppRequestEventArgs(List<FacebookAppRequest> appRequests)
        {
            this.AppRequests = appRequests;
        }

        #endregion

        #region Public Properties

        public List<FacebookAppRequest> AppRequests { get; set; }

        #endregion
    }
}