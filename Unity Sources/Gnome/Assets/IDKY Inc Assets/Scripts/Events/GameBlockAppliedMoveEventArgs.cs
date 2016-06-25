// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GameBlockSetAnimationEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public GameBlockSetAnimationEventArgs(BlockAnimationType animationType, bool canOverride, bool disableSounds)
        {
            this.AnimationType = animationType;
            this.CanOverride = canOverride;
            this.DisableSounds = disableSounds;
        }

        #endregion

        #region Public Properties

        public BlockAnimationType AnimationType { get; set; }

        public bool CanOverride { get; set; }

        public bool DisableSounds { get; set; }

        #endregion
    }
}