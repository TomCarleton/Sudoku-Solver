# Sudoku-Solver
A C# application for solving sudoku puzzles using a hybrid bifurcation approach.

To download and use the program, you'll only need to download 'Sudoku Solver.exe'

This program uses a combination of solving methods, making it much more efficient than a pure brute force solve. Firstly, the puzzle is 'solved' using the following logic. 

- For each grid square, calculate possible values. If there is only 1 possibility, then this number is filled in immediately.

- For each missing number in each row/column/box, check how many valid placements there are in this row/column/box. If there is only 1 valid placement, then fill in this grid square.

The above logic is sufficient to solve basic puzzles, however if this does not fully solve the puzzle we employ a recursive bifurcation approach. This involves finding a grid square with 2 possible values, and attempting to solve the puzzle by 'guessing' one of the two values and then using the above solve logic. If the method reaches a dead end with this 'guess', then the correct solution must use the other possible number.
