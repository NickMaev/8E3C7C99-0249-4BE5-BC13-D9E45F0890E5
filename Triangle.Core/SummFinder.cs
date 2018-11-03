using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Triangle.Core
{
    /// <summary>
    /// Object-oriented representation of the alghorithm.
    /// </summary>
    public class SummFinder
    {
        protected internal bool IsInitialized { get; set; }
        
        [ExcludeFromCodeCoverage]
        protected internal ISummFinderLogger Logger { get; set; }

        /// <summary>
        /// Successful paths count.
        /// </summary>
        protected internal int SuccessfulPathCount { get; set; }

        protected internal Stack<SavedLocation> PathsWithAvailableChoice { get; set; }

        /// <summary>
        /// List of bad paths to prevent pass the same path repeatedly.
        /// </summary>
        protected internal List<SavedLocation> BadPaths { get; set; }
        
        /// <summary>
        /// Array of input values.
        /// </summary>
        protected internal int[][] Array = null;
        
        [ExcludeFromCodeCoverage]
        public SummFinder(ISummFinderLogger logger = null)
        {
            Logger = logger;
        }
        
        [ExcludeFromCodeCoverage]
        protected internal void ResetState()
        {
            SuccessfulPathCount = 0;
            PathsWithAvailableChoice = new Stack<SavedLocation>();
            BadPaths = new List<SavedLocation>();
        }

        /// <summary>
        /// Removes all empty rows and prepares an array for the next steps.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        protected internal int[][] PrepareArray(int[][] array)
        {
            Logger?.WriteLine("Preparing an array...");
            Logger?.WriteLine("Array before preparing:");
            LogArray(array);
            Logger?.WriteLine();

            // Remove empty row from array.
            array = array.Where(x => x.Any()).ToArray();
            
            Logger?.WriteLine("Array after preparing:");
            LogArray(array);
            Logger?.WriteLine();

            return array;
        }

        [ExcludeFromCodeCoverage]
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        protected internal static void ThrowIfInvalidArray(int[][] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (!array.Any())
                throw new InvalidDataException("Array doesn't contain any data.");
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [ExcludeFromCodeCoverage]
        protected internal static void ThrowIfFileInfoNullOrNotExists(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));
            if (!fileInfo.Exists)
                throw new FileNotFoundException(fileInfo.FullName);
        }

        /// <summary>
        /// Load array which represents the triangle.
        /// </summary>
        /// <param name="array">Array that represents the triangle.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public void LoadFromArray(int[][] array)
        {
            ThrowIfInvalidArray(array);
            
            array = PrepareArray(array);

            LogArray(array);

            Array = array;

            IsInitialized = true;
        }

        /// <summary>
        /// Load data from file.
        /// </summary>
        /// <param name="fileInfo">File info.</param>
        /// <returns>Parsed array of integers.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public void LoadFromFile(FileInfo fileInfo)
        {
            ThrowIfFileInfoNullOrNotExists(fileInfo);

            var lines = File.ReadAllLines(fileInfo.FullName);

            var array = new int[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                var cols = lines[i].Split(' ');
                array[i] = new int[cols.Length];
                for (int j = 0; j < cols.Length; j++)
                {
                    array[i][j] = int.Parse(cols[j]);
                }
            }

            ThrowIfInvalidArray(array);

            array = PrepareArray(array);
            
            Array = array;

            IsInitialized = true;
        }

        /// <summary>
        /// Finds the path that provides the maximum
        /// possible sum of the numbers and gets this summ.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual int GetMaxSumm(out int[] maxSummPath)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    $"Summ finder hasn't been initialized. Please, load data before execution the '{nameof(SummFinder.GetMaxSumm)}' method.");
            }
            
            ResetState();
            
            var path = new List<int>(); // List of traveled path.

            int maxSumm = 0; // Maximal summ of the all paths.

            var fullTraveledPathWithMaxSumm = new List<int>();
            
            bool needEven = false, doBackwardStep = false;

            int binaryTreeStartColIndex = 0, 
                binaryTreeColIndex = 0, // Internal tree index.
                binaryTreeThreshold = 1; // Threshold of the internal three index.

            int rowIndex = 0, colIndex = 0;

            LogStartPath(SuccessfulPathCount);

            SavedLocation backwardStep = null;
            do
            {
                if (doBackwardStep)
                {
                    if (PathsWithAvailableChoice.Count == 0)
                    {
                        if (PathsWithAvailableChoice.Count == 0 &&
                            SuccessfulPathCount == 1 && 
                            path.Count < Array.Length)
                        {
                            throw new InvalidDataException("Input triangle has invalid numbers. It must have path odd->even->odd->even->... till the end.");
                        }
                        break;
                    }

                    // Restore previous location.
                    backwardStep = PathsWithAvailableChoice.Pop();

                    Logger?.WriteLine(
                        backgroundColor: ConsoleColor.Red, 
                        text: $"<- Get back to the row {backwardStep.RowIndex} and column {backwardStep.ColIndex}."
                        );

                    doBackwardStep = false;

                    rowIndex = backwardStep.RowIndex; // Start from the next row index.
                    binaryTreeStartColIndex = backwardStep.ColIndex + 1; // Start from the next column index.
                    binaryTreeColIndex = 1; // Start from the next internal tree index.

                    // Restore previous path.
                    path.Clear();
                    path.AddRange(backwardStep.Path);
                }
                else
                {
                    binaryTreeColIndex = 0;
                }

                colIndex = binaryTreeStartColIndex;

                needEven = !IsEven(rowIndex); // If this row is odd, we need even value.

                var row = Array[rowIndex];

                Logger?.Write($"Process row {rowIndex} ", ConsoleColor.Blue);
                
                for (; colIndex < row.Length; colIndex++)
                {
                    if (binaryTreeColIndex > binaryTreeThreshold)
                    {           
                        Logger?.Write(
                            "Binary tree reached.",
                            backgroundColor: ConsoleColor.Gray,
                            fontColor: ConsoleColor.Black);

                        if (backwardStep != null)
                        {
                            Logger?.Write(
                                $" Add bad path at row {backwardStep.RowIndex} and column {backwardStep.ColIndex}.",
                                backgroundColor: ConsoleColor.Gray,
                                fontColor: ConsoleColor.Black);

                            BadPaths.Add(backwardStep);
                        }                    
                        Logger?.WriteLine();

                        doBackwardStep = true;
                        break;
                    }

                    int val = row[colIndex];
                    
                    var isValueEven = IsEven(val);
                    if(rowIndex == 0 && colIndex == 0 && isValueEven)
                        throw new InvalidDataException("First value of the triangle should be even.");

                    if (needEven)
                    {
                        // Need even value.

                        if (isValueEven)
                        {
                            path.Add(val);

                            Logger?.Write($"even: {val}");

                            binaryTreeStartColIndex = colIndex;
                            var hasAvailableChoice = false;
                            if (binaryTreeColIndex < binaryTreeThreshold &&
                                colIndex < row.Length - 1 &&
                                rowIndex < Array.Length)
                            {
                                var nextValue = row[colIndex + 1];
                                hasAvailableChoice = IsEven(nextValue);
                            }
                            ProcessPaths(hasAvailableChoice, colIndex, rowIndex, path);
                            break;
                        }
                    }
                    else
                    {
                        // Need odd value.

                        if (!isValueEven)
                        {
                            path.Add(val);
                            
                            Logger?.Write($"odd: {val}");

                            binaryTreeStartColIndex = colIndex;
                            var hasAvailableChoice = false;
                            if (binaryTreeColIndex < binaryTreeThreshold &&
                                colIndex < row.Length - 1 &&
                                rowIndex < Array.Length)
                            {
                                var nextValue = row[colIndex + 1];
                                hasAvailableChoice = !IsEven(nextValue);
                            }
                            ProcessPaths(hasAvailableChoice, colIndex, rowIndex, path);
                            break;
                        }
                    }

                    binaryTreeColIndex++;
                }

                rowIndex++;

                if (rowIndex == Array.Length)
                {
                    // End of triangle (last row).

                    doBackwardStep = true;

                    var currentPathSumm = path.Sum();
                    if (maxSumm < currentPathSumm)
                    {
                        fullTraveledPathWithMaxSumm.Clear();
                        fullTraveledPathWithMaxSumm.AddRange(path);
                        maxSumm = currentPathSumm;
                    }

                    Logger?.WriteLine($"Total summ of the pass {SuccessfulPathCount}: {currentPathSumm}");
                    Logger?.WriteLine();

                    SuccessfulPathCount++;

                    if (PathsWithAvailableChoice.Any())
                    {
                        LogStartPath(SuccessfulPathCount);
                    }
                }

            } while (true);

            maxSummPath = fullTraveledPathWithMaxSumm.ToArray();

            return maxSumm;
        }
        
        [ExcludeFromCodeCoverage]
        protected internal void LogStartPath(int pathCount)
        {
            Logger?.WriteLine($"\tStart path {pathCount}\t", ConsoleColor.Green, ConsoleColor.Black);
        }

        /// <summary>
        /// Add bad path or path with available choice.
        /// </summary>
        /// <param name="hasAvailableChoice"></param>
        /// <param name="colIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="path"></param>
        protected internal void ProcessPaths(bool hasAvailableChoice, int colIndex, int rowIndex, List<int> path)
        {
            if (hasAvailableChoice)
            {
                if (BadPaths.Any(x => x.ColIndex == colIndex + 1 && x.RowIndex == rowIndex + 1))
                {
                    Logger?.WriteLine($" Bad path at row {rowIndex} and column {colIndex}.");
                    return;
                }

                var currentLocation = new SavedLocation()
                {
                    ColIndex = colIndex,
                    RowIndex = rowIndex,
                    Path = new List<int>()
                };

                currentLocation.Path.AddRange(path.Take(path.Count - 1));

                PathsWithAvailableChoice.Push(currentLocation);
                
                Logger?.Write(" Has available choice. Save location.");
            }

            Logger?.WriteLine();
        }

        protected internal static bool IsEven(int number)
        {
            return number % 2 == 0;
        }
        
        [ExcludeFromCodeCoverage]
        protected internal void LogArray(int[][] arr)
        {
            foreach (var ints in arr)
            {
                foreach (var integer in ints)
                {
                    Logger?.Write($"{integer}, ");
                }
                Logger?.WriteLine();
            }
        }
    }
}
