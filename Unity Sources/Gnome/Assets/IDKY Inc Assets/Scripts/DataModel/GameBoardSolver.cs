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
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class GameBoardSolver
    {
        #region Static Fields

        private static HashSet<string> AlreadyTestedBoards;

        private static int Failures;

        private static object MyLock = new object();

        private static int Successes;

        #endregion

        #region Public Methods and Operators

        public static void Solve(GameBoard gameBoard, int milliSecondsTimeout, out int failures, out int successes)
        {
            Failures = 0;
            Successes = 0;
            AlreadyTestedBoards = new HashSet<string>();
            List<Thread> threads = new List<Thread>();
            
            foreach (IGameBlockParent gameBlock in gameBoard.GameBlocks.OfType<IGameBlockParent>())
            {
                GameBoard gameBoardClone = (GameBoard)gameBoard.Clone();
                IGameBlockParent gameBlockClone = gameBoardClone.GameBlocks[gameBlock.IndexRow, gameBlock.IndexColumn] as IGameBlockParent;

                // Parallel process the solutions
                ThreadStart threadStart = () => SolveMovements(gameBoardClone, gameBlockClone, null);

                Thread threadFunction = new Thread(threadStart);
                threads.Add(threadFunction);
            }

            List<Thread> runningThreads = new List<Thread>();

            while (threads.Count > 0)
            {
                // Limit running threads to 8
                for (int i = 0; i < 8 - runningThreads.Count && i < threads.Count; i++)
                {
                    threads[i].Start();
                    runningThreads.Add(threads[i]);
                    threads.Remove(threads[i]);
                }

                List<Thread> finishedThreads = new List<Thread>();

                // Remove any threads that finished
                foreach (Thread runningThread in runningThreads)
                {
                    bool joined = runningThread.Join(1000);

                    if (joined)
                    {
                        finishedThreads.Add(runningThread);
                    }
                }

                foreach (Thread finishedThread in finishedThreads)
                {
                    runningThreads.Remove(finishedThread);
                }
            }

            // Wait for the threads to finish
            foreach (Thread thread in runningThreads)
            {
                thread.Join(milliSecondsTimeout);

                // Took too long so abort it
                if (thread.IsAlive)
                {
                    thread.Abort();
                }
            }

            failures = Failures;
            successes = Successes;

            AlreadyTestedBoards = new HashSet<string>();

            GC.Collect();
        }

        #endregion

        #region Methods

        private static List<MovementDirection> GetDirections()
        {
            List<MovementDirection> directions = new List<MovementDirection>
                                                     {
                                                         MovementDirection.Down,
                                                         MovementDirection.Up,
                                                         MovementDirection.Right,
                                                         MovementDirection.Left,
                                                     };

            return directions;
        }

        private static void SolveMovements(GameBoard gameBoard, IGameBlockParent sourceBlock, BlockMovement head)
        {
            string key = gameBoard.GetBoardStringFullHashed() + "_" + sourceBlock.IndexRow + "_" + sourceBlock.IndexColumn;

            lock (AlreadyTestedBoards)
            {
                // Already tested this, so don't need to try it again
                if (AlreadyTestedBoards.Contains(key))
                {
                    return;
                }

                AlreadyTestedBoards.Add(key);
            }

            List<MovementDirection> movementDirections = GetDirections();

            bool allFailures = true;

            foreach (MovementDirection direction in movementDirections)
            {
                BlockMovement currentMove = new BlockMovement(sourceBlock, direction);
                BlockMovement headMove;
                if (head != null)
                {
                    headMove = head.CloneFromGameBoard(gameBoard, head);
                    headMove.Enqueue(currentMove);
                }
                else
                {
                    currentMove.Head = currentMove;
                    headMove = currentMove;
                }

                if (sourceBlock.ApplyMove(null, direction, headMove))
                {
                    allFailures = false;

                    List<IGameBlockParent> leftoverBlocks =
                        gameBoard.GameBlocks.OfType<IGameBlockParent>().Where(g => g.AvailableMoves > 0).ToList();

                    if (leftoverBlocks.Count == 0)
                    {
                        if (gameBoard.HasOrphan())
                        {
                            // failed
                            lock (MyLock)
                            {
                                Failures++;
                            }
                        }
                        else
                        {
                            // succeeded
                            lock (MyLock)
                            {
                                Successes++;
                            }
                        }
                    }
                    else
                    {
                        // Continue to the next move
                        foreach (IGameBlockParent gameBlockParent in leftoverBlocks)
                        {
                            SolveMovements(gameBoard, gameBlockParent, head);
                        }
                    }

                    // Undo move
                    headMove.UndoMove();
                }
            }

            if (allFailures)
            {
                // Any direction, doesn't matter
                BlockMovement currentMove = new BlockMovement(sourceBlock, MovementDirection.Right);
                BlockMovement headMove;
                if (head != null)
                {
                    headMove = head.CloneFromGameBoard(gameBoard, head);
                    headMove.Enqueue(currentMove);
                }
                else
                {
                    currentMove.Head = currentMove;
                    headMove = currentMove;
                }

                lock (MyLock)
                {
                    Failures++;
                }
            }
        }

        #endregion
    }
}