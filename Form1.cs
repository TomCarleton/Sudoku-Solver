using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sudoku_Solver
{
    public partial class Form1 : Form
    {
        // Initialise variables
        bool lockedForm = false;
        int[,] grid;
        Color wGrey = Color.FromArgb(40, 40, 40);
        Color wActive = Color.FromArgb(50, 50, 50);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private bool ValidateGrid()
        {
            int boxesFilled = 0;    // Variable to ensure 17+ boxes filled

            foreach (TextBox box in gridPanel.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            {
                // Check grid entry is a number
                if (!(box.Text.All(Char.IsNumber)))
                {
                    InvalidGrid("You may only enter numbers 1-9 into the grid.");
                    return false;
                }
                // Check grid entry != 0
                if (box.Text == "0")
                {
                    InvalidGrid("You may only enter numbers 1-9 into the grid.");
                    return false;
                }

                // If a valid nonzero grid entry, count as filled
                if (box.Text != "")
                    boxesFilled++;
            }

            // Puzzle cannot be solves with less than 17 starter numbers
            if (boxesFilled < 17)
            {
                InvalidGrid("Not enough numbers filled in.");
                return false;
            }

            // Check for duplicate numbers in rows/columns/boxes
            if (!CheckSolveable())
            {
                InvalidGrid("Puzzle has no solution.");
                return false;
            }

            // Else, grid is valid (test)
            return true;
        }

        private void PuzzleSolve()
        {
            // Prevent user from editing grid
            DeactivateGrid();

            // Create the puzzle class and solve
            Puzzle sudoku1 = new Puzzle(grid);
            BifurcationSolve(sudoku1);

            // Check if puzzle has been solved
            if (!IsGridFull(grid))
                InvalidGrid("Could not solve puzzle");
            else
                instructText.Text = "Solved";

            // Update grid with solution and allow user editing
            UpdateGrid();
            ActivateGrid();
        }

        private void BifurcationSolve(Puzzle inputPuzzle)
        {
            // First, we solve
            grid = inputPuzzle.Solve();

            // If grid is solved, we are done
            if (IsGridFull(grid))
                return;

            // Else, find bifurcations
            int[] bif = inputPuzzle.FindBifurcation();

            // If there are none, return
            if (bif[2] == 0)
                return;

            // Make copy of original grid before making 'guesses'
            int[,] gridCopy = grid;

            // Add first bif to copy of grid and solve
            gridCopy[bif[0], bif[1]] = bif[2];
            Puzzle sudokuCopy0 = new Puzzle(gridCopy);
            BifurcationSolve(sudokuCopy0);
            if (IsGridFull(grid))
                return;

            // Add second bif to copy of grid and solve
            gridCopy[bif[0], bif[1]] = bif[3];
            Puzzle sudokuCopy1 = new Puzzle(gridCopy);
            BifurcationSolve(sudokuCopy1);
        }

        private bool IsGridFull(int[,] testGrid)
        {
            for (int i = 0; i < 9; i++)
                for(int j = 0; j < 9; j++)
                {
                    if (testGrid[i, j] == 0)
                        return false;
                }
            return true;
        }

        private void InvalidGrid(string text)
        {
            instructText.ForeColor = Color.IndianRed;
            instructText.Text = text;
        }

        private bool CheckSolveable()
        {
            grid = new int[9, 9];
            // Convert grid into 2D array
            foreach (TextBox gridBox in gridPanel.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            {
                if (gridBox.Text != "")
                    grid[gridBox.TabIndex / 9, gridBox.TabIndex % 9] = Int32.Parse(gridBox.Text);
            }

            // Check each of the 9 rows/columns/boxes
            for (int i = 0; i < 9; i++)
            {
                // Create lists for current row/column/box
                var row = new List<int>();
                var column = new List<int>();
                var box = new List<int>();

                for (int j = 0; j < 9; j++)
                {
                    row.Add(grid[i, j]);
                    column.Add(grid[j, i]);
                    box.Add(grid[3 * (i / 3) + (j / 3), ((3 * i) % 9) + (j % 3)]);
                }

                // Remove excess 0s (blank squares)
                row.RemoveAll(num => num == 0);
                column.RemoveAll(num => num == 0);
                box.RemoveAll(num => num == 0);

                // Check if row valid
                if (row.Count != row.Distinct().Count())    // Removes all duplicate numbers and compares to original count
                    return false;

                // Check if column valid
                if (column.Count != column.Distinct().Count())
                    return false;

                // Check if box valid
                if (box.Count != box.Distinct().Count())
                    return false;

            }

            return true;
        }

        private void DeactivateGrid()
        {
            foreach (TextBox gridBox in gridPanel.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            {
                gridBox.Enabled = false;
            }
        }

        private void ActivateGrid()
        {
            foreach (TextBox gridBox in gridPanel.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            {
                gridBox.Enabled = true;
            }
        }

        private void UpdateGrid()
        {
            foreach (TextBox gridBox in gridPanel.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            {
                if (grid[gridBox.TabIndex / 9, gridBox.TabIndex % 9] != 0)
                    gridBox.Text = grid[gridBox.TabIndex / 9, gridBox.TabIndex % 9].ToString();
            }
        }

        private void ClearGrid()
        {
            foreach (TextBox gridBox in gridPanel.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            {
                gridBox.Enabled = true;
                gridBox.Text = "";
            }
            instructText.Text = "Insert starting numbers below, then click enter.";
            instructText.ForeColor = Color.White;
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void quitButton_MouseEnter(object sender, EventArgs e)
        {
            quitButton.BackColor = Color.Brown;
        }

        private void quitButton_MouseLeave(object sender, EventArgs e)
        {
            quitButton.BackColor = wGrey;
        }

        private void minButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void minButton_MouseEnter(object sender, EventArgs e)
        {
            minButton.BackColor = Color.FromArgb(60, 60, 60);
        }

        private void minButton_MouseLeave(object sender, EventArgs e)
        {
            minButton.BackColor = wGrey;
        }

        private void lockButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (lockedForm == false)
            {
                lockedForm = true;
                Form.ActiveForm.TopMost = true;
                lockButton.BackColor = Color.Brown;
            }
            else
            {
                lockedForm = false;
                Form.ActiveForm.TopMost = false;
                lockButton.BackColor = Color.FromArgb(60, 60, 60);
            }
        }

        private void lockButton_MouseEnter(object sender, EventArgs e)
        {
            if (lockedForm == false)
            {
                lockButton.BackColor = Color.FromArgb(60, 60, 60);
            }
        }

        private void lockButton_MouseLeave(object sender, EventArgs e)
        {
            if (lockedForm == false)
            {
                lockButton.BackColor = wGrey;
            }
        }

        // Required for window dragging
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            // Allows window to be dragged
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void square88_TextChanged(object sender, EventArgs e)
        {
            //TextBox thisSquare = sender as TextBox;

            //if (thisSquare.Text.Length == 1)
            //{
            //    thisSquare.BackColor = wActive;
            //}
            //else
            //{
            //    thisSquare.BackColor = wGrey;
            //}
        }

        private void enterButton_MouseEnter(object sender, EventArgs e)
        {
            enterButton.ForeColor = Color.White;
            enterButton.BackColor = wActive;
        }

        private void enterButton_MouseLeave(object sender, EventArgs e)
        {
            enterButton.ForeColor = Color.LightGray;
            enterButton.BackColor = wGrey;
        }

        private void enterButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (ValidateGrid())
                PuzzleSolve();
        }

        private void resetButton_MouseEnter(object sender, EventArgs e)
        {
            resetButton.ForeColor = Color.White;
            resetButton.BackColor = wActive;
        }

        private void resetButton_MouseLeave(object sender, EventArgs e)
        {
            resetButton.ForeColor = Color.LightGray;
            resetButton.BackColor = wGrey;
        }

        private void resetButton_MouseDown(object sender, MouseEventArgs e)
        {
            ClearGrid();
        }
    }
}
