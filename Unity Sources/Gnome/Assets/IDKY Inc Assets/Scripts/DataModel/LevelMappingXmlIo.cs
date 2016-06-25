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
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using UnityEngine;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class LevelMappingXmlIo
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
                XmlSerializer serializer = new XmlSerializer(typeof(LevelMapping));

#if UNITY_EDITOR
                if (File.Exists(path))
                {
                    using (TextReader reader = new StreamReader(path))
                    {
                        levelMapping = serializer.Deserialize(reader) as LevelMapping;

                        if (levelMapping == null)
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
                    using (TextReader reader = new StringReader(serializedString))
                    {
                        levelMapping = serializer.Deserialize(reader) as LevelMapping;

                        if (levelMapping == null)
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
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Read LevelMapping failed: " + ex.Message);
                levelMapping = new LevelMapping();
            }
            
            levelMapping.Deserialized();

            return levelMapping;
        }

        public static string WriteLevelMapping(
            LevelMapping levelMapping, string folderPath = @"Assets/Resources/Gameboards/", string fileName = "levelMapping.txt")
        {
            string path = Path.Combine(folderPath, fileName);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (TextWriter writer = File.CreateText(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LevelMapping));
                    levelMapping.Serializating();
                    serializer.Serialize(writer, levelMapping);
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
                    string headers =
                        string.Format(
                            "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}",
                            "Level",
                            "LevelId",
                            "Successes",
                            "Failures",
                            "NumBlockTypes",
                            "NumPlayers",
                            "ChangeDirection",
                            "ExtraMove",
                            "MultipleMoves",
                            "PlayersCount1",
                            "PlayersCount2",
                            "PlayersCount3",
                            "PlayersCount4",
                            "PlayersCount5",
                            "PlayersCount6",
                            "PlayersCount7",
                            "NormalCount",
                            "ChangeDirectionCount",
                            "ExtraMovesCount",
                            "MultipleMoves",
                            "CalculatedDifficulty");

                    textWriter.WriteLine(headers);

                    foreach (KeyValuePair<int, string> keyValuePair in levelMapping.Mapping)
                    {
                        if (!string.IsNullOrEmpty(keyValuePair.Value))
                        {
                            // Fill out the block types it contains
                            GameBoard readGameBoard = GameBoardXmlIo.ReadGameBoard(keyValuePair.Value, folderPath);
                            HashSet<Type> containingBlockTypes = readGameBoard.GetContainingBlockTypes();

                            List<GameBlockPlayer> gameBlockPlayers = readGameBoard.GameBlocks.OfType<GameBlockPlayer>().ToList();
                            int playersCount1 = gameBlockPlayers.Count(p => p.AvailableMoves == 1);
                            int playersCount2 = gameBlockPlayers.Count(p => p.AvailableMoves == 2);
                            int playersCount3 = gameBlockPlayers.Count(p => p.AvailableMoves == 3);
                            int playersCount4 = gameBlockPlayers.Count(p => p.AvailableMoves == 4);
                            int playersCount5 = gameBlockPlayers.Count(p => p.AvailableMoves == 5);
                            int playersCount6 = gameBlockPlayers.Count(p => p.AvailableMoves == 6);
                            int playersCount7 = gameBlockPlayers.Count(p => p.AvailableMoves == 7);

                            int normalCount = readGameBoard.GameBlocks.OfType<GameBlockNormal>().Count();
                            int changeDirectionCount = readGameBoard.GameBlocks.OfType<GameBlockChangeDirection>().Count();
                            int extraMoveCount = readGameBoard.GameBlocks.OfType<GameBlockExtraMove>().Count();
                            int multipleMovesCount = readGameBoard.GameBlocks.OfType<GameBlockMultipleMoves>().Count();

                            double difficulty = readGameBoard.GetDifficulty();

                            string line =
                                string.Format(
                                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20:F2}",
                                    keyValuePair.Key,
                                    keyValuePair.Value ?? string.Empty,
                                    readGameBoard.Successes,
                                    readGameBoard.Failures,
                                    containingBlockTypes.Count - 2,
                                    gameBlockPlayers.Count,
                                    containingBlockTypes.Contains(typeof(GameBlockChangeDirection)) ? "X" : " ",
                                    containingBlockTypes.Contains(typeof(GameBlockExtraMove)) ? "X" : " ",
                                    containingBlockTypes.Contains(typeof(GameBlockMultipleMoves)) ? "X" : " ",
                                    playersCount1,
                                    playersCount2,
                                    playersCount3,
                                    playersCount4,
                                    playersCount5,
                                    playersCount6,
                                    playersCount7,
                                    normalCount,
                                    changeDirectionCount,
                                    extraMoveCount,
                                    multipleMovesCount,
                                    difficulty);

                            textWriter.WriteLine(line);
                        }
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