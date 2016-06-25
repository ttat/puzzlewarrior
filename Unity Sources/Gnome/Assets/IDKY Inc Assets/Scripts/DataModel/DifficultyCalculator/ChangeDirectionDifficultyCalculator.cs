// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;
    using System.Linq;

    public class ChangeDirectionDifficultyCalculator : IDifficultyCalculator
    {
        #region Public Methods and Operators

        public double CalculateDifficulty(GameBoard gameBoard)
        {
            // Based on some data analysis.  Difficulty = 16 * LogFailuresOverSuccesses + 4 ChangeDirectionCount
            const double FailsSuccessesWeight = 16;
            const double ChangeDirectionWeight = 4;

            int numChangeDirection = gameBoard.GameBlocks.OfType<GameBlockChangeDirection>().Count();

            // Make sure we don't divide by 0 or try to get Log2(0)
            double logFailures = gameBoard.Failures == 0 ? 0 : Math.Log(gameBoard.Failures, 2);
            double failuresAndSuccesses = gameBoard.Successes == 0 ? 0 : logFailures / gameBoard.Successes;

            double difficulty = (FailsSuccessesWeight * failuresAndSuccesses) + (ChangeDirectionWeight * numChangeDirection);

            return difficulty;
        }

        #endregion
    }
}