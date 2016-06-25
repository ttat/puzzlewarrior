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
    public class GameBoardToRowsTimesColumnsConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GameBoard gameBoard = value as GameBoard;

            int rowsTimesColumns = gameBoard != null
                                       ? gameBoard.GetTrimmedBoard().GetLength(0) * gameBoard.GetTrimmedBoard().GetLength(1)
                                       : 0;

            return rowsTimesColumns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}