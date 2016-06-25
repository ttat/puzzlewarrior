// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    public interface IIdkyKeyValuePair<TKey, TValue>
    {
        #region Public Methods and Operators

        TKey GetKey();

        TValue GetValue();

        void SetKey(TKey key);

        void SetValue(TValue value);

        #endregion
    }
}