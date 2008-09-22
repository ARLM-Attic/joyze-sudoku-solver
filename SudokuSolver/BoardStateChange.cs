using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class BoardStateChange
    {
        public enum StateChange { Set, Reduce };

        private StateChange _op;
        private int _row;
        private int _col;
        private int _value;

        public BoardStateChange(StateChange op, int row, int col, int value)
        {
            _op = op;
            _row = row;
            _col = col;
            _value = value;
        }

        public StateChange Operation
        {
            get { return _op; }
        }

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
