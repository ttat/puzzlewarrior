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

    using UnityEngine;

    public static class GameBoardIo
    {
        #region Public Methods and Operators

        public static bool DeleteGameBoard(
            string filename,
#if UNITY_EDITOR
 string folderPath = @"Assets/Resources/Gameboards/"
#else
            string folderPath = @"Gameboards/"
#endif
)
        {
#if UNITY_EDITOR
            // Make sure it has the right file extension
            string file = filename;
            string extension = Path.GetExtension(filename);
            if (extension == null || !extension.Equals(".txt"))
            {
                file = Path.ChangeExtension(filename, "txt");
            }

            string path = Path.Combine(folderPath, file);
#else
            string path = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(filename));
#endif

            bool deleted;

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    deleted = true;
                }
                else
                {
                    deleted = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Delete Gameboard failed: " + ex.Message);
                deleted = false;
            }

            return deleted;
        }

        public static List<GameBoard> ReadAllGameBoards(
#if UNITY_EDITOR
            string folderPath = @"Assets/Resources/Gameboards/"
#else
            string folderPath = @"Gameboards/"
#endif
            )
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            FileInfo[] puzzleFiles = directoryInfo.GetFiles("*.txt");
            List<GameBoard> gameBoards = new List<GameBoard>();

            foreach (FileInfo puzzleFile in puzzleFiles)
            {
                // Special case: Ignore levelMapping and specialStage files
                if (!puzzleFile.Name.StartsWith("levelMapping", StringComparison.CurrentCultureIgnoreCase) &&
                    !puzzleFile.Name.StartsWith("specialStage", StringComparison.CurrentCultureIgnoreCase))
                {
                    GameBoard gameBoard = ReadGameBoard(puzzleFile.Name, folderPath);

                    if (gameBoard != null)
                    {
                        gameBoards.Add(gameBoard);
                    }
                }
            }

            return gameBoards;
        }

        public static GameBoard ReadGameBoard(
            string filename, 
#if UNITY_EDITOR
            string folderPath = @"Assets/Resources/Gameboards/"
#else
            string folderPath = @"Gameboards/"
#endif
            )
        {
            GameBoard gameBoard = null;

#if UNITY_EDITOR
            // Make sure it has the right file extension
            string file = filename;
            string extension = Path.GetExtension(filename);
            if (extension == null || !extension.Equals(".txt"))
            {
                file = Path.ChangeExtension(filename, "txt");
            }
            
            string path = Path.Combine(folderPath, file);
#else
            string path = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(filename));
#endif
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
                                gameBoard = Serializer.Deserialize<GameBoard>(serializedData);
                                gameBoard.SyncWithGameBlocks();
                            }
                        }

                        reader.Close();
                    }
                }
#else
                TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
                string serializedString = textAsset.text;

                if (!String.IsNullOrEmpty(serializedString))
                {
                    byte[] serializedData = Convert.FromBase64String(serializedString);

                    if (serializedData.Length > 0)
                    {
                        gameBoard = Serializer.Deserialize<GameBoard>(serializedData);
                        gameBoard.SyncWithGameBlocks();
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Read Gameboard failed: " + ex.Message);
            }

            return gameBoard;
        }

        public static string WriteGameBoard(GameBoard gameBoard, string folderPath = @"Assets/Resources/Gameboards/")
        {
            byte[] bytes = Serializer.Serialize(gameBoard);

            string path = Path.Combine(folderPath, string.Format("{0}.txt", gameBoard));

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
            }
            catch (Exception ex)
            {
                Debug.LogError("Saving gameboard failed: " + ex.Message);
            }

            return path;
        }

        #endregion
    }
}