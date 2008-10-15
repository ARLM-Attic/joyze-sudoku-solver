using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class ActivityEventArgs : EventArgs
    {
        public string Activity;
        public int Row;
        public int Column;
        public int Value;

        public ActivityEventArgs(string activity)
        {
            Activity = activity;
        }

        public ActivityEventArgs(string activity, int row, int col, int val)
        {
            Activity = activity;
            Row = row;
            Column = col;
            Value = val;
        }
    }

    public delegate void ActivityEventHandler(object sender, ActivityEventArgs e);

    class BoardViewModel
    {
        private SquareViewModel[,] _square;

        public event ActivityEventHandler ActivityEvent;

        protected void NotifyActivity(string act)
        {
            if (ActivityEvent != null)
                ActivityEvent(this, new ActivityEventArgs(act));
        }

        protected void NotifyActivity(string act, int row, int col, int val)
        {
            if (ActivityEvent != null)
                ActivityEvent(this, new ActivityEventArgs(act, row, col, val));
        }

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
                SquareViewModel sq = _square[changes[i].Row, changes[i].Col];
                sq.IsKnown = true;
                sq.Value = changes[i].Value.ToString();
            }
        }

        public void Read(BoardChangeList changes, int step)
        {
            if (step >= 0 && step < changes.Count)
            {
                int row = changes[step].Row;
                int col = changes[step].Col;
                int val = changes[step].Value;
                SquareViewModel sq = _square[row, col];
                sq.IsKnown = true;
                sq.Value = val.ToString();
                NotifyActivity("ReadStep", row, col, val);
            }
            if (GameSettings.Settings.IsCandidatesDisplayed)
                SetCandidates();
        }

        public int Write(BoardChangeList changes)
        {
            changes.Clear();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    SquareViewModel sq = _square[row, col];
                    if (sq.Value != String.Empty && sq.IsStart)
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

        public void ResetToStart()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    if (!_square[row,col].IsStart)
                        _square[row, col].Reset();
        }

        private bool[] CalcCandidates(int row, int col)
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
                bool[] digits = CalcCandidates(row, col);
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

        public void SetCandidates()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    SquareViewModel sq = _square[row, col];
                    bool[] digits = CalcCandidates(row, col);
                    sq.Candidates.Clear();
                    for (int i = 1; i < 10; i++)
                    {
                        if (digits[i])
                            sq.Candidates.Add(i);
                    }
                    sq.RefreshCandidates();
                }
        }

    }
}
