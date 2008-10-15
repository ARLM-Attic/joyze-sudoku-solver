using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class BoardStateChange
    {
        public enum StateChange { Set, Reduce };
        public StateChange Operation { get; set; }       
        public int Row { get; set; }
        public int Col { get; set; }
        public int Value { get; set; }

    }
}
