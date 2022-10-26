using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver
{
    public class Puzzle
    {
        // Variables here
        public Box[,] grid;
        public bool solved = false;
        bool progress = true;

        public Puzzle(int[, ] inputGrid)
        {
            // Create blank grid
            grid = new Box[9, 9];

            // Assign puzzle grid as instructed, we assume it has a solution
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    grid[i,j] = new Box(inputGrid[i, j]);
                }
            }

            // Check if already solved
            if (IsFull())
                solved = true;
        }

        public int[,] Solve()
        {
            // Keep solving until fully solved
            while (solved == false && progress == true)
            {
                progress = false;
                // Iterate through each box in the grid, and update if there is only 1 possibility
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        // Only try and solve if value is zero (unsolved)
                        if (grid[i, j].value == 0)
                        {
                            // Remove possible values by row
                            for (int k = 0; k < 9; k++)   // Iterate across row, without self
                            {
                                if (i != k)
                                    grid[i, j].possibleValues.Remove(grid[k, j].value);
                            }

                            // Remove possible values by column
                            for (int k = 0; k < 9; k++)   // Iterate down column, without self
                            {
                                if (j != k)
                                    grid[i, j].possibleValues.Remove(grid[i, k].value);
                            }

                            // Remove possible values by box
                            for (int k = 3 * (i / 3); k < 3 * (i / 3) + 3; k++)   // Iterate through box
                            {
                                for (int l = 3 * (j / 3); l < 3 * (j / 3) + 3; l++)
                                {
                                    if (i != k && j != l)
                                        grid[i, j].possibleValues.Remove(grid[k, l].value);
                                }
                            }

                            // If only 1 possible value, update to this value
                            if (grid[i, j].possibleValues.Count == 1)
                            {
                                grid[i, j].value = grid[i, j].possibleValues[0];
                                progress = true;
                                //Debug.Fail($"Square[{i}, {j}] = {grid[i, j].possibleValues[0]}");
                            }
                        }
                    }
                }

                // Iterate through each row, column and box for each number and update if only 1 possibility
                for (int i = 0; i < 9; i++)
                {
                    // Initialise lists
                    var row = new List<int>();
                    var column = new List<int>();
                    var box = new List<int>();

                    // Fill lists with 1-9
                    for (int x = 1; x <= 9; x++)
                    {
                        row.Add(x);
                        column.Add(x);
                        box.Add(x);
                    }

                    // Remove numbers that already exist
                    for (int j = 0; j < 9; j++)
                    {
                        row.Remove(grid[i, j].value);
                        column.Remove(grid[j, i].value);
                        box.Remove(grid[3 * (i / 3) + (j / 3), ((3 * i) % 9) + (j % 3)].value);
                    }

                    // Check row & update if possible
                    foreach (int r in row)
                    {
                        int validCount = 0;
                        int posI = 0;
                        int posJ = 0;
                        for (int j = 0; j < 9; j++)
                        {
                            if (IsValid(i, j, r) && grid[i,j].value == 0)
                            {
                                validCount++;
                                posI = i;
                                posJ = j;
                                //Debug.Fail($"{r} is valid in ({posI}, {posJ})");
                            }
                        }
                        if (validCount == 1)
                        {
                            grid[posI, posJ].value = r;
                            progress = true;
                            //Debug.Fail($"Square[{posI}, {posJ}] = {r}");
                        }
                    }

                    // Check column & update if possible
                    foreach (int c in column)
                    {
                        int validCount = 0;
                        int posI = 0;
                        int posJ = 0;
                        for (int j = 0; j < 9; j++)
                        {
                            if (IsValid(j, i, c) & grid[j, i].value == 0)
                            {
                                validCount++;
                                posI = j;
                                posJ = i;
                                //Debug.Fail($"{c} is valid in ({posI}, {posJ})");
                            }
                        }
                        if (validCount == 1)
                        {
                            grid[posI, posJ].value = c;
                            progress = true;
                            //Debug.Fail($"Square[{posI}, {posJ}] = {c}");
                        }
                    }

                    // Check box & update if possible
                    foreach (int b in box)
                    {
                        int validCount = 0;
                        int posI = 0;
                        int posJ = 0;
                        for (int j = 0; j < 9; j++)
                        {
                            if (IsValid(3 * (i / 3) + (j / 3), ((3 * i) % 9) + (j % 3), b) && grid[3 * (i / 3) + (j / 3), ((3 * i) % 9) + (j % 3)].value == 0)
                            {
                                validCount++;
                                posI = 3 * (i / 3) + (j / 3);
                                posJ = ((3 * i) % 9) + (j % 3);
                            }
                        }
                        if (validCount == 1)
                        {
                            grid[posI, posJ].value = b;
                            progress = true;
                            //Debug.Fail($"Square[{posI}, {posJ}] = {b}");
                        }
                    }
                }

                // Check if grid is now solved
                solved = IsFull();
            }

            // Create output grid
            int[,] outputGrid = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j= 0; j < 9; j++)
                {
                    outputGrid[i, j] = grid[i, j].value;
                }
            }
            return outputGrid;
        }

        public bool IsFull()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j].value == 0)
                        return false;
                }
            }
            return true;
        }

        public bool IsValid(int i, int j, int val)
        {
            // Create blank lists
            var row = new List<int>();
            var column = new List<int>();
            var box = new List<int>();

            // Add numbers from rows/columns/boxes
            for (int k = 0; k < 9; k++)
            {
                row.Add(grid[k, j].value);
                column.Add(grid[i, k].value);
                box.Add(grid[3 * (i / 3) + (k % 3), 3 * (j / 3) + (k / 3)].value);
            }

            // Remove excess 0s (blank squares) and itself (if non-zero)
            row.RemoveAll(num => num == 0);
            row.Remove(grid[i, j].value);
            column.RemoveAll(num => num == 0);
            column.Remove(grid[i, j].value);
            box.RemoveAll(num => num == 0);
            box.Remove(grid[i, j].value);

            // Add trial number to list
            row.Add(val);
            column.Add(val);
            box.Add(val);

            // Check if row valid
            if (row.Count != row.Distinct().Count())    // Removes all duplicate numbers and compares to original count
                return false;

            // Check if column valid
            if (column.Count != column.Distinct().Count())
                return false;

            // Check if box valid
            if (box.Count != box.Distinct().Count())
                return false;

            return true;
        }

        public int[] FindBifurcation()
        {
            // Here we aim to find the first grid entry with only 2 possible values for a bifurcation

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j].possibleValues.Count() == 2)
                    {
                        int[] output0 = { i, j, grid[i, j].possibleValues[0], grid[i, j].possibleValues[1] };
                        return output0;
                    }
                }
            int[] output1 = { 0, 0, 0, 0 };
            return output1;
        }

    }
}
