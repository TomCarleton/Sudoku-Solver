using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver
{
    public class Box
    {
        // Define variables
        public int value;
        public List<int> possibleValues;

        public Box(int num)
        {
            value = num;
            possibleValues = new List<int>();

            // If zero, 9 possibilities
            if (value == 0)
            {
                for (int i = 1; i <= 9; i++)
                    possibleValues.Add(i);
            }
            else
                possibleValues.Add(value);  // If non-zero, only 1 possible value
        }
    }
}
