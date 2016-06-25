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

    public class DefaultDifficultyCalculator : IDifficultyCalculator
    {
        #region Public Methods and Operators

        public double CalculateDifficulty(GameBoard gameBoard)
        {
            // Based on some data analysis.  Difficulty = (12 * (Log2(Failures) / Successes)) + (2 * NumberOfPlayerBlocks)
            const double FailsSuccessesWeight = 12.0;
            const double NumPlayersWeight = 2.0;

            int numPlayerBlocks = gameBoard.GameBlocks.OfType<GameBlockPlayer>().Count();

            // Make sure we don't divide by 0 or try to get Log2(0)
            double logFailures = gameBoard.Failures == 0 ? 0 : Math.Log(gameBoard.Failures, 2);
            double failuresAndSuccesses = gameBoard.Successes == 0 ? 0 : logFailures / gameBoard.Successes;

            double difficulty = (FailsSuccessesWeight * failuresAndSuccesses) + (NumPlayersWeight * numPlayerBlocks);

            return difficulty;
        }

        #endregion
    }
}