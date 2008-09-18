using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class SudokuBoard
    {
        PossibleSet[,] _square = null;
        FoundHandler foundCallback;

        public SudokuBoard(FoundHandler foundNotification)
        {
            _square = new PossibleSet[9, 9];
            foundCallback = foundNotification;
            Clear();
        }

        public void Clear()
        {
            //create new board with all possible values in each square
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _square[row, col] = new PossibleSet(row, col, foundCallback);
        }

        public int CountKnowns()
        {
            int count = 0;
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    if (_square[row,col].IsKnownValue()) 
                        count++;
            return count;
        }

        public int CountPossibles()
        {
            int count = 0;
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    count = count + _square[row, col].Count;
            return count;
        }

        public PossibleSet this[int row, int col]
        {
            get { return _square[row, col]; }
        }   
    }
}
