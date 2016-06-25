// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;
    using System.IO;

    using UnityEngine;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class TextureIo
    {
        #region Public Methods and Operators

        public static Texture2D ReadTextureFromFile(string filepath, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            try
            {
                if (File.Exists(filepath))
                {
                    using (BinaryReader reader = new BinaryReader(File.OpenRead(filepath)))
                    {
                        long length = reader.BaseStream.Length;
                        byte[] textureData = reader.ReadBytes((int)length);

                        WWW www = new WWW("file://" + filepath);
                        www.LoadImageIntoTexture(texture);

                        texture.LoadImage(textureData);
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Loading texture failed: " + ex.Message);
            }

            return texture;
        }

        public static void SaveTextureToFile(string filepath, Texture2D texture)
        {
            try
            {
                byte[] texturePng = texture.EncodeToPNG();
                using (BinaryWriter writer = new BinaryWriter(File.Create(filepath)))
                {
                    writer.Write(texturePng);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Saving texture failed: " + ex.Message);
            }
        }

        #endregion
    }
}