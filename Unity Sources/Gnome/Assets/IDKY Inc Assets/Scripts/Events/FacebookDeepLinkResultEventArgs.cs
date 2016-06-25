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

    public class FacebookDeepLinkResultEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public FacebookDeepLinkResultEventArgs(string requestId)
        {
            this.RequestId = requestId;
        }

        #endregion

        #region Public Properties

        public string RequestId { get; private set; }

        #endregion
    }
}