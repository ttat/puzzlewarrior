// ----------------------------------------------
// 
//             2 Upper
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    using UnityEngine;

    public static class PlayerPrefsFast
    {
        #region Constants

        private const string ENCRYPTED_TAG = "[ENCRYPTED]";

        private const string KEY_VALUE_SEPERATOR = "::";

        private const string PARAMETERS_SEPERATOR = ";;";

        #endregion

        #region Static Fields

        private static readonly string fileName = Application.persistentDataPath + "/PlayerPrefs.txt";

        private static bool hashTableChanged;

        private static Hashtable playerPrefsHashtable = new Hashtable();

        private static string serializedInput = "";

        private static string serializedOutput = "";

        #endregion

        #region Constructors and Destructors

        static PlayerPrefsFast()
        {
            //load previous settings
            StreamReader fileReader = null;

            if (File.Exists(fileName))
            {
                fileReader = new StreamReader(fileName);
                string readLine = fileReader.ReadLine();

                if (readLine.StartsWith(ENCRYPTED_TAG))
                {
                    string remove = readLine.Remove(0, ENCRYPTED_TAG.Length);
                    serializedInput = DecryptStringAes(remove, KayoCow.TeeTee);
                }
                else
                {
                    serializedInput = readLine;
                }

                Deserialize();

                fileReader.Close();
            }

            Debug.Log("Player Prefs path = " + fileName);
        }

        #endregion

        #region Public Methods and Operators

        public static void DeleteAll()
        {
            playerPrefsHashtable.Clear();
        }

        public static void DeleteKey(string key)
        {
            playerPrefsHashtable.Remove(key);
        }

#if UNITY_EDITOR
        public static void Flush(bool encrypted = false)
#else
        public static void Flush(bool encrypted = true)
#endif
        {
            if (hashTableChanged)
            {
                Serialize();

                StreamWriter fileWriter = null;
                fileWriter = File.CreateText(fileName);

                if (fileWriter == null)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
                    }
                }

                string output;

                if (encrypted)
                {
                    output = ENCRYPTED_TAG + EncryptStringAes(serializedOutput, KayoCow.TeeTee);
                }
                else
                {
                    output = serializedOutput;
                }

                fileWriter.WriteLine(output);

                fileWriter.Close();

                serializedOutput = "";
            }
        }

        public static bool GetBool(string key)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                bool returnVal;

                try
                {
                    returnVal = (bool)playerPrefsHashtable[key];
                }
                catch
                {
                    returnVal = false;
                }

                return returnVal;
            }

            return false;
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                bool returnVal;

                try
                {
                    returnVal = (bool)playerPrefsHashtable[key];
                }
                catch
                {
                    returnVal = defaultValue;
                }

                return returnVal;
            }
            else
            {
                playerPrefsHashtable.Add(key, defaultValue);
                hashTableChanged = true;
                return defaultValue;
            }
        }

        public static float GetFloat(string key)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                return (float)playerPrefsHashtable[key];
            }

            return 0.0f;
        }

        public static float GetFloat(string key, float defaultValue)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                return (float)playerPrefsHashtable[key];
            }
            else
            {
                playerPrefsHashtable.Add(key, defaultValue);
                hashTableChanged = true;
                return defaultValue;
            }
        }

        public static int GetInt(string key)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                return (int)playerPrefsHashtable[key];
            }

            return 0;
        }

        public static int GetInt(string key, int defaultValue)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                return (int)playerPrefsHashtable[key];
            }
            else
            {
                playerPrefsHashtable.Add(key, defaultValue);
                hashTableChanged = true;
                return defaultValue;
            }
        }

        public static string GetString(string key)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                return playerPrefsHashtable[key].ToString();
            }

            return null;
        }

        public static string GetString(string key, string defaultValue)
        {
            if (playerPrefsHashtable.ContainsKey(key))
            {
                return playerPrefsHashtable[key].ToString();
            }
            else
            {
                playerPrefsHashtable.Add(key, defaultValue);
                hashTableChanged = true;
                return defaultValue;
            }
        }

        public static object GetTypeValue(string typeName, string value)
        {
            object returnData = null;

            switch (typeName)
            {
                case "System.String":
                    returnData = value;
                    break;

                case "System.Int32":
                    returnData = Convert.ToInt32(value);
                    break;

                case "System.Boolean":
                    returnData = Convert.ToBoolean(value);
                    break;

                case "System.Single":
                    returnData = Convert.ToSingle(value);
                    break;

                default:
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogError("Unsupported type: " + typeName);
                    }
                    break;
            }

            return returnData;
        }

        public static bool HasKey(string key)
        {
            return playerPrefsHashtable.ContainsKey(key);
        }

        public static void SetBool(string key, bool value)
        {
            if (!playerPrefsHashtable.ContainsKey(key))
            {
                playerPrefsHashtable.Add(key, value);
            }
            else
            {
                playerPrefsHashtable[key] = value;
            }

            hashTableChanged = true;
        }

        public static void SetFloat(string key, float value)
        {
            if (!playerPrefsHashtable.ContainsKey(key))
            {
                playerPrefsHashtable.Add(key, value);
            }
            else
            {
                playerPrefsHashtable[key] = value;
            }

            hashTableChanged = true;
        }

        public static void SetInt(string key, int value)
        {
            if (!playerPrefsHashtable.ContainsKey(key))
            {
                playerPrefsHashtable.Add(key, value);
            }
            else
            {
                playerPrefsHashtable[key] = value;
            }

            hashTableChanged = true;
        }

        public static void SetString(string key, string value)
        {
            if (!playerPrefsHashtable.ContainsKey(key))
            {
                playerPrefsHashtable.Add(key, value);
            }
            else
            {
                playerPrefsHashtable[key] = value;
            }

            hashTableChanged = true;
        }

        #endregion

        #region Methods

        internal static string DecryptStringAes(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }
            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, Encoding.ASCII.GetBytes(Muoi.Mang));

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            return plaintext;
        }

        internal static string EncryptStringAes(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");
            }
            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            string outStr = null; // Encrypted string to return
            RijndaelManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, Encoding.ASCII.GetBytes(Muoi.Mang));

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        private static string DeEscapeNonSeperators(string inputToDeEscape)
        {
            inputToDeEscape = inputToDeEscape.Replace("\\" + KEY_VALUE_SEPERATOR, KEY_VALUE_SEPERATOR);
            inputToDeEscape = inputToDeEscape.Replace("\\" + PARAMETERS_SEPERATOR, PARAMETERS_SEPERATOR);
            return inputToDeEscape;
        }

        private static void Deserialize()
        {
            string[] parameters = serializedInput.Split(new[] { " " + PARAMETERS_SEPERATOR + " " }, StringSplitOptions.None);

            foreach (string parameter in parameters)
            {
                string[] parameterContent = parameter.Split(new[] { " " + KEY_VALUE_SEPERATOR + " " }, StringSplitOptions.None);

                playerPrefsHashtable.Add(
                    DeEscapeNonSeperators(parameterContent[0]),
                    GetTypeValue(parameterContent[2], DeEscapeNonSeperators(parameterContent[1])));

                if (parameterContent.Length > 3)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements");
                    }
                }
            }
        }

        private static string EscapeNonSeperators(string inputToEscape)
        {
            inputToEscape = inputToEscape.Replace(KEY_VALUE_SEPERATOR, "\\" + KEY_VALUE_SEPERATOR);
            inputToEscape = inputToEscape.Replace(PARAMETERS_SEPERATOR, "\\" + PARAMETERS_SEPERATOR);
            return inputToEscape;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        private static void Serialize()
        {
            IDictionaryEnumerator myEnumerator = playerPrefsHashtable.GetEnumerator();

            while (myEnumerator.MoveNext())
            {
                if (serializedOutput != "")
                {
                    serializedOutput += " " + PARAMETERS_SEPERATOR + " ";
                }
                serializedOutput += EscapeNonSeperators(myEnumerator.Key.ToString()) + " " + KEY_VALUE_SEPERATOR + " "
                                    + EscapeNonSeperators(myEnumerator.Value.ToString()) + " " + KEY_VALUE_SEPERATOR + " "
                                    + myEnumerator.Value.GetType();
            }
        }

        #endregion
    }
}