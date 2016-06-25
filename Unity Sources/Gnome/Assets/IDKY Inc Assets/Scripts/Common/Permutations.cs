// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Sources: https://stackoverflow.com/questions/127704/algorithm-to-return-all-combinations-of-k-elements-from-n/
    /// http://stackoverflow.com/questions/15150147/all-combinations-of-a-list
    /// </summary>
    public static class Permutations
    {
        #region Public Methods and Operators

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            if (k == 0)
            {
                return new List<IEnumerable<T>>();
            }

            if (k == 1)
            {
                List<IEnumerable<T>> combos = new List<IEnumerable<T>>();

                foreach (T element in elements)
                {
                    combos.Add(new List<T> { element });
                }

                return combos;
            }

            return elements.SelectMany((e, i) => elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                yield break;
            }

            List<T> list = sequence.ToList();

            if (!list.Any())
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                int startingElementIndex = 0;

                foreach (T startingElement in list)
                {
                    IEnumerable<T> remainingItems = list.AllExcept(startingElementIndex);

                    foreach (IEnumerable<T> permutationOfRemainder in remainingItems.Permute())
                    {
                        yield return startingElement.Concat(permutationOfRemainder);
                    }

                    startingElementIndex++;
                }
            }
        }

        #endregion

        #region Methods

        private static IEnumerable<T> AllExcept<T>(this IEnumerable<T> sequence, int indexToSkip)
        {
            if (sequence == null)
            {
                yield break;
            }

            int index = 0;

            foreach (T item in sequence.Where(item => index++ != indexToSkip))
            {
                yield return item;
            }
        }

        private static IEnumerable<T> Concat<T>(this T firstElement, IEnumerable<T> secondSequence)
        {
            yield return firstElement;
            if (secondSequence == null)
            {
                yield break;
            }

            foreach (T item in secondSequence)
            {
                yield return item;
            }
        }

        #endregion
    }
}