// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;

    using UnityEngine;

    [Serializable]
    public class IdkyKeyValuePairGameObject : IIdkyKeyValuePair<string, GameObject>
    {
        #region Fields

        public string Key;

        public GameObject Value;

        #endregion

        #region Public Methods and Operators

        public string GetKey()
        {
            return this.Key;
        }

        public GameObject GetValue()
        {
            return this.Value;
        }

        public void SetKey(string key)
        {
            this.Key = key;
        }

        public void SetValue(GameObject value)
        {
            this.Value = value;
        }

        #endregion
    }
}