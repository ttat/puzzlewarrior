// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    public struct Margin
    {
        #region Constructors

        public Margin(float left, float top, float right, float bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        #endregion

        #region Properties

        public float Left;

        public float Top;

        public float Right;

        public float Bottom;

        #endregion
    }
}