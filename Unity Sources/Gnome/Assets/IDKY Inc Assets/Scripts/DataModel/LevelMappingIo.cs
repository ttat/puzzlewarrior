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
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using UnityEngine;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class LevelMappingIo
    {
        #region Public Methods and Operators

        public static LevelMapping ReadLevelMapping(
#if UNITY_EDITOR
            string folderPath = @"Assets/Resources/Gameboards/",
            string filename = "levelMapping.txt"
#else
            string folderPath = @"Gameboards/", 
            string filename = "levelMapping"
#endif
)
        {
            LevelMapping levelMapping;
            string path = Path.Combine(folderPath, filename);

            try
            {
#if UNITY_EDITOR
                if (File.Exists(path))
                {
                    using (TextReader reader = new StreamReader(path))
                    {
                        string serializedString = reader.ReadToEnd();

                        if (!String.IsNullOrEmpty(serializedString))
                        {
                            byte[] serializedData = Convert.FromBase64String(serializedString);

                            if (serializedData.Length > 0)
                            {
                                levelMapping = Serializer.Deserialize<LevelMapping>(serializedData);
                            }
                            else
                            {
                                levelMapping = new LevelMapping();
                            }
                        }
                        else
                        {
                            levelMapping = new LevelMapping();
                        }

                        reader.Close();
                    }
                }
                else
                {
                    levelMapping = new LevelMapping();
                }
#else
                TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
                string serializedString = textAsset.text;

                if (!String.IsNullOrEmpty(serializedString))
                {
                    byte[] serializedData = Convert.FromBase64String(serializedString);

                    if (serializedData.Length > 0)
                    {
                        levelMapping = Serializer.Deserialize<LevelMapping>(serializedData);
                    }
                    else
                    {
                        levelMapping = new LevelMapping();
                    }
                }
                else
                {
                    levelMapping = new LevelMapping();
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Read LevelMapping failed: " + ex.Message);
                levelMapping = new LevelMapping();
            }

            return levelMapping;
        }

        public static string WriteLevelMapping(
            LevelMapping levelMapping, string folderPath = @"Assets/Resources/Gameboards/", string fileName = "levelMapping.txt")
        {
            byte[] bytes = Serializer.Serialize(levelMapping);

            string path = Path.Combine(folderPath, fileName);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (TextWriter writer = File.CreateText(path))
                {
                    string serializedString = Convert.ToBase64String(bytes);

                    writer.Write(serializedString);
                    writer.Close();
                }
                
#if UNITY_EDITOR
                // Also write the data into an XML file so it's easier to read and compare changes between versions
                string readableFileName = "levelMappingReadable.xml";
                string readablePath = Path.Combine(folderPath, readableFileName);
                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                xmlSettings.IndentChars = "\t";
                xmlSettings.NewLineChars = "\n";

                using (XmlWriter xmlWriter =  XmlWriter.Create(readablePath, xmlSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(levelMapping.GetType());
                    serializer.Serialize(xmlWriter, levelMapping);
                }

                // Also write the data into an csv file so we can import it into a spreadsheet if needed
                string csvFileName = "levelMappingReadable.csv";
                string csvPath = Path.Combine(folderPath, csvFileName);

                using (TextWriter textWriter = new StreamWriter(csvPath, false, Encoding.ASCII))
                {
                    foreach (KeyValuePair<int, string> keyValuePair in levelMapping.Mapping)
                    {
                        textWriter.WriteLine(string.Format("{0}, {1}", keyValuePair.Key, keyValuePair.Value ?? string.Empty));
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Saving level mapping failed: " + ex.Message);
            }

            return path;
        }

        #endregion
    }
}