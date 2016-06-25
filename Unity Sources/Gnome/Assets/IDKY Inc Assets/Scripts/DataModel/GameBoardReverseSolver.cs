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
    public class GameBoardReverseSolver
    {
        #region Static Fields

        private static HashSet<string> AlreadyTestedBoards;

        private static bool AnalyzeImmediately;

        private static bool CanTestReverseSolve;

        private static int MaxCombinations;

        private static Random RandomGenerator;

        private static bool RequestAbort;

        #endregion

        #region Constructors and Destructors

        static GameBoardReverseSolver()
        {
            RandomGenerator = new Random();
        }

        #endregion

        #region Public Events

        public static event EventHandler AnalyzedSolutionEvent;

        public static event EventHandler SolutionFailedEvent;

        public static event EventHandler SolutionFoundEvent;

        public static event EventHandler SolveSolutionEndedEvent;

        public static event EventHandler SolveSolutionStartedEvent;

        #endregion

        #region Public Methods and Operators

        public static bool GetRequestAbort()
        {
            return RequestAbort;
        }

        public static void RequestAbortSolver()
        {
            RequestAbort = true;
        }

        /// <summary>
        /// Reverses the solve.
        /// </summary>
        /// <param name="gameBoard">The game board.</param>
        /// <param name="maxCountAtPlayer">The max count at player.</param>
        /// <param name="maxNumberOfSolutions">The max number of solutions.</param>
        /// <param name="variability">The variability.</param>
        /// <param name="milliSecondsTimeout">The milli seconds timeout.</param>
        /// <param name="aborted">if set to <c>true</c> [aborted].</param>
        /// <returns></returns>
        public static List<GameBoard> ReverseSolve(
            GameBoard gameBoard, int maxCountAtPlayer, int maxNumberOfSolutions, int variability, int milliSecondsTimeout, out bool aborted)
        {
            RequestAbort = false;
            AlreadyTestedBoards = new HashSet<string>();
            Dictionary<string, GameBoard> solutionGameBoards = new Dictionary<string, GameBoard>();
            List<Thread> threads = new List<Thread>();
            List<IGameBlockDestination> destinations = new List<IGameBlockDestination>();

            foreach (IGameBlock gameBlock in gameBoard.GameBlocks)
            {
                IGameBlockDestination nextDestination = gameBlock as IGameBlockDestination;

                if (nextDestination != null)
                {
                    destinations.Add(gameBlock as IGameBlockDestination);
                }
            }

            Shuffle(destinations, destinations.Count);

            foreach (IGameBlockDestination gameBlock in destinations)
            {
                GameBoard gameBoardClone = (GameBoard)gameBoard.Clone();
                IGameBlockDestination gameBlockClone =
                    gameBoardClone.GameBlocks[gameBlock.IndexRow, gameBlock.IndexColumn] as IGameBlockDestination;

                // Parallel process the solutions
                ThreadStart threadStart = () =>
                                              {
                                                  int stepsBack = 0;

                                                  ReverseSolveMovements(
                                                      gameBoardClone,
                                                      gameBlockClone,
                                                      null,
                                                      maxCountAtPlayer,
                                                      maxNumberOfSolutions,
                                                      variability,
                                                      ref stepsBack,
                                                      null,
                                                      solutionGameBoards);
                                              };

                Thread threadFunction = new Thread(threadStart);
                threads.Add(threadFunction);
                threadFunction.Start();
            }

            // Wait for the threads to finish
            foreach (Thread thread in threads)
            {
                thread.Join(milliSecondsTimeout);

                // Took too long so abort it
                if (thread.IsAlive)
                {
                    thread.Abort();
                }
            }

            NotifySolveSolutionStartedEvent();

            // Clear it to take back memory
            AlreadyTestedBoards = new HashSet<string>();

            GC.Collect();

            if (AnalyzeImmediately)
            {
                foreach (KeyValuePair<string, GameBoard> solutionGameBoard in solutionGameBoards)
                {
                    int fails;
                    int successes;

                    GameBoardSolver.Solve(solutionGameBoard.Value, milliSecondsTimeout * 4, out fails, out successes);
                    solutionGameBoard.Value.Failures = fails;
                    solutionGameBoard.Value.Successes = successes;

                    NotifyAnalyzedSolutionEvent();
                }
            }

            NotifySolveSolutionEndedEvent();

            aborted = RequestAbort;
            RequestAbort = false;

            return solutionGameBoards.Values.ToList();
        }

        public static void SetAnalyzeImmediately(bool analyze)
        {
            AnalyzeImmediately = analyze;
        }

        public static void SetMaxCombinationsSolve(int combo)
        {
            MaxCombinations = combo;
        }

        public static void SetTestCanReverseSolve(bool canTest)
        {
            CanTestReverseSolve = canTest;
        }

        #endregion

        #region Methods

        private static List<MovementDirection> GetRandomDirections()
        {
            List<MovementDirection> directions = new List<MovementDirection>
                                                     {
                                                         MovementDirection.Down,
                                                         MovementDirection.Up,
                                                         MovementDirection.Right,
                                                         MovementDirection.Left,
                                                     };

            // Shuffle it
            for (int i = 0; i < 5; i++)
            {
                int next = RandomGenerator.Next(0, 3);
                MovementDirection movementDirection = directions[next];
                directions.Remove(movementDirection);
                directions.Add(movementDirection);
            }

            return directions;
        }

        private static void NotifyAnalyzedSolutionEvent()
        {
            if (AnalyzedSolutionEvent != null)
            {
                AnalyzedSolutionEvent(null, new EventArgs());
            }
        }

        private static void NotifySolutionFailedEvent()
        {
            if (SolutionFailedEvent != null)
            {
                SolutionFailedEvent(null, new EventArgs());
            }
        }

        private static void NotifySolutionFoundEvent()
        {
            if (SolutionFoundEvent != null)
            {
                SolutionFoundEvent(null, new EventArgs());
            }
        }

        private static void NotifySolveSolutionEndedEvent()
        {
            if (SolveSolutionEndedEvent != null)
            {
                SolveSolutionEndedEvent(null, new EventArgs());
            }
        }

        private static void NotifySolveSolutionStartedEvent()
        {
            if (SolveSolutionStartedEvent != null)
            {
                SolveSolutionStartedEvent(null, new EventArgs());
            }
        }

        private static void ReverseSolveDirection(
            GameBoard gameBoard,
            IGameBlockDestination gameBlock,
            IGameBlockDestination destinationBlock,
            MovementDirection direction,
            int maxCountAtPlayer,
            int maxNumberOfSolutions,
            int variability,
            ref int stepsBack,
            BlockMovement nextMove,
            Dictionary<string, GameBoard> solutionGameBoards)
        {
            // Test the board to see if a reverse solve is even possible.  If it's not, return early so it doesn't
            // waste time trying to solve something that can't be solved.
            if (CanTestReverseSolve && !gameBoard.TestCanReverseSolve())
            {
                return;
            }

            // So it doesn't always choose the max
            List<int> counts = Enumerable.Range(1, maxCountAtPlayer).ToList();

            for (int i = 0; i < counts.Count; i++)
            {
                int next = RandomGenerator.Next(0, counts.Count - 1);
                int index = counts[next];
                counts.Remove(index);
                counts.Add(index);
            }

            for (int i = 0; i < counts.Count && solutionGameBoards.Count < maxNumberOfSolutions && !RequestAbort; i++)
            {
                BlockMovement temp;

                if (gameBlock.ApplyReverseMove(destinationBlock, direction, counts[i], out temp, null, null, new Queue<IGameBlock>()))
                {
                    bool allMovesUsed = true;
                    int originalAvailableMoves = temp.SourceBlock.AvailableMoves;

                    List<List<IGameBlockDestination>> removedCombinations;

                    if (nextMove != null)
                    {
                        temp.NextMove = nextMove;
                        nextMove.PreviousMove = temp;
                    }

                    // Got more moves that we wanted, so take out a few randomly
                    int max = counts[i];

                    if (temp.SourceBlock.PreferredMaxMoves != 0 && temp.SourceBlock.AvailableMoves > temp.SourceBlock.PreferredMaxMoves)
                    {
                        max = Math.Min(max, temp.SourceBlock.PreferredMaxMoves);
                    }

                    // More moves than can be used
                    if (temp.SourceBlock.AvailableMoves > max)
                    {
                        int numberOfMovedNeedRemoving = temp.SourceBlock.AvailableMoves - max;

                        int numberAvailableForRemoving = temp.IntermediateBlocks.Count(ib => ib.CanBeRemovedIntermediate());

                        // Not possible because it needs to remove more items than it could
                        if (numberAvailableForRemoving < numberOfMovedNeedRemoving)
                        {
                            BlockMovement move = new BlockMovement(temp.SourceBlock, temp.Direction);
                            temp.SourceBlock.ApplyMove(null, temp.Direction, move);

                            continue;
                        }

                        allMovesUsed = false;

                        List<IGameBlockDestination> removeableIntermediates =
                            temp.IntermediateBlocks.Where(ib => ib.CanBeRemovedIntermediate()).ToList();

                        removedCombinations = new List<List<IGameBlockDestination>>();
                        IEnumerable<IEnumerable<IGameBlockDestination>> combos =
                            removeableIntermediates.Combinations(numberOfMovedNeedRemoving).ToList();

                        int count = 0;
                        foreach (IEnumerable<IGameBlockDestination> gameBlockDestinations in combos)
                        {
                            removedCombinations.Add(gameBlockDestinations.ToList());

                            if (MaxCombinations > 0 && ++count >= MaxCombinations)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        removedCombinations = new List<List<IGameBlockDestination>> { new List<IGameBlockDestination>() };
                    }

                    List<IGameBlockDestination> originalIntermediateBlocks = new List<IGameBlockDestination>();

                    foreach (IGameBlockDestination gameBlockDestination in temp.IntermediateBlocks)
                    {
                        originalIntermediateBlocks.Add(gameBlockDestination.Clone() as IGameBlockDestination);
                    }

                    foreach (List<IGameBlockDestination> removedCombination in removedCombinations)
                    {
                        List<IGameBlockDestination> unusedBlocks = new List<IGameBlockDestination>();
                        List<List<IGameBlockDestination>> intermediateBlockSets = new List<List<IGameBlockDestination>>();

                        foreach (IGameBlockDestination gameBlockDestination in removedCombination)
                        {
                            temp.IntermediateBlocks.Remove(gameBlockDestination);

                            temp.SourceBlock.AvailableMoves--;
                            gameBlockDestination.SetAvailability(false);
                            unusedBlocks.Add(gameBlockDestination);
                        }

                        foreach (IGameBlockDestination unusedBlock in unusedBlocks)
                        {
                            for (int j = 0; j < temp.IntermediateBlocks.Count; j++)
                            {
                                List<IGameBlockDestination> sets = new List<IGameBlockDestination>(temp.IntermediateBlocks);

                                sets.RemoveAt(j);
                                sets.Add(unusedBlock);
                                intermediateBlockSets.Add(sets);
                            }
                        }

                        do
                        {
                            if (gameBoard.IsUntouchedPuzzle())
                            {
                                lock (solutionGameBoards)
                                {
                                    // HACK: Test the solution in case it comes up with an invalid solution
                                    GameBoard testSolution = (GameBoard)gameBoard.Clone();
                                    testSolution.SolutionStartMove = temp.CloneFromGameBoard(testSolution, null);
                                    testSolution.NullZeroPlayers();

                                    string key = testSolution.ToString();

                                    if (!solutionGameBoards.ContainsKey(key))
                                    {
                                        if (testSolution.PlaySolution())
                                        {
                                            GameBoard solution = (GameBoard)gameBoard.Clone();
                                            solution.SolutionStartMove = temp.CloneFromGameBoard(solution, null);
                                            solution.NullZeroPlayers();
                                            solutionGameBoards.Add(key, solution);
                                            NotifySolutionFoundEvent();
                                        }
                                    }
                                }
                            }
                            else if (!gameBoard.HasOrphan())
                            {
                                List<IGameBlockDestination> destinations = new List<IGameBlockDestination>();

                                foreach (IGameBlock nextBlock in gameBoard.GameBlocks)
                                {
                                    IGameBlockDestination nextDestination = nextBlock as IGameBlockDestination;

                                    if (nextDestination != null && !nextDestination.IsFullyAvailable)
                                    {
                                        for (int j = 0; j < nextDestination.NumberUsed; j++)
                                        {
                                            destinations.Add(nextDestination);
                                        }
                                    }
                                }

                                Shuffle(destinations, destinations.Count);

                                foreach (IGameBlockDestination nextBlock in destinations)
                                {
                                    ReverseSolveMovements(
                                        gameBoard,
                                        nextBlock,
                                        null,
                                        maxCountAtPlayer,
                                        maxNumberOfSolutions,
                                        variability,
                                        ref stepsBack,
                                        temp,
                                        solutionGameBoards);

                                    if (solutionGameBoards.Count >= maxNumberOfSolutions)
                                    {
                                        allMovesUsed = true;
                                        break;
                                    }

                                    if (RequestAbort)
                                    {
                                        break;
                                    }
                                }
                            }

                            // Not all the moves were used, but no solution was found, so let's use one of the ones we took out
                            if (!allMovesUsed)
                            {
                                if (intermediateBlockSets.Count == 0)
                                {
                                    allMovesUsed = true;
                                }
                                else
                                {
                                    // Replace the block with one of the unused ones
                                    temp.IntermediateBlocks.ForEach(g => g.SetAvailability(true));

                                    List<IGameBlockDestination> intermediateBlockSet = intermediateBlockSets[0];
                                    intermediateBlockSets.RemoveAt(0);
                                    intermediateBlockSet.ForEach(g => g.SetAvailability(false));

                                    temp.IntermediateBlocks = intermediateBlockSet;
                                }
                            }
                            else
                            {
                                allMovesUsed = true;
                                NotifySolutionFailedEvent();
                            }
                        }
                        while (!allMovesUsed && !RequestAbort);

                        if (removedCombination.Count > 0)
                        {
                            allMovesUsed = false;
                            temp.IntermediateBlocks.Clear();

                            foreach (IGameBlockDestination gameBlockDestination in originalIntermediateBlocks)
                            {
                                IGameBlockDestination blockDestination =
                                    gameBoard.GameBlocks[gameBlockDestination.IndexRow, gameBlockDestination.IndexColumn] as
                                    IGameBlockDestination;
                                blockDestination.Copy(gameBlockDestination);
                                temp.IntermediateBlocks.Add(blockDestination);
                            }

                            temp.SourceBlock.AvailableMoves = originalAvailableMoves;
                        }
                    }

                    BlockMovement blockMove = new BlockMovement(temp.SourceBlock, temp.Direction);
                    temp.SourceBlock.ApplyMove(null, temp.Direction, blockMove);
                }
            }
        }

        private static void ReverseSolveMovements(
            GameBoard gameBoard,
            IGameBlockDestination gameBlock,
            IGameBlockDestination destinationBlock,
            int maxCountAtPlayer,
            int maxNumberOfSolutions,
            int variability,
            ref int stepsBack,
            BlockMovement nextMove,
            Dictionary<string, GameBoard> solutionGameBoards)
        {
            List<MovementDirection> movementDirections = GetRandomDirections();

            if (!gameBlock.IsFullyAvailable && !RequestAbort)
            {
                int oldNumSolutions = solutionGameBoards.Count;
                string key = gameBoard.GetBoardStringFull() + "_" + gameBlock.IndexRow + "_" + gameBlock.IndexColumn;

                lock (AlreadyTestedBoards)
                {
                    // Already tested this, so don't need to try it again
                    if (AlreadyTestedBoards.Contains(key))
                    {
                        return;
                    }

                    AlreadyTestedBoards.Add(key);
                }

                foreach (MovementDirection direction in movementDirections)
                {
                    ReverseSolveDirection(
                        gameBoard,
                        gameBlock,
                        destinationBlock,
                        direction,
                        maxCountAtPlayer,
                        maxNumberOfSolutions,
                        variability,
                        ref stepsBack,
                        nextMove,
                        solutionGameBoards);

                    // New solutions found so step back up the stack to get more variability in the solutions
                    if (oldNumSolutions != solutionGameBoards.Count)
                    {
                        if (variability > stepsBack)
                        {
                            stepsBack++;
                            break;
                        }
                        else
                        {
                            stepsBack = 0;
                        }
                    }

                    if (RequestAbort)
                    {
                        break;
                    }
                }
            }
        }

        private static void Shuffle(List<IGameBlockDestination> numbers, int timesToShuffle)
        {
            int maxIndex = numbers.Count - 1;

            for (int i = 0; i < timesToShuffle; i++)
            {
                int index1 = RandomGenerator.Next(0, maxIndex);
                int index2 = RandomGenerator.Next(0, maxIndex);

                IGameBlockDestination temp = numbers[index1];
                numbers[index1] = numbers[index2];
                numbers[index2] = temp;
            }
        }

        #endregion
    }
}