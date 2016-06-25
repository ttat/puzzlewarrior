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
    public static class SpecialStageIo
    {
        #region Fields

        private const string SpecialStagePrefix = "specialStage";

        #endregion

        #region Public Methods and Operators

        public static SpecialStage ReadSpecialStage(
#if UNITY_EDITOR
            string stageId,
            string folderPath = @"Assets/Resources/Gameboards/"
#else
            string stageId,
            string folderPath = @"Gameboards/"
#endif
)
        {
            SpecialStage specialStage;

            string filename = string.Format("{0}_{1}.txt", SpecialStagePrefix, stageId);

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
                                specialStage = Serializer.Deserialize<SpecialStage>(serializedData);
                            }
                            else
                            {
                                specialStage = new SpecialStage();
                            }
                        }
                        else
                        {
                            specialStage = new SpecialStage();
                        }

                        reader.Close();
                    }
                }
                else
                {
                    specialStage = new SpecialStage();
                }
#else
                TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
                string serializedString = textAsset.text;

                if (!String.IsNullOrEmpty(serializedString))
                {
                    byte[] serializedData = Convert.FromBase64String(serializedString);

                    if (serializedData.Length > 0)
                    {
                        specialStage = Serializer.Deserialize<SpecialStage>(serializedData);
                    }
                    else
                    {
                        specialStage = new SpecialStage();
                    }
                }
                else
                {
                    specialStage = new SpecialStage();
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Read SpecialStage failed: " + ex.Message);
                specialStage = new SpecialStage();
            }

            return specialStage;
        }

        public static string WriteSpecialStage(
            SpecialStage specialStage, string folderPath = @"Assets/Resources/Gameboards/")
        {
            byte[] bytes = Serializer.Serialize(specialStage);

            string filename = string.Format("{0}_{1}.txt", SpecialStagePrefix, specialStage.StageId);

            string path = Path.Combine(folderPath, filename);

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
                string readableFileName = string.Format("{0}_Readable_{1}.xml", SpecialStagePrefix, specialStage.StageId);
                string readablePath = Path.Combine(folderPath, readableFileName);
                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                xmlSettings.IndentChars = "\t";
                xmlSettings.NewLineChars = "\n";

                using (XmlWriter xmlWriter =  XmlWriter.Create(readablePath, xmlSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(specialStage.GetType());
                    serializer.Serialize(xmlWriter, specialStage);
                }

                // Also write the data into an csv file so we can import it into a spreadsheet if needed
                string csvFileName = string.Format("{0}_Readable_{1}.csv", SpecialStagePrefix, specialStage.StageId);
                string csvPath = Path.Combine(folderPath, csvFileName);

                using (TextWriter textWriter = new StreamWriter(csvPath, false, Encoding.ASCII))
                {
                    textWriter.WriteLine(string.Format("Special Stage: {0} - {1}", specialStage.StageId, specialStage.Description));

                    foreach (KeyValuePair<int, string> keyValuePair in specialStage.LevelMapping.Mapping)
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