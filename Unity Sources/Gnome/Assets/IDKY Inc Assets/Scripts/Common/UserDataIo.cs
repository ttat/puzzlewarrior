// ----------------------------------------------
// 
//             Tearible Monster Run
// 
//  Copyright © 2013 IDKY
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public static class UserDataIo
    {
        #region Public Methods

        public static T GetData<T>(string key) where T : class, new()
        {
            T returnData;

            string dataString = PlayerPrefsFast.GetString(key);

            // If there's data
            if (!string.IsNullOrEmpty(dataString))
            {
                // Deserialize
                using (TextReader textReader = new StringReader(dataString))
                {
                    using (XmlTextReader xmlTextReader = new XmlTextReader(textReader))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof (T));

                        returnData = xmlSerializer.Deserialize(xmlTextReader) as T;
                    }
                }
            }
            else
            {
                returnData = null;
            }

            return returnData;
        }

        public static void SaveData<T>(string key, T data)
        {
            string dataString = string.Empty;

            // Convert the data to an XML string
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof (T));

                    xmlSerializer.Serialize(xmlTextWriter, data);

                    memoryStream.Position = 0;

                    TextReader textReader = new StreamReader(memoryStream);
                    dataString = textReader.ReadToEnd();
                }
            }

            // Save the string
            PlayerPrefsFast.SetString(key, dataString);
        }

        public static void WritePlayerPrefs()
        {
            PlayerPrefsFast.Flush();
        }

        #endregion
    }
}