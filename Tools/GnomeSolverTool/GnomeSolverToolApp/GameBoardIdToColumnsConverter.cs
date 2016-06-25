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

    public class GameBoardIdToColumnsConverter : IValueConverter
    {
        #region Constants

        private const string UnityResourcesFolder = @"../../../../Unity Sources/Gnome/Assets/Resources/Gameboards/";

        #endregion

        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string id = (string)value;
            GameBoard gameBoard = GameBoardXmlIo.ReadGameBoard(id, UnityResourcesFolder);

            int columns = gameBoard != null ? gameBoard.GetTrimmedBoard().GetLength(1) : 0;

            return columns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}