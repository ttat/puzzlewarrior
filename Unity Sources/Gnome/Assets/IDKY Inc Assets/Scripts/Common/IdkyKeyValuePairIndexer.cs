// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    public static class IdkyKeyValuePairIndexer
    {
        #region Public Methods and Operators

        public static IIdkyKeyValuePair<TKey, TValue> GetPair<TKey, TValue>(this IIdkyKeyValuePair<TKey, TValue>[] keyValuePairs, TKey key)
        {
            foreach (IIdkyKeyValuePair<TKey, TValue> keyValuePair in keyValuePairs)
            {
                if (Equals(keyValuePair.GetKey(), key))
                {
                    return keyValuePair;
                }
            }

            return null;
        }

        #endregion
    }
}