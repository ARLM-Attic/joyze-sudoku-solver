using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    // Board model used by Sudoku game to solve the puzzle.
    // This is a different from BoardViewModel which is the board model used for presentation

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

        private void Clear()
        {
            //create new board with empty possible values in each square
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _square[row, col] = new PossibleSet(row, col, foundCallback);
        }

        public void Reset()
        {
            // reset solution board to include all possibilities (1..9) in every square
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _square[row, col].AddOneToNine();
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

        public void Process(BoardStateChange change)
        {
            switch (change.Operation)
            {
                case BoardStateChange.StateChange.Set:
                    Debug.WriteLine("reduce " + change.Value.ToString() + " at (" + change.Row.ToString() + "," + change.Col.ToString() + ")");
                    this[change.Row, change.Col].SetKnownValue(change.Value);
                    ReduceRun(new SquareRun(this, SquareRun.SquareRunType.RowRun, change.Row), change.Value);
                    ReduceRun(new SquareRun(this, SquareRun.SquareRunType.ColumnRun, change.Col), change.Value);
                    ReduceRun(new SquareRun(this, SquareRun.SquareRunType.BlockRun, SquareRun.BlockContaining(change.Row, change.Col)), change.Value);
                    break;
            }
        }

        private void ReduceRun(SquareRun square, int value)
        {
            for (int i = 0; i < 9; i++)
                square[i].Reduce(value);
        }
    }
}
