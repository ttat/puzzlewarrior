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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class UserHighScore
    {
        #region Public Properties

        public uint HighScore { get; set; }

        public string Name { get; set; }

        public Texture2D ProfilePicture { get; set; }

        public string UserId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return string.Format("Name: {0}, ID: {1}, Score: {2}", this.Name, this.UserId, this.HighScore);
        }

        #endregion
    }
}