// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace GnomeSolverToolApp
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Idky;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GameBoardToDestinationBoardStringConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GameBoard gameBoard = value as GameBoard;

            string types = gameBoard != null ? gameBoard.GetBoardDestinationStringShort() : string.Empty;

            return types;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}