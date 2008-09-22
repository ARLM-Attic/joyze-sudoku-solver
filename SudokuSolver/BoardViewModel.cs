using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class BoardViewModel
    {
        private SquareViewModel[,] _square;

        public BoardViewModel()
        {
            _square = new SquareViewModel[9, 9];
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _square[row, col] = new SquareViewModel();
        }

        public void Read(BoardChangeList changes)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                _square[changes[i].Row, changes[i].Col].Value = changes[i].Value.ToString();
            }
        }

        public void Read(BoardChangeList changes, int step)
        {
            if (step >= 0 && step < changes.Count)
                _square[changes[step].Row, changes[step].Col].Value = changes[step].Value.ToString();

        }

        public int Write(BoardChangeList changes)
        {
            changes.Clear();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    SquareViewModel sq = _square[row, col];
                    if (sq.Value != String.Empty)
                    {
                        int val = Convert.ToInt32(sq.Value);
                        changes.AddKnown(row, col, val);
                    }
                }
            return changes.Count;
        }

        public void Reset()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    _square[row, col].Reset();
        }

        public SquareViewModel this[int row, int col]
        {
            get { return _square[row, col] ; }
        }   
    }
}
