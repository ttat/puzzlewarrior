// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky.Tests
{
    using System.IO;
    using System.Linq;

    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class IoTests
    {
        #region Fields

        private GameBoard gameBoard;

        private LevelMapping mapping;

        #endregion

        #region Public Methods and Operators

        [Test]
        public void CanReadWriteGameBoardBinary()
        {
            string gameBoardPath = GameBoardIo.WriteGameBoard(this.gameBoard, "Test");
            string fileName = Path.GetFileName(gameBoardPath);

            GameBoard readGameBoard = GameBoardIo.ReadGameBoard(fileName, "Test");

            Assert.AreEqual(this.gameBoard.GetBoardString(), readGameBoard.GetBoardString());
        }

        [Test]
        public void CanReadWriteGameBoardXml()
        {
            string gameBoardPath = GameBoardXmlIo.WriteGameBoard(this.gameBoard, "Test");
            string fileName = Path.GetFileName(gameBoardPath);

            GameBoard readGameBoard = GameBoardXmlIo.ReadGameBoard(fileName, "Test");

            Assert.AreEqual(this.gameBoard.GetBoardString(), readGameBoard.GetBoardString());
        }

        [Test]
        public void CanReadWriteGameBoardXmlFromFile()
        {
            GameBoard readGameBoard = GameBoardXmlIo.ReadGameBoard("gameboardXml.txt", "TestData");

            Assert.AreEqual(this.gameBoard.GetBoardString(), readGameBoard.GetBoardString());
        }

        [Test]
        public void CanReadWriteLevelMappingBinary()
        {
            string levelMappingPath = LevelMappingIo.WriteLevelMapping(this.mapping, "Test");
            string fileName = Path.GetFileName(levelMappingPath);

            LevelMapping readLevelMapping = LevelMappingIo.ReadLevelMapping("Test", fileName);

            bool isEqual = this.mapping.Mapping.SequenceEqual(readLevelMapping.Mapping);
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void CanReadWriteLevelMappingXml()
        {
            string levelMappingPath = LevelMappingXmlIo.WriteLevelMapping(this.mapping, "Test");
            string fileName = Path.GetFileName(levelMappingPath);

            LevelMapping readLevelMapping = LevelMappingXmlIo.ReadLevelMapping("Test", fileName);

            bool isEqual = this.mapping.Mapping.SequenceEqual(readLevelMapping.Mapping);
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void CanReadWriteLevelMappingXmlFromFile()
        {
            LevelMapping readLevelMapping = LevelMappingXmlIo.ReadLevelMapping("TestData", "levelMappingXml.txt");

            bool isEqual = this.mapping.Mapping.SequenceEqual(readLevelMapping.Mapping);
            Assert.IsTrue(isEqual);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Initialize expected values
            this.gameBoard = new GameBoard(2, 3);

            this.gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 0);
            this.gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 0, 1);
            this.gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2);
            this.gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 0);
            this.gameBoard.CreateGameBlock(GameBoard.GameBlockType.Null, 1, 1);
            this.gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 2);

            this.mapping = new LevelMapping();

            this.mapping.GenerateLevelKeys(2);
            this.mapping.Mapping[1] = "Test1";
            this.mapping.Mapping[2] = "Test2";
        }

        #endregion
    }
}