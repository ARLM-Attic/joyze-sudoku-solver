using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class BlockMap
    {
        // Block Numbers:
        // 0 1 2
        // 3 4 5
        // 6 7 8

        // _mapBlockToRowCol is used to map from a block number to the top, left row/col.
        static int[,] _mapBlockToRowCol = { { 0, 0 }, { 0, 3 }, { 0, 6 }, { 3, 0 }, { 3, 3 }, { 3, 6 }, { 6, 0 }, { 6, 3 }, { 6, 6 } };
        // _mapOffsetToRowCol is used to map from an index along a run (offset) to the distance from the top left of the block
        static int[,] _mapOffsetToRowCol = { { 0, 0 }, { 0, 1 }, { 0, 2 }, { 1, 0 }, { 1, 1 }, { 1, 2 }, { 2, 0 }, { 2, 1 }, { 2, 2 } };

        public static int BlockContaining(int row, int column)
        {
            return (row / 3) * 3 + (column / 3);
        }

        public static int BlockTopRow(int block)
        {
            return _mapBlockToRowCol[block, 0];
        }

        public static int BlockLeftColumn(int block)
        {
            return _mapBlockToRowCol[block, 1];
        }

        // square offset numbers within a block:
        // 0 1 2
        // 3 4 5
        // 6 7 8

        public static int Row(int block, int offset)
        {
            return BlockTopRow(block) + _mapOffsetToRowCol[offset, 0]; 
        }

        public static int Column(int block, int offset)
        {
            return BlockLeftColumn(block) + _mapOffsetToRowCol[offset, 1];
        }
    }
}
