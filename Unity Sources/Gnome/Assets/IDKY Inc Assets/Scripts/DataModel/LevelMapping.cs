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
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class LevelMapping
#if UNITY_IOS
        : ISerializable
#endif
    {
        #region Fields

        private List<int> keys;

        /// <summary>
        /// Unity won't serialize Dictionaries
        /// </summary>
        [field: NonSerialized]
        private Dictionary<int, string> mapping;

        private List<string> values;

        #endregion

        #region Constructors and Destructors

        public LevelMapping()
        {
            this.mapping = new Dictionary<int, string>();
        }

        public LevelMapping(SerializationInfo info, StreamingContext ctxt)
        {
            this.Keys = (List<int>)info.GetValue("Keys", typeof(List<int>));
            this.Values = (List<string>)info.GetValue("Values", typeof(List<string>));
        }

        #endregion

        #region Public Properties

        [XmlArray("Keys")]
        [XmlArrayItem("Key")]
        public List<int> Keys
        {
            get
            {
                return this.keys;
            }
            set
            {
                this.keys = value;
            }
        }

        [XmlIgnore]
        public Dictionary<int, string> Mapping
        {
            get
            {
                return this.mapping;
            }
            set
            {
                this.mapping = value;
            }
        }

        [XmlArray("Values")]
        [XmlArrayItem("Value")]
        public List<string> Values
        {
            get
            {
                return this.values;
            }
            set
            {
                this.values = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Deserialized()
        {
            this.OnDeserialized(new StreamingContext());
        }

        public void GenerateLevelKeys(int numberOfLevels)
        {
            for (int i = 1; i <= numberOfLevels; i++)
            {
                if (!this.mapping.ContainsKey(i))
                {
                    this.mapping.Add(i, string.Empty);
                }
            }
        }

#if UNITY_IOS
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Keys", this.keys);
            info.AddValue("Values", this.values);
        }
#endif

        public void Serializating()
        {
            this.OnSerializing(new StreamingContext());
        }

        #endregion

        #region Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.mapping = new Dictionary<int, string>();

            for (int i = 0; i < this.Keys.Count; i++)
            {
                this.mapping.Add(this.Keys[i], this.Values[i]);
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            this.Keys = new List<int>();
            this.Values = new List<string>();

            foreach (KeyValuePair<int, string> keyValuePair in this.mapping)
            {
                this.Keys.Add(keyValuePair.Key);
                this.Values.Add(keyValuePair.Value);
            }
        }

        #endregion
    }
}