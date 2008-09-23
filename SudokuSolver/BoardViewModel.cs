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
                        changes.AddKnown(row, col, sq.ToInt());
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

        private bool[] Candidates(int row, int col)
        {
            bool[] digits = new bool[10];

            // start with all digits that can occur
            for (int digit = 0; digit < 10; digit++)
                digits[digit] = true;

            // remove digits that occur in the column
            for (int i = 0; i < 9; i++)
                if (this[i,col].Value != String.Empty)
                    digits[this[i,col].ToInt()] = false;

            // remove digits that occur in the row
            for (int j = 0; j < 9; j++)
                if (this[row,j].Value != String.Empty)
                    digits[this[row, j].ToInt()] = false;

            // remove digits that occur in the block
            int block = BlockMap.BlockContaining(row, col);
            for (int offset = 0; offset < 9; offset++)
            {
                SquareViewModel square = this[BlockMap.Row(block, offset),BlockMap.Column(block,offset)];
                if (square.Value != String.Empty)
                    digits[square.ToInt()] = false;
            }
                
            return digits;
        }

        public bool IsValid(int row, int col, string value)
        {
            if (value != String.Empty)
            {
                bool[] digits = Candidates(row, col);
                int val = Convert.ToInt32(value);
                return digits[val];
            }
            else
                return true;
        }

        public SquareViewModel this[int row, int col]
        {
            get { return _square[row, col] ; }
        }   
    }
}
