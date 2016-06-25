// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class SolverTests
    {
        #region Public Methods and Operators

        [Test]
        public void SolveDoubleBlock()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             *   0 1 2  
             * 0 P P P  
             * 1 P G P   
             * 2 P G P  
             * 3 P P P  
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(4, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 2);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 3, 20, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);
            Assert.LessOrEqual(solutions.Count, 20);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveLargePuzzle()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             *   0 1 2 3 4 5 
             * 0 N P N N P N 
             * 1 P G G G G P  
             * 2 N G N N G N 
             * 3 N G G G G P  
             * 4 N P G G G N 
             * 5 N N G N G N  
             * 6 N N P N P N 
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(7, 6);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 5);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 5);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 6, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 6, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 5, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 5, 4);

            GameBoardReverseSolver.SetMaxCombinationsSolve(1);
            int fails = 0;
            GameBoardReverseSolver.SolutionFailedEvent += (sender, args) => fails++;

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 5, 10, 0, 15000, out aborted);

            Console.WriteLine("Failures: {0}\n", fails);

            Assert.GreaterOrEqual(solutions.Count, 1);
            Assert.LessOrEqual(solutions.Count, 10);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);

            GameBoardReverseSolver.SetMaxCombinationsSolve(0);
        }

        [Test]
        public void SolveLargePuzzle2()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             *   0 1 2 3 4 5 
             * 0 P P P N P P 
             * 1 G G G P G G  
             * 2 G G G P G G 
             * 3 G G G P G G  
             * 4 G G G P G G 
             * 5 G G G P G G  
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(5, 6);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 5);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 5);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 5);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 5);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 5);

            //GameBoardReverseSolver.SetTestCanReverseSolve(true);
            GameBoardReverseSolver.SetMaxCombinationsSolve(1);
            int fails = 0;
            GameBoardReverseSolver.SolutionFailedEvent += (sender, args) => fails++;

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 5, 1, 0, 60000, out aborted);

            Console.WriteLine("Failures: {0}\n", fails);

            Assert.GreaterOrEqual(solutions.Count, 1);
            Assert.LessOrEqual(solutions.Count, 10);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);

            //GameBoardReverseSolver.SetTestCanReverseSolve(false);
            GameBoardReverseSolver.SetMaxCombinationsSolve(0);
        }

        [Test]
        public void SolveLooped()
        {
            GameBoard gameBoard = new GameBoard(2, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Null, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 1, MovementDirection.Right);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 3, MovementDirection.Down);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 1, MovementDirection.Up);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 3, MovementDirection.Left);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 3, 10, 0, 30000, out aborted);

            // There should only be 1 solution
            Assert.AreEqual(1, solutions.Count);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }
        }

        [Test]
        public void SolveMiniPuzzle()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             *   0 1 2
             * 0 N P N 
             * 1 P G G  
             * 2 N G N  
             * 3 N G N  
             * 4 N P N   
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(5, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 1);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 1);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 5, 10, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);
            Assert.LessOrEqual(solutions.Count, 10);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveMiniWithMultiplePuzzle()
        {
            GameBoard gameBoard = new GameBoard(3, 3);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 2);
            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 3, 10, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveMiniWithMultiplePuzzleWithLoop()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 0, 3, MovementDirection.Left);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 3, 10, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveSingleBlock()
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

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 1, 4, 0, 60000, out aborted);

            Assert.AreEqual(2, solutions.Count);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveSmallPuzzle()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             *   0 1 2 3 
             * 0 N P N N 
             * 1 P G G P   
             * 2 N G N N  
             * 3 N G G P  
             * 4 N P G G  
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(5, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 1);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 3);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 5, 10, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);
            Assert.LessOrEqual(solutions.Count, 10);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveSmallWithChangeDirectionPuzzle()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             *   0 1 2 3 4
             * 0 N P N P N
             * 1 P G D G P 
             * 2 N G G G P 
             * 3 N G G P N
             * 4 N P G G N
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(5, 5);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 1);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 3);

            GameBlockChangeDirection changeDirection =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 2) as GameBlockChangeDirection;
            changeDirection.ForceDirection = MovementDirection.Down;
            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 5, 10, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);
            Assert.LessOrEqual(solutions.Count, 10);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveSmallWithExtraPuzzle()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             * X = extra block
             *   0 1 2 3 4
             * 0 N P N P N
             * 1 P G D G P 
             * 2 N G X G P 
             * 3 N G G P N
             * 4 N P G G N
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(5, 5);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 1);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 2, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 3);

            GameBlockChangeDirection changeDirection =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 2) as GameBlockChangeDirection;
            changeDirection.ForceDirection = MovementDirection.Down;
            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 5, 10, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                bool playSolution = solution.PlaySolution();

                if (!playSolution)
                {
                    int x = 0;
                }
                Assert.IsTrue(playSolution);
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveSmallWithMultiplePuzzle()
        {
            /***************************
             * N = null
             * P = player
             * G = normal game block
             * X = extra block
             * M = multiple move block
             *   0 1 2 3 4
             * 0 N P N P N
             * 1 P G D G P 
             * 2 N M X G P 
             * 3 N G G P N
             * 4 N P G G N
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(5, 5);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 4);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 4, 1);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 2, 1, numberOfMultipleMoves: 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.ExtraMove, 2, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 2, 3);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 3, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 4, 3);

            GameBlockChangeDirection changeDirection =
                gameBoard.CreateGameBlock(GameBoard.GameBlockType.ChangeDirection, 1, 2) as GameBlockChangeDirection;
            changeDirection.ForceDirection = MovementDirection.Down;
            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 5, 10, 0, 60000, out aborted);

            Assert.GreaterOrEqual(solutions.Count, 1);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveTrivialBoard()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 1, 1, 0, 60000, out aborted);

            Assert.AreEqual(1, solutions.Count);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolveTrivialBoardWithMultipleMoves()
        {
            GameBoard gameBoard = new GameBoard(1, 4);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 0, 2);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 3);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 2, 1, 0, 60000, out aborted);

            Assert.AreEqual(1, solutions.Count);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        [Repeat(5)]
        public void SolveTwoMultiBlock()
        {
            /***************************
             * N = null
             * P = player
             * M = multiple game block
             *   0 1  
             * 0 P P  
             * 1 P M   
             * 2 P M  
             * 3 P P  
             * 
             ***************************/
            GameBoard gameBoard = new GameBoard(4, 2);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 2, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 0);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 3, 1);

            gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 1, 1);
            gameBoard.CreateGameBlock(GameBoard.GameBlockType.MultipleMoves, 2, 1);

            bool aborted;
            List<GameBoard> solutions = GameBoardReverseSolver.ReverseSolve(gameBoard, 2, 20, 0, 60000, out aborted);

            Assert.AreEqual(8, solutions.Count);

            foreach (GameBoard solution in solutions)
            {
                Console.WriteLine("Game board: Difficulty={0} \n{1}\n", solution.GetDifficulty(), solution.GetBoardString());
                Console.WriteLine("Solution: \n{0}\n", solution.GetSolutionString());

                Assert.IsTrue(solution.IsUntouchedPuzzle());
                Assert.IsTrue(solution.PlaySolution());
            }

            Console.WriteLine("Number of Solutions: {0}", solutions.Count);
        }

        [Test]
        public void SolverDoesNotChangeGameBoard()
        {
            GameBoard gameBoard = new GameBoard(2, 5);

            GameBlockPlayer gameBlockPlayer1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;
            GameBlockNormal gameBlockNormal3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 3) as GameBlockNormal;
            GameBlockPlayer gameBlockPlayer2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 4) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal4 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2) as GameBlockNormal;
            GameBlockPlayer gameBlockPlayer3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3) as GameBlockPlayer;

            gameBlockNormal1.SetAvailability(true);
            gameBlockNormal2.SetAvailability(true);
            gameBlockNormal3.SetAvailability(true);
            gameBlockNormal4.SetAvailability(true);
            gameBlockPlayer1.AvailableMoves = 1;
            gameBlockPlayer2.AvailableMoves = 2;
            gameBlockPlayer3.AvailableMoves = 1;

            string originalBoardString = gameBoard.GetBoardStringFull();

            int failures;
            int successes;
            GameBoardSolver.Solve(gameBoard, 30000, out failures, out successes);

            string boardStringAfterSolving = gameBoard.GetBoardStringFull();
            Assert.AreEqual(originalBoardString, boardStringAfterSolving);
        }

        [Test]
        public void TestFindsSolution()
        {
            GameBoard gameBoard = new GameBoard(2, 5);

            GameBlockPlayer gameBlockPlayer1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 0) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal1 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 1) as GameBlockNormal;
            GameBlockNormal gameBlockNormal2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 2) as GameBlockNormal;
            GameBlockNormal gameBlockNormal3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 0, 3) as GameBlockNormal;
            GameBlockPlayer gameBlockPlayer2 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 0, 4) as GameBlockPlayer;
            GameBlockNormal gameBlockNormal4 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Normal, 1, 2) as GameBlockNormal;
            GameBlockPlayer gameBlockPlayer3 = gameBoard.CreateGameBlock(GameBoard.GameBlockType.Player, 1, 3) as GameBlockPlayer;

            gameBlockNormal1.SetAvailability(true);
            gameBlockNormal2.SetAvailability(true);
            gameBlockNormal3.SetAvailability(true);
            gameBlockNormal4.SetAvailability(true);
            gameBlockPlayer1.AvailableMoves = 1;
            gameBlockPlayer2.AvailableMoves = 2;
            gameBlockPlayer3.AvailableMoves = 1;

            int failures;
            int successes;
            GameBoardSolver.Solve(gameBoard, 30000, out failures, out successes);

            Assert.AreEqual(2, failures);
            Assert.AreEqual(3, successes);
        }

        #endregion
    }
}