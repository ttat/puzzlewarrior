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

    using UnityEngine;

    public class FacebookProfilePictureEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public FacebookProfilePictureEventArgs(Texture2D profilePicture)
        {
            this.ProfilePicture = profilePicture;
        }

        #endregion

        #region Public Properties

        public Texture2D ProfilePicture { get; private set; }

        #endregion
    }
}