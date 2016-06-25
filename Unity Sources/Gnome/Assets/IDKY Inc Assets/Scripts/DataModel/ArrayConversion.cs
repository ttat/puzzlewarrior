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
    public static class ArrayConversion
    {
        #region Public Methods and Operators

        /// <summary>
        /// Converts jagged array to 2D multidimension array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="jagged">The jagged.</param>
        /// <returns></returns>
        public static TU[,] JaggedArrayToMultidimension<T, TU>(this T[][] jagged) where TU : class
        {
            int rows = jagged.Length;
            int columns = 0;

            for (int i = 0; i < rows; i++)
            {
                columns = Math.Max(columns, jagged[i].Length);
            }

            TU[,] multidimension = new TU[rows,columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < jagged[i].Length; j++)
                {
                    multidimension[i, j] = jagged[i][j] as TU;
                }
            }

            return multidimension;
        }

        /// <summary>
        /// Converts 2D multidimension array to jagged array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="multidimension">The multidimension.</param>
        /// <returns></returns>
        public static TU[][] MultidimensionToJaggedArray<T, TU>(this T[,] multidimension) where TU : class
        {
            int rows = multidimension.GetLength(0);
            int columns = multidimension.GetLength(1);

            TU[][] jagged = new TU[rows][];

            for (int i = 0; i < rows; i++)
            {
                jagged[i] = new TU[columns];

                for (int j = 0; j < columns; j++)
                {
                    jagged[i][j] = multidimension[i, j] as TU;
                }
            }

            return jagged;
        }

        #endregion
    }
}