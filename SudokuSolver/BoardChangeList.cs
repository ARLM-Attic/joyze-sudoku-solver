using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class BoardChangeList :  List<BoardStateChange>
    {

        public BoardChangeList()
        {
            new List<BoardStateChange>(200);
        }

        public void AddKnown(int row, int col, int value)
        {
            this.Add(new BoardStateChange(BoardStateChange.StateChange.Set, row, col, value));
        }
    }
}
