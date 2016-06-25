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

    public class LoadingEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public LoadingEventArgs(float currentProgressPercentage, bool completed)
        {
            this.CurrentProgressPercentage = currentProgressPercentage;
            this.Completed = completed;
        }

        #endregion

        #region Public Properties

        public bool Completed { get; set; }

        public float CurrentProgressPercentage { get; set; }

        #endregion
    }
}