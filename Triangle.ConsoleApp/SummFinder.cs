using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Triangle.ConsoleApp
{
    public class SummFinder
    {
        private Stack<SavedLocation> PathsWithAvailableChoice { get; } = new Stack<SavedLocation>();

        /// <summary>
        /// List of bad paths to prevent pass the same path repeatedly.
        /// </summary>
        private List<SavedLocation> BadPaths { get; } = new List<SavedLocation>();

        /// <summary>
        /// List of summ of the each path.
        /// </summary>
        private List<int> SumList { get; } = new List<int>();
        
        /// <summary>
        /// Array of input values.
        /// </summary>
        private int[][] Array = null;

        public SummFinder(string path)
        {
            Array = ImportData(path);
        }

        public static int[][] ImportData(string path)
        {
            var lines = File.ReadAllLines(path);

            var arr = new int[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                var cols = lines[i].Split(' ');
                arr[i] = new int[cols.Length];
                for (int j = 0; j < cols.Length; j++)
                {
                    arr[i][j] = int.Parse(cols[j]);
                }
            }

            return arr;
        }

        public int GetMaxSumm()
        {
            PathsWithAvailableChoice.Clear();
            BadPaths.Clear();
            SumList.Clear();

            var passCount = 0; // Possible paths passes.

            Console.ForegroundColor = ConsoleColor.White;

            var valList = new List<int>(); // List of summs of already traveled path.
            
            bool needEven = false, doBackwardStep = false;

            int binaryTreeStartColIndex = 0, 
                binaryTreeColIndex = 0, // Internal tree index.
                binaryTreeThreshold = 1; // Threshold of the internal three index.

            int rowIndex = 0, colIndex = 0;

            ConsoleWriteStartPass(passCount);

            SavedLocation backwardStep = null;
            do
            {
                if (doBackwardStep)
                {
                    if (PathsWithAvailableChoice.Count == 0)
                        break;

                    // Restore previous location.
                    backwardStep = PathsWithAvailableChoice.Pop();
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"<- Get back to the row {backwardStep.RowIndex} and column {backwardStep.ColIndex}.");
                    Console.BackgroundColor = ConsoleColor.Black;
                    doBackwardStep = false;

                    rowIndex = backwardStep.RowIndex + 1; // Start from the next row index.
                    binaryTreeStartColIndex = backwardStep.ColIndex + 1; // Start from the next column index.
                    binaryTreeColIndex = 1; // Start from the next internal tree index.

                    valList.Clear();
                    valList.Add(backwardStep.Sum);
                }
                else
                {
                    binaryTreeColIndex = 0;
                }

                colIndex = binaryTreeStartColIndex;

                needEven = !isEven(rowIndex); // If this row is odd, we need even value.

                var row = Array[rowIndex];

                ConsoleWriteProcessRow(rowIndex);

                for (; colIndex < row.Length; colIndex++)
                {
                    if (binaryTreeColIndex > binaryTreeThreshold)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write($"Binary tree reached.");
                        Console.ResetColor();
                        if (backwardStep != null)
                        {
                            Console.Write($" Add bad path at row {backwardStep.RowIndex} and column {backwardStep.ColIndex}.");
                            BadPaths.Add(backwardStep);
                        }
                        Console.WriteLine();
                        doBackwardStep = true;
                        break;
                    }

                    int val = row[colIndex];

                    var isCellEven = isEven(val);

                    if (needEven)
                    {
                        // Need even value.

                        if (isCellEven)
                        {
                            valList.Add(val);
                            Console.Write($"even: {val}");
                            binaryTreeStartColIndex = colIndex;
                            var hasAvailableChoice = false;
                            if (binaryTreeColIndex < binaryTreeThreshold &&
                                colIndex < row.Length - 1 &&
                                rowIndex < Array.Length - 1)
                            {
                                var nextValue = row[colIndex + 1];
                                hasAvailableChoice = isEven(nextValue);
                            }
                            
                            ProcessPaths(hasAvailableChoice, colIndex, rowIndex, valList);
                            break;
                        }
                    }
                    else
                    {
                        // Need odd value.

                        if (!isCellEven)
                        {
                            valList.Add(val);
                            Console.Write($"odd: {val}");
                            binaryTreeStartColIndex = colIndex;
                            var hasAvailableChoice = false;
                            if (binaryTreeColIndex < binaryTreeThreshold &&
                                colIndex < row.Length - 1 &&
                                rowIndex < Array.Length - 1)
                            {
                                var nextValue = row[colIndex + 1];
                                hasAvailableChoice = !isEven(nextValue);
                            }
                            ProcessPaths(hasAvailableChoice, colIndex, rowIndex, valList);
                            break;
                        }
                    }

                    binaryTreeColIndex++;
                }

                rowIndex++;

                if (rowIndex == Array.Length)
                {
                    SumList.Add(valList.Sum());
                    doBackwardStep = true;
                    Console.WriteLine($"Total summ of the pass {passCount}: {SumList[passCount]}");
                    Console.WriteLine();
                    passCount++;

                    if (PathsWithAvailableChoice.Any())
                    {
                        ConsoleWriteStartPass(passCount);
                    }
                }

            } while (true);

            return SumList.Max();
        }

        private static void ConsoleWriteProcessRow(int rowIndex)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write($"Process row {rowIndex} ");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void ConsoleWriteStartPass(int passCount)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"\tStart pass {passCount}\t");
            Console.ResetColor();
        }

        /// <summary>
        /// Add bad path or path with available choice.
        /// </summary>
        /// <param name="hasAvailableChoice"></param>
        /// <param name="colIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="valList"></param>
        private void ProcessPaths(bool hasAvailableChoice, int colIndex, int rowIndex, List<int> valList)
        {
            if (hasAvailableChoice)
            {
                if (BadPaths.Any(x => x.ColIndex == colIndex + 1 && x.RowIndex == rowIndex + 1))
                {
                    Console.Write($" Bad path at row {rowIndex} and column {colIndex}");
                    Console.Write(Environment.NewLine);
                    return;
                }

                PathsWithAvailableChoice.Push(new SavedLocation()
                {
                    ColIndex = colIndex,
                    RowIndex = rowIndex,
                    Sum = valList.Sum()
                });
                Console.Write(" Has available choice. Save location.");
            }

            Console.Write(Environment.NewLine);
        }

        static bool isEven(int number)
        {
            return number % 2 == 0;
        }

        static void PrintArray(int[][] arr)
        {
            foreach (var ints in arr)
            {
                Console.WriteLine("\n");
                foreach (var integer in ints)
                {
                    Console.Write($"{integer}, ");
                }
            }
        }
    }
}
