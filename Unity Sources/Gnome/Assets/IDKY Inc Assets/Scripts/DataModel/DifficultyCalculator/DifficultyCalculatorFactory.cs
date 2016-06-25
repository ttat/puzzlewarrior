// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;
    using System.Collections.Generic;

    public static class DifficultyCalculatorFactory
    {
        #region Public Methods and Operators

        public static IDifficultyCalculator GetCalculator(HashSet<Type> blockTypes)
        {
            bool containsChangeDirection = blockTypes.Contains(typeof(GameBlockChangeDirection));
            bool containsExtraMove = blockTypes.Contains(typeof(GameBlockExtraMove));
            bool containsMultipleMoves = blockTypes.Contains(typeof(GameBlockMultipleMoves));

            IDifficultyCalculator calculator;

            if (!containsChangeDirection && !containsExtraMove && !containsMultipleMoves)
            {
                calculator = new NormalOnlyDifficultyCalculator();
            }
            else if (containsChangeDirection && !containsExtraMove && !containsMultipleMoves)
            {
                calculator = new ChangeDirectionDifficultyCalculator();
            }
            else
            {
                calculator = new DefaultDifficultyCalculator();
            }

            return calculator;
        }

        #endregion
    }
}