﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            //watch for game settings changes
            GameSettings.Settings.PropertyChanged += settings_PropertyChanged;
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
                SquareViewModel sq = _square[changes[step].Row, changes[step].Col];
                sq.IsKnown = true;
                sq.Value = changes[step].Value.ToString();
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

        public void Square_Changed()
        {
        }

        void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsCandidatesDisplayed":

                    if (GameSettings.Settings.IsCandidatesDisplayed)
                        SetCandidates();

                    for (int row = 0; row < 9; row++)
                        for (int col = 0; col < 9; col++)
                            _square[row, col].Refresh();
                    break;
            }
        }

        private void SetCandidates()
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
                    sq.Refresh();
                }
        }

    }
}
