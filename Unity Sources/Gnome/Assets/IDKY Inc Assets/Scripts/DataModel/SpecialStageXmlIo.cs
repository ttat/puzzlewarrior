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
    public static class SpecialStageXmlIo
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

#if UNITY_EDITOR
            string filename = string.Format("{0}_{1}.txt", SpecialStagePrefix, stageId);
#else
            string filename = string.Format("{0}_{1}", SpecialStagePrefix, stageId);
#endif

            string path = Path.Combine(folderPath, filename);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SpecialStage));

#if UNITY_EDITOR
                if (File.Exists(path))
                {
                    using (TextReader reader = new StreamReader(path))
                    {
                        specialStage = serializer.Deserialize(reader) as SpecialStage;

                        if (specialStage == null)
                        {
                            specialStage = new SpecialStage();
                        }
                        else
                        {
                            specialStage.Deserialized();
                        }

                        reader.Close();
                    }
                }
                else
                {
                    specialStage = new SpecialStage();
                }
#else
                Debug.Log("SpecialStage Path: " + path);
                TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
                Debug.Log("TextAsset null? " + textAsset == null ? "yes" : "no");
                Debug.Log("TextAsset text null? " + textAsset.text == null ? "yes" : "no");
                string serializedString = textAsset.text;

                if (!String.IsNullOrEmpty(serializedString))
                {
                    using (TextReader reader = new StringReader(serializedString))
                    {
                        specialStage = serializer.Deserialize(reader) as SpecialStage;

                        if (specialStage == null)
                        {
                            specialStage = new SpecialStage();
                        }
                        else
                        {
                            specialStage.Deserialized();
                        }
                        
                        reader.Close();
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
                string errorMessage = string.Format(
                    "Read SpecialStage failed: {0}\n\nStackTrace: {1}\n\nSource: {2}", ex.Message, ex.StackTrace, ex.Source);
                Debug.LogError(errorMessage);
                specialStage = new SpecialStage();
            }

            return specialStage;
        }

        public static string WriteSpecialStage(
            SpecialStage specialStage, string folderPath = @"Assets/Resources/Gameboards/")
        {
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
                    XmlSerializer serializer = new XmlSerializer(typeof(SpecialStage));
                    specialStage.Serializating();
                    serializer.Serialize(writer, specialStage);
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
                    string headers =
                        string.Format(
                            "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}",
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
                            "MultipleMoves");

                    textWriter.WriteLine(headers);

                    foreach (KeyValuePair<int, string> keyValuePair in specialStage.LevelMapping.Mapping)
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

                            string line =
                                string.Format(
                                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}",
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
                                    multipleMovesCount);

                            textWriter.WriteLine(line);
                        }
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Saving SpecialStage failed: " + ex.Message);
            }

            return path;
        }

        #endregion
    }
}