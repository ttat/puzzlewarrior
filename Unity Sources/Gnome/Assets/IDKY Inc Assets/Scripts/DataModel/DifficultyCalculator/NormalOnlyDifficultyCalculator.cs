// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System.Linq;

    public class NormalOnlyDifficultyCalculator : IDifficultyCalculator
    {
        #region Public Methods and Operators

        public double CalculateDifficulty(GameBoard gameBoard)
        {
            // Based on some data analysis.  Difficulty = 3 * NormalCount
            const double Weight = 3;

            int numNormalBlocks = gameBoard.GameBlocks.OfType<GameBlockNormal>().Count();

            double difficulty = Weight * numNormalBlocks;

            return difficulty;
        }

        #endregion
    }
}