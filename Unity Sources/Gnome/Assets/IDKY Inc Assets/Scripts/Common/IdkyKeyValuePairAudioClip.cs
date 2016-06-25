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
    public class IdkyKeyValuePairAudioClip : IIdkyKeyValuePair<string, AudioClip>
    {
        #region Fields

        public string Key;

        public AudioClip Value;

        #endregion

        #region Public Methods and Operators

        public string GetKey()
        {
            return this.Key;
        }

        public AudioClip GetValue()
        {
            return this.Value;
        }

        public void SetKey(string key)
        {
            this.Key = key;
        }

        public void SetValue(AudioClip value)
        {
            this.Value = value;
        }

        #endregion
    }
}