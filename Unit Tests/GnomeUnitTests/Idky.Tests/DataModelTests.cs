// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class DataModelTests
    {
        #region Public Methods and Operators

        [Test]
        public void TestReverseIsTrueCorrectly()
        {
            GameBoard gameBoard = new GameBoard(1, 3);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;

            gameBlockNormal1.SetAvailability(false);
            gameBlockNormal2.SetAvailability(false);

            bool canReverseSolve = gameBoard.TestCanReverseSolve();

            Assert.IsTrue(canReverseSolve);
        }

        [Test]
        public void TestReverseIsFalseCorrectly()
        {
            GameBoard gameBoard = new GameBoard(1, 3);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;

            gameBlockNormal1.SetAvailability(true);
            gameBlockNormal2.SetAvailability(false);

            bool canReverseSolve = gameBoard.TestCanReverseSolve();

            Assert.IsFalse(canReverseSolve);
        }

        [Test]
        public void TestReverseIsFalseCorrectlyPlayerUsed()
        {
            GameBoard gameBoard = new GameBoard(1, 3);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;

            gameBlockPlayer.AvailableMoves = 1;
            gameBlockNormal1.SetAvailability(false);
            gameBlockNormal2.SetAvailability(false);

            bool canReverseSolve = gameBoard.TestCanReverseSolve();

            Assert.IsFalse(canReverseSolve);
        }

        [Test]
        public void ChecksApplyMoveWithLoopedChangeDirection()
        {
            GameBoard gameBoard = new GameBoard(2, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Null, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 1, MovementDirection.Right);
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 3, MovementDirection.Down);
            GameBlockPlayer player = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0) as GameBlockPlayer;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 1, MovementDirection.Up);
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2) as GameBlockNormal;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 3, MovementDirection.Left);

            gameBlockNormal1.SetAvailability(true);
            gameBlockNormal2.SetAvailability(true);

            player.AvailableMoves = 3;
            BlockMovement move = new BlockMovement(player, MovementDirection.Right);
            bool applyMove = player.ApplyMove(null, MovementDirection.Right, move);

            Assert.IsFalse(applyMove);
        }

        [Test]
        public void ChecksApplyMoveWithLoopedChangeDirection2()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            GameBlockPlayer player = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 1, MovementDirection.Right);
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 3, MovementDirection.Left);

            gameBlockNormal1.SetAvailability(true);

            player.AvailableMoves = 2;
            BlockMovement move = new BlockMovement(player, MovementDirection.Right);
            bool applyMove = player.ApplyMove(null, MovementDirection.Right, move);

            Assert.IsFalse(applyMove);
        }

        [Test]
        public void ChecksApplyMultipleMoves()
        {
            GameBoard gameBoard = new GameBoard(3, 3);

            GameBlockPlayer player1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockMultipleMoves gameBlockMultiple =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2, numberOfMultipleMoves: 2) as GameBlockMultipleMoves;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2) as GameBlockNormal;
            GameBlockPlayer player2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2) as GameBlockPlayer;

            gameBlockNormal1.SetAvailability(true);
            gameBlockNormal2.SetAvailability(true);
            gameBlockMultiple.NumberOfMovesApplied = 0;

            player1.AvailableMoves = 2;
            player2.AvailableMoves = 2;
            BlockMovement move1 = new BlockMovement(player1, MovementDirection.Right);
            BlockMovement move2 = new BlockMovement(player2, MovementDirection.Up);
            bool applyMove1 = player1.ApplyMove(null, MovementDirection.Right, move1);
            bool applyMove2 = player2.ApplyMove(null, MovementDirection.Up, move2);

            Assert.IsTrue(applyMove1);
            Assert.IsTrue(applyMove2);
        }

        [Test]
        public void ChecksApplyMultipleMovesNotEnough()
        {
            GameBoard gameBoard = new GameBoard(3, 5);

            GameBlockPlayer player1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockMultipleMoves gameBlockMultiple =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2, numberOfMultipleMoves: 2) as GameBlockMultipleMoves;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2) as GameBlockNormal;
            GameBlockPlayer player2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 3) as GameBlockNormal;
            GameBlockPlayer player3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 4) as GameBlockPlayer;

            gameBlockNormal1.SetAvailability(true);
            gameBlockNormal2.SetAvailability(true);
            gameBlockNormal3.SetAvailability(true);
            gameBlockMultiple.NumberOfMovesApplied = 0;

            player1.AvailableMoves = 2;
            player2.AvailableMoves = 2;
            player3.AvailableMoves = 2;
            BlockMovement move1 = new BlockMovement(player1, MovementDirection.Right);
            BlockMovement move2 = new BlockMovement(player2, MovementDirection.Up);
            BlockMovement move3 = new BlockMovement(player3, MovementDirection.Left);
            bool applyMove1 = player1.ApplyMove(null, MovementDirection.Right, move1);
            bool applyMove2 = player2.ApplyMove(null, MovementDirection.Up, move2);
            bool applyMove3 = player3.ApplyMove(null, MovementDirection.Left, move3);

            Assert.IsTrue(applyMove1);
            Assert.IsTrue(applyMove2);
            Assert.IsFalse(applyMove3);
        }

        [Test]
        public void ChecksApplyMultipleMovesWithLoop()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            GameBlockPlayer player1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockMultipleMoves gameBlockMultiple =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2, numberOfMultipleMoves: 2) as GameBlockMultipleMoves;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 3, MovementDirection.Left);

            gameBlockNormal1.SetAvailability(true);
            gameBlockMultiple.NumberOfMovesApplied = 0;

            player1.AvailableMoves = 3;
            BlockMovement move = new BlockMovement(player1, MovementDirection.Right);
            bool applyMove1 = player1.ApplyMove(null, MovementDirection.Right, move);

            Assert.IsTrue(applyMove1);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ChecksMultipleMovesOrphanedState(bool isOrphan)
        {
            GameBoard gameBoard = new GameBoard(1, 3);

            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 0) as GameBlockNormal;
            GameBlockMultipleMoves gameBlockMultiple =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 1, numberOfMultipleMoves: 2) as GameBlockMultipleMoves;

            gameBlockNormal1.SetAvailability(true);
            gameBlockMultiple.NumberOfMovesApplied = 0;

            if (!isOrphan)
            {
                GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;
                gameBlockNormal2.SetAvailability(true);
            }

            bool isOrphaned = gameBlockMultiple.IsOrphaned();

            Assert.AreEqual(isOrphan, isOrphaned);
        }

        [Test]
        public void ChecksUsedBlocksWithLoopedChangeDirection()
        {
            GameBoard gameBoard = new GameBoard(2, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            IGameBlock gameBlock1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 1, MovementDirection.Right);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2);
            IGameBlock gameBlock2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 3, MovementDirection.Down);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            IGameBlock gameBlock3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 1, MovementDirection.Up);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            IGameBlock gameBlock4 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 3, MovementDirection.Left);

            int countUsedBlocks1 = gameBlock1.CountUsedBlocks(MovementDirection.Down, null);
            int countUsedBlocks2 = gameBlock2.CountUsedBlocks(MovementDirection.Left, null);
            int countUsedBlocks3 = gameBlock3.CountUsedBlocks(MovementDirection.Right, null);
            int countUsedBlocks4 = gameBlock4.CountUsedBlocks(MovementDirection.Up, null);

            Assert.AreEqual(2, countUsedBlocks1);
            Assert.AreEqual(2, countUsedBlocks2);
            Assert.AreEqual(2, countUsedBlocks3);
            Assert.AreEqual(2, countUsedBlocks4);
        }

        [Test]
        public void ChecksUsedBlocksWithMultipleChangeDirection()
        {
            GameBoard gameBoard = new GameBoard(3, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            IGameBlock gameBlock1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 1);
            IGameBlock gameBlock2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 3);

            int countUsedBlocks1 = gameBlock1.CountUsedBlocks(MovementDirection.Left, null);
            int countUsedBlocks2 = gameBlock2.CountUsedBlocks(MovementDirection.Right, null);

            Assert.AreEqual(0, countUsedBlocks1);
            Assert.AreEqual(0, countUsedBlocks2);
        }

        [Test]
        public void CorrectlyChecksNotOrphanedGameBoard()
        {
            GameBoard gameBoard = new GameBoard(3, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 2);

            Assert.IsFalse(gameBoard.HasOrphan());
        }

        [Test]
        public void CorrectlyChecksOrphanedGameBoardWithGameBlockPlayers()
        {
            GameBoard gameBoard = new GameBoard(3, 3);

            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer).AvailableMoves = 1;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1) as GameBlockPlayer).AvailableMoves = 1;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2) as GameBlockPlayer).AvailableMoves = 1;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0) as GameBlockPlayer).AvailableMoves = 1;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 2) as GameBlockPlayer).AvailableMoves = 1;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0) as GameBlockPlayer).AvailableMoves = 1;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 1) as GameBlockPlayer).AvailableMoves = 1;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2) as GameBlockPlayer).AvailableMoves = 1;

            GameBlockNormal gameBlock = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1) as GameBlockNormal;
            gameBlock.SetAvailability(true);

            Assert.IsTrue(gameBoard.HasOrphan());
        }
        [Test]
        public void CorrectlyChecksOrphanedGameBoard()
        {
            GameBoard gameBoard = new GameBoard(3, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 2);

            GameBlockNormal gameBlock = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1) as GameBlockNormal;
            gameBlock.SetAvailability(true);

            Assert.IsTrue(gameBoard.HasOrphan());
        }

        [Test]
        public void CorrectlyChecksSolvedGameBoard()
        {
            GameBoard gameBoard = new GameBoard(4, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 2);

            Assert.IsTrue(gameBoard.IsSolved());
        }

        [Test]
        public void CorrectlyChecksUnsolvedGameBoard()
        {
            GameBoard gameBoard = new GameBoard(4, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            GameBlockNormal gameBlockNormal21 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1) as GameBlockNormal;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 2);

            gameBlockNormal21.SetAvailability(true);

            Assert.IsTrue(!gameBoard.IsSolved());
        }

        [Test]
        public void CorrectlyMovesWithExtraBlock()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockExtraMove gameBlockExtraMove = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 0, 1) as GameBlockExtraMove;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 3) as GameBlockNormal;

            gameBlockPlayer.AvailableMoves = 1;
            gameBlockNormal1.SetAvailability(true);
            gameBlockNormal2.SetAvailability(true);

            BlockMovement move = new BlockMovement(gameBlockPlayer, MovementDirection.Right);
            Assert.IsTrue(gameBlockPlayer.ApplyMove(null, MovementDirection.Right, move));
            Assert.IsFalse(gameBlockNormal1.IsAvailable);
            Assert.IsFalse(gameBlockNormal2.IsAvailable);
        }

        [Test]
        public void DeniesMoveWithExtraBlockWithNotEnoughBlocks()
        {
            GameBoard gameBoard = new GameBoard(1, 3);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockExtraMove gameBlockExtraMove = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 0, 1) as GameBlockExtraMove;
            GameBlockNormal gameBlockNormal = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;

            gameBlockPlayer.AvailableMoves = 1;
            gameBlockNormal.SetAvailability(true);

            BlockMovement move = new BlockMovement(gameBlockPlayer, MovementDirection.Right);
            Assert.IsFalse(gameBlockPlayer.ApplyMove(null, MovementDirection.Right, move));
            Assert.IsTrue(gameBlockNormal.IsAvailable);
        }

        [Test]
        [TestCase(MovementDirection.Up, 4, 2, MovementDirection.Up, new[] { 1, 3 }, new[] { 2, 2 }, true)]
        [TestCase(MovementDirection.Up, 2, 0, MovementDirection.Right, new[] { 1, 2 }, new[] { 2, 1 }, true)]
        [TestCase(MovementDirection.Up, 2, 4, MovementDirection.Left, new[] { 1, 2 }, new[] { 2, 3 }, true)]
        [TestCase(MovementDirection.Up, 0, 2, MovementDirection.Down, new int[] { }, new int[] { }, false)]
        [TestCase(MovementDirection.Down, 4, 2, MovementDirection.Up, new int[] { }, new int[] { }, false)]
        [TestCase(MovementDirection.Down, 2, 0, MovementDirection.Right, new[] { 3, 2 }, new[] { 2, 1 }, true)]
        [TestCase(MovementDirection.Down, 2, 4, MovementDirection.Left, new[] { 3, 2 }, new[] { 2, 3 }, true)]
        [TestCase(MovementDirection.Down, 0, 2, MovementDirection.Down, new[] { 3, 1 }, new[] { 2, 2 }, true)]
        [TestCase(MovementDirection.Left, 4, 2, MovementDirection.Up, new[] { 2, 3 }, new[] { 1, 2 }, true)]
        [TestCase(MovementDirection.Left, 2, 0, MovementDirection.Right, new int[] { }, new int[] { }, false)]
        [TestCase(MovementDirection.Left, 2, 4, MovementDirection.Left, new[] { 2, 2 }, new[] { 1, 3 }, true)]
        [TestCase(MovementDirection.Left, 0, 2, MovementDirection.Down, new[] { 2, 1 }, new[] { 1, 2 }, true)]
        [TestCase(MovementDirection.Right, 4, 2, MovementDirection.Up, new[] { 2, 3 }, new[] { 3, 2 }, true)]
        [TestCase(MovementDirection.Right, 2, 0, MovementDirection.Right, new[] { 2, 2 }, new[] { 3, 1 }, true)]
        [TestCase(MovementDirection.Right, 2, 4, MovementDirection.Left, new int[] { }, new int[] { }, false)]
        [TestCase(MovementDirection.Right, 0, 2, MovementDirection.Down, new[] { 2, 1 }, new[] { 3, 2 }, true)]
        public void ForceChangeDirectionCorrectly(
            MovementDirection forceDirection,
            int playerRow,
            int playerColumn,
            MovementDirection playerMovementDirection,
            int[] filledX,
            int[] filledY,
            bool moveGetsApplied)
        {
            GameBoard gameBoard = new GameBoard(5, 5);

            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2) as IGameBlockParent).AvailableMoves = 2;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0) as IGameBlockParent).AvailableMoves = 2;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 4) as IGameBlockParent).AvailableMoves = 2;
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 2) as IGameBlockParent).AvailableMoves = 2;

            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2) as IGameBlockDestination).SetAvailability(true);
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1) as IGameBlockDestination).SetAvailability(true);
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 3) as IGameBlockDestination).SetAvailability(true);
            (gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2) as IGameBlockDestination).SetAvailability(true);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 2, 2, forceDirection);

            IGameBlockParent gameBlockParent = gameBoard.GameBlocks[playerRow, playerColumn] as IGameBlockParent;
            BlockMovement move = new BlockMovement(gameBlockParent, playerMovementDirection);
            bool moveApplied = gameBlockParent.ApplyMove(null, playerMovementDirection, move);

            Assert.AreEqual(moveGetsApplied, moveApplied);

            for (int i = 0; i < filledX.Length; i++)
            {
                IGameBlockDestination gameBlock = gameBoard.GameBlocks[filledX[i], filledY[i]] as IGameBlockDestination;

                Assert.IsFalse(gameBlock.IsAvailable);
            }
        }

        [Test]
        // Moves successfully
        [TestCase(1, MovementDirection.Up, 1, 0, true, false, 2, 1)]
        [TestCase(2, MovementDirection.Down, 1, 0, true, false, 0, 1)]
        [TestCase(3, MovementDirection.Left, 1, 0, true, false, 1, 2)]
        [TestCase(4, MovementDirection.Right, 1, 0, true, false, 1, 0)]
        // Moves unsuccessfully with extra moves
        [TestCase(11, MovementDirection.Up, 2, 2, true, true, 2, 1)]
        [TestCase(12, MovementDirection.Down, 2, 2, true, true, 0, 1)]
        [TestCase(13, MovementDirection.Left, 2, 2, true, true, 1, 2)]
        [TestCase(14, MovementDirection.Right, 2, 2, true, true, 1, 0)]
        // Moves unsuccessfully because in the wrong direction
        [TestCase(21, MovementDirection.Up, 1, 1, true, true, 0, 1)]
        [TestCase(22, MovementDirection.Down, 1, 1, true, true, 2, 1)]
        [TestCase(23, MovementDirection.Left, 1, 1, true, true, 1, 0)]
        [TestCase(24, MovementDirection.Right, 1, 1, true, true, 1, 2)]
        // Moves unsuccessfully because not available
        [TestCase(31, MovementDirection.Up, 1, 1, false, false, 2, 1)]
        [TestCase(32, MovementDirection.Down, 1, 1, false, false, 0, 1)]
        [TestCase(33, MovementDirection.Left, 1, 1, false, false, 1, 2)]
        [TestCase(34, MovementDirection.Right, 1, 1, false, false, 1, 0)]
        public void MoveOneGameBlock(
            int nothing,
            MovementDirection direction,
            int startAvailableMoves,
            int endAvailableMoves,
            bool startAvailability,
            bool endAvailability,
            int playerRow,
            int playerColumn)
        {
            GameBoard gameBoard = new GameBoard(3, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2);

            GameBlockNormal gameBlockNormal = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1) as GameBlockNormal;
            gameBlockNormal.SetAvailability(startAvailability);

            GameBlockPlayer gameBlockPlayer = gameBoard.GameBlocks[playerRow, playerColumn] as GameBlockPlayer;
            gameBlockPlayer.AvailableMoves = startAvailableMoves;

            BlockMovement move = new BlockMovement(gameBlockPlayer, direction);
            gameBlockPlayer.ApplyMove(null, direction, move);

            Assert.AreEqual(endAvailability, gameBlockNormal.IsAvailable);
            Assert.AreEqual(endAvailableMoves, gameBlockPlayer.AvailableMoves);
        }

        [Test]
        // Moves skipping middle block successfully
        [TestCase(1, MovementDirection.Up, 2, 0, new[] { true, false, true }, new[] { false, false, false }, 4)]
        [TestCase(2, MovementDirection.Down, 2, 0, new[] { true, false, true }, new[] { false, false, false }, 0)]
        // Moves skipping closest block successfully
        [TestCase(11, MovementDirection.Up, 2, 0, new[] { true, true, false }, new[] { false, false, false }, 4)]
        [TestCase(12, MovementDirection.Down, 2, 0, new[] { false, true, true }, new[] { false, false, false }, 0)]
        // Fails because too many moves
        [TestCase(21, MovementDirection.Up, 3, 3, new[] { true, false, true }, new[] { true, false, true }, 4)]
        [TestCase(22, MovementDirection.Down, 3, 3, new[] { true, false, true }, new[] { true, false, true }, 0)]
        public void MoveThreeByOneGameBlock(
            int nothing,
            MovementDirection direction,
            int startAvailableMoves,
            int endAvailableMoves,
            bool[] startAvailability,
            bool[] endAvailability,
            int playerRow)
        {
            GameBoard gameBoard = new GameBoard(5, 1);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 0);

            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 0) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 0) as GameBlockNormal;
            GameBlockNormal gameBlockNormal3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 0) as GameBlockNormal;

            gameBlockNormal1.SetAvailability(startAvailability[0]);
            gameBlockNormal2.SetAvailability(startAvailability[1]);
            gameBlockNormal3.SetAvailability(startAvailability[2]);

            GameBlockPlayer gameBlockPlayer = gameBoard.GameBlocks[playerRow, 0] as GameBlockPlayer;
            gameBlockPlayer.AvailableMoves = startAvailableMoves;

            BlockMovement move = new BlockMovement(gameBlockPlayer, direction);
            gameBlockPlayer.ApplyMove(null, direction, move);

            Assert.AreEqual(endAvailability[0], gameBlockNormal1.IsAvailable);
            Assert.AreEqual(endAvailability[1], gameBlockNormal2.IsAvailable);
            Assert.AreEqual(endAvailability[2], gameBlockNormal3.IsAvailable);
            Assert.AreEqual(endAvailableMoves, gameBlockPlayer.AvailableMoves);
        }

        [Test]
        // Moves successfully all free
        [TestCase(1, MovementDirection.Up, 1, 0, new[] { true, true, true, true }, new[] { true, true, false, true }, 3, 1)]
        [TestCase(2, MovementDirection.Up, 1, 0, new[] { true, true, true, true }, new[] { true, true, true, false }, 3, 2)]
        [TestCase(3, MovementDirection.Down, 1, 0, new[] { true, true, true, true }, new[] { false, true, true, true }, 0, 1)]
        [TestCase(4, MovementDirection.Down, 1, 0, new[] { true, true, true, true }, new[] { true, false, true, true }, 0, 2)]
        [TestCase(5, MovementDirection.Left, 1, 0, new[] { true, true, true, true }, new[] { true, false, true, true }, 1, 3)]
        [TestCase(6, MovementDirection.Left, 1, 0, new[] { true, true, true, true }, new[] { true, true, true, false }, 2, 3)]
        [TestCase(7, MovementDirection.Right, 1, 0, new[] { true, true, true, true }, new[] { false, true, true, true }, 1, 0)]
        [TestCase(8, MovementDirection.Right, 1, 0, new[] { true, true, true, true }, new[] { true, true, false, true }, 2, 0)]
        // Moves successfully next block
        [TestCase(11, MovementDirection.Up, 1, 0, new[] { true, true, false, false }, new[] { false, true, false, false }, 3, 1)]
        [TestCase(12, MovementDirection.Up, 1, 0, new[] { true, true, false, false }, new[] { true, false, false, false }, 3, 2)]
        [TestCase(13, MovementDirection.Down, 1, 0, new[] { false, false, true, true }, new[] { false, false, false, true }, 0, 1)]
        [TestCase(14, MovementDirection.Down, 1, 0, new[] { false, false, true, true }, new[] { false, false, true, false }, 0, 2)]
        [TestCase(15, MovementDirection.Left, 1, 0, new[] { true, false, true, false }, new[] { false, false, true, false }, 1, 3)]
        [TestCase(16, MovementDirection.Left, 1, 0, new[] { true, false, true, false }, new[] { true, false, false, false }, 2, 3)]
        [TestCase(17, MovementDirection.Right, 1, 0, new[] { false, true, false, true }, new[] { false, false, false, true }, 1, 0)]
        [TestCase(18, MovementDirection.Right, 1, 0, new[] { false, true, false, true }, new[] { false, true, false, false }, 2, 0)]
        // Moves unsuccessfully next block with too many moves
        [TestCase(21, MovementDirection.Up, 2, 2, new[] { true, true, false, false }, new[] { true, true, false, false }, 3, 1)]
        [TestCase(22, MovementDirection.Up, 2, 2, new[] { true, true, false, false }, new[] { true, true, false, false }, 3, 2)]
        [TestCase(23, MovementDirection.Down, 2, 2, new[] { false, false, true, true }, new[] { false, false, true, true }, 0, 1)]
        [TestCase(24, MovementDirection.Down, 2, 2, new[] { false, false, true, true }, new[] { false, false, true, true }, 0, 2)]
        [TestCase(25, MovementDirection.Left, 2, 2, new[] { true, false, true, false }, new[] { true, false, true, false }, 1, 3)]
        [TestCase(26, MovementDirection.Left, 2, 2, new[] { true, false, true, false }, new[] { true, false, true, false }, 2, 3)]
        [TestCase(27, MovementDirection.Right, 2, 2, new[] { false, true, false, true }, new[] { false, true, false, true }, 1, 0)]
        [TestCase(28, MovementDirection.Right, 2, 2, new[] { false, true, false, true }, new[] { false, true, false, true }, 2, 0)]
        // Moves unsuccessfully
        [TestCase(31, MovementDirection.Up, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 3, 1)]
        [TestCase(32, MovementDirection.Up, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 3, 2)]
        [TestCase(33, MovementDirection.Down, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 0, 1)]
        [TestCase(34, MovementDirection.Down, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 0, 2)]
        [TestCase(35, MovementDirection.Left, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 1, 3)]
        [TestCase(36, MovementDirection.Left, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 2, 3)]
        [TestCase(37, MovementDirection.Right, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 1, 0)]
        [TestCase(38, MovementDirection.Right, 1, 1, new[] { false, false, false, false }, new[] { false, false, false, false }, 2, 0)]
        public void MoveTwoByTwoGameBlock(
            int nothing,
            MovementDirection direction,
            int startAvailableMoves,
            int endAvailableMoves,
            bool[] startAvailability,
            bool[] endAvailability,
            int playerRow,
            int playerColumn)
        {
            GameBoard gameBoard = new GameBoard(4, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);

            GameBlockNormal gameBlockNormal11 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1) as GameBlockNormal;
            GameBlockNormal gameBlockNormal12 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2) as GameBlockNormal;
            GameBlockNormal gameBlockNormal21 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1) as GameBlockNormal;
            GameBlockNormal gameBlockNormal22 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 2) as GameBlockNormal;
            gameBlockNormal11.SetAvailability(startAvailability[0]);
            gameBlockNormal12.SetAvailability(startAvailability[1]);
            gameBlockNormal21.SetAvailability(startAvailability[2]);
            gameBlockNormal22.SetAvailability(startAvailability[3]);

            GameBlockPlayer gameBlockPlayer = gameBoard.GameBlocks[playerRow, playerColumn] as GameBlockPlayer;
            gameBlockPlayer.AvailableMoves = startAvailableMoves;

            BlockMovement move = new BlockMovement(gameBlockPlayer, direction);
            gameBlockPlayer.ApplyMove(null, direction, move);

            Assert.AreEqual(endAvailability[0], gameBlockNormal11.IsAvailable);
            Assert.AreEqual(endAvailability[1], gameBlockNormal12.IsAvailable);
            Assert.AreEqual(endAvailability[2], gameBlockNormal21.IsAvailable);
            Assert.AreEqual(endAvailability[3], gameBlockNormal22.IsAvailable);
            Assert.AreEqual(endAvailableMoves, gameBlockPlayer.AvailableMoves);
        }

        [Test]
        [TestCase(MovementDirection.Up, new[] { 2, 3 }, new[] { 3, 2 }, MovementDirection.Up, 1, 2, 2, 0)]
        [TestCase(MovementDirection.Up, new[] { 2, 2 }, new[] { 1, 3 }, MovementDirection.Up, 1, 2, 4, 2)]
        [TestCase(MovementDirection.Up, new[] { 2, 3 }, new[] { 1, 2 }, MovementDirection.Up, 1, 2, 2, 4)]
        [TestCase(MovementDirection.Down, new[] { 2, 1 }, new[] { 1, 2 }, MovementDirection.Down, 3, 2, 2, 4)]
        [TestCase(MovementDirection.Down, new[] { 2, 2 }, new[] { 1, 3 }, MovementDirection.Down, 3, 2, 0, 2)]
        [TestCase(MovementDirection.Down, new[] { 2, 1 }, new[] { 3, 2 }, MovementDirection.Down, 3, 2, 2, 0)]
        [TestCase(MovementDirection.Left, new[] { 1, 2 }, new[] { 2, 3 }, MovementDirection.Left, 2, 1, 4, 2)]
        [TestCase(MovementDirection.Left, new[] { 1, 3 }, new[] { 2, 2 }, MovementDirection.Left, 2, 1, 2, 4)]
        [TestCase(MovementDirection.Left, new[] { 3, 2 }, new[] { 2, 3 }, MovementDirection.Left, 2, 1, 0, 2)]
        [TestCase(MovementDirection.Right, new[] { 1, 2 }, new[] { 2, 1 }, MovementDirection.Right, 2, 3, 4, 2)]
        [TestCase(MovementDirection.Right, new[] { 1, 3 }, new[] { 2, 2 }, MovementDirection.Right, 2, 3, 2, 0)]
        [TestCase(MovementDirection.Right, new[] { 3, 2 }, new[] { 2, 1 }, MovementDirection.Right, 2, 3, 0, 2)]
        public void ReverseChangeDirectionFindsMoves(
            MovementDirection forceDirection,
            int[] availableX,
            int[] availableY,
            MovementDirection destinationReverseDirection,
            int destinationX,
            int destinationY,
            int startX,
            int startY)
        {
            GameBoard gameBoard = new GameBoard(5, 5);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 2);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 2, 2, forceDirection);

            for (int i = 0; i < availableX.Length; i++)
            {
                (gameBoard.GameBlocks[availableX[i], availableY[i]] as IGameBlockDestination).SetAvailability(true);
            }

            IGameBlockDestination destination = gameBoard.GameBlocks[destinationX, destinationY] as IGameBlockDestination;
            BlockMovement move;
            bool applyReverseMove = destination.ApplyReverseMove(
                null, destinationReverseDirection, 2, out move, null, null, new Queue<IGameBlock>());

            Assert.IsTrue(applyReverseMove);

            IGameBlockParent parent = gameBoard.GameBlocks[startX, startY] as IGameBlockParent;

            Assert.AreEqual(2, parent.AvailableMoves);
        }

        [Test]
        public void ReverseExtraMoveFailsIfNotEnoughPreviousBlocks()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockExtraMove gameBlockExtraMove = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 0, 1) as GameBlockExtraMove;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;

            BlockMovement movement;
            bool applied = gameBlockNormal1.ApplyReverseMove(
                null, MovementDirection.Right, 1, out movement, null, null, new Queue<IGameBlock>());

            Assert.IsFalse(applied);
        }

        [Test]
        public void ReverseExtraMoveGivesCorrectNumberOfMoves()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockExtraMove gameBlockExtraMove = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 0, 1) as GameBlockExtraMove;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 3) as GameBlockNormal;

            BlockMovement movement;
            bool applied = gameBlockNormal2.ApplyReverseMove(
                null, MovementDirection.Right, 1, out movement, null, null, new Queue<IGameBlock>());

            Assert.IsTrue(applied);
            Assert.AreEqual(1, gameBlockPlayer.AvailableMoves);
            Assert.AreSame(gameBlockNormal2, movement.DestinationBlock);
            Assert.IsTrue(movement.IntermediateBlocks.Contains(gameBlockNormal1));
        }

        [Test]
        public void ReverseExtraMoveGivesCorrectNumberOfMoves2()
        {
            GameBoard gameBoard = new GameBoard(1, 5);

            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockExtraMove gameBlockExtraMove = gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 0, 2) as GameBlockExtraMove;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 3) as GameBlockNormal;
            GameBlockNormal gameBlockNormal3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 4) as GameBlockNormal;

            BlockMovement movement;
            bool applied = gameBlockNormal3.ApplyReverseMove(
                null, MovementDirection.Right, 3, out movement, null, null, new Queue<IGameBlock>());

            Assert.IsTrue(applied);
            Assert.AreEqual(2, gameBlockPlayer.AvailableMoves);
            Assert.IsTrue(movement.IntermediateBlocks.Contains(gameBlockNormal1));
            Assert.IsTrue(movement.IntermediateBlocks.Contains(gameBlockNormal2));
            Assert.AreSame(movement.DestinationBlock, gameBlockNormal3);
        }

        [Test]
        public void ReverseMoveFailsIfAlreadyUsed()
        {
            GameBoard gameBoard = new GameBoard(3, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            GameBlockPlayer gameBlockPlayer = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1) as GameBlockPlayer;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2);

            GameBlockNormal gameBlockNormal = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1) as GameBlockNormal;

            gameBlockPlayer.AvailableMoves = 1;

            BlockMovement movement;
            bool applied = gameBlockNormal.ApplyReverseMove(
                null, MovementDirection.Down, 1, out movement, null, null, new Queue<IGameBlock>());

            Assert.IsFalse(applied);
        }

        [Test]
        // Successfully finds one move
        [TestCase(1, MovementDirection.Up, true, 1)]
        [TestCase(2, MovementDirection.Down, true, 1)]
        [TestCase(3, MovementDirection.Left, true, 1)]
        [TestCase(4, MovementDirection.Right, true, 1)]
        // Fails because too many moves required
        [TestCase(11, MovementDirection.Up, false, 2)]
        [TestCase(12, MovementDirection.Down, false, 2)]
        [TestCase(13, MovementDirection.Left, false, 2)]
        [TestCase(14, MovementDirection.Right, false, 2)]
        public void ReverseMoveOneGameBlock(int nothing, MovementDirection direction, bool shouldBeApplied, int startingPointAvailableMoves)
        {
            GameBoard gameBoard = new GameBoard();
            gameBoard.GameBlocks = new IGameBlock[3,3];

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            GameBlockNormal gameBlockNormal = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1) as GameBlockNormal;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2);

            gameBlockNormal.SetAvailability(false);

            BlockMovement movement;
            bool applied = gameBlockNormal.ApplyReverseMove(
                null, direction, startingPointAvailableMoves, out movement, null, null, new Queue<IGameBlock>());

            Assert.AreEqual(shouldBeApplied, applied);

            if (shouldBeApplied)
            {
                Assert.AreEqual(startingPointAvailableMoves, movement.SourceBlock.AvailableMoves);
            }
        }

        [Test]
        // Successfully applied reverse move
        [TestCase(1, MovementDirection.Up, true, 2, 2, new[] { false, false }, new[] { true, true }, true)]
        [TestCase(2, MovementDirection.Down, true, 2, 2, new[] { false, false }, new[] { true, true }, false)]
        // Successfully applied reverse move with extra
        [TestCase(1, MovementDirection.Up, true, 1, 2, new[] { false, false }, new[] { true, true }, true)]
        [TestCase(2, MovementDirection.Down, true, 1, 2, new[] { false, false }, new[] { true, true }, false)]
        // Fails because requesting too many moves
        [TestCase(1, MovementDirection.Up, false, 3, 0, new[] { false, false }, new[] { false, false }, true)]
        [TestCase(2, MovementDirection.Down, false, 3, 0, new[] { false, false }, new[] { false, false }, false)]
        public void ReverseMoveTwoOneGameBlock(
            int nothing,
            MovementDirection direction,
            bool shouldBeApplied,
            int startingPointAvailableMoves,
            int endingPointAvailableMoves,
            bool[] startingAvailable,
            bool[] endingAvaialble,
            bool useTopStartingBlock)
        {
            GameBoard gameBoard = new GameBoard();
            gameBoard.GameBlocks = new IGameBlock[4,1];

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 0);
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 0) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 0) as GameBlockNormal;

            gameBlockNormal1.SetAvailability(startingAvailable[0]);
            gameBlockNormal2.SetAvailability(startingAvailable[1]);

            GameBlockNormal gameBlockNormal = useTopStartingBlock ? gameBlockNormal1 : gameBlockNormal2;

            BlockMovement movement;
            bool applied = gameBlockNormal.ApplyReverseMove(
                null, direction, startingPointAvailableMoves, out movement, null, null, new Queue<IGameBlock>());

            Assert.AreEqual(shouldBeApplied, applied);
            Assert.AreEqual(endingAvaialble[0], gameBlockNormal1.IsAvailable);
            Assert.AreEqual(endingAvaialble[1], gameBlockNormal2.IsAvailable);

            if (shouldBeApplied)
            {
                Assert.AreEqual(endingPointAvailableMoves, movement.SourceBlock.AvailableMoves);
            }
        }

        [Test]
        public void ReverseMultipleMovesGiveCorrectNumberOfMoves()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            GameBlockPlayer player1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockMultipleMoves gameBlockMultiple =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2, numberOfMultipleMoves: 2) as GameBlockMultipleMoves;
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 3, MovementDirection.Left);

            gameBlockNormal1.SetAvailability(false);
            gameBlockMultiple.NumberOfMovesApplied = 2;

            BlockMovement move;
            bool applyReverseMove = gameBlockMultiple.ApplyReverseMove(
                null, MovementDirection.Right, 3, out move, null, null, new Queue<IGameBlock>());

            Assert.IsTrue(applyReverseMove);
        }

        #endregion
    }
}