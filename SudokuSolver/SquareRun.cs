﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{

    class SquareRun
    {
        // This class abstracts a sequence of squares so we can make processing algorithms generic
        // There are three kinds of runs:

        public enum SquareRunType { RowRun, ColumnRun, BlockRun };

        // Block Numbers:
        // 0 1 2
        // 3 4 5
        // 6 7 8

        private SquareRunType _runtype;
        private SudokuBoard _board;
        private int _row;
        private int _column;
        private int _block;

        public int Position;

        // _excludes is used to remove specific offsets from a run in order to create a subrun
        private int[] _exclude;

        public static int BlockContaining(int row, int column)
        {
            return BlockMap.BlockContaining(row, column);
        }

        public SquareRun(SudokuBoard board, SquareRunType runtype, int position)
        {
            if ((position < 0 || position > 8) || board == null)
            {
                throw new ArgumentException("invalid SquareRun constructor call");
            }

            Position = position; // for debugging
            _runtype = runtype;
            _board = board;
            _exclude = null;

            switch (_runtype)
            {
                case SquareRunType.RowRun:
                    _row = position;
                    break;
                case SquareRunType.ColumnRun:
                    _column = position;
                    break;
                case SquareRunType.BlockRun:
                    _block = position;
                    //_row = BlockMap.BlockTopRow(_block);
                    //_column = BlockMap.BlockLeftColumn(_block);
                    
                    break;
            }
        }

        public PossibleSet this[int offset]
        {
            get
            {
                if (offset < 0 || offset > 8)
                {
                    throw new ArgumentException("invalid offset parameter");
                }

                switch (_runtype)
                {
                    case SquareRunType.RowRun:
                        if (_exclude != null)
                            for (int i = 0; i < _exclude.Length; i++)
                                if (offset == _exclude[i])
                                    return null;
                        return _board[_row, offset];

                    case SquareRunType.ColumnRun:
                        if (_exclude != null)
                            for (int i = 0; i < _exclude.Length; i++)
                                if (offset == _exclude[i])
                                    return null;
                            return _board[offset, _column];

                    case SquareRunType.BlockRun:

                        return _board[BlockMap.Row(_block,offset), BlockMap.Column(_block,offset)];
                }

                return null;
            }
        }

        // warning: implementation not robust for following two methods

        public SquareRun GetRun(int[] include)
        {
            // Given a blockrun and the squares to include, return the row
            // or column run which contains those squares.
            // Expecting [0,1,2] [3,4,5] [6,7,8] or [0,3,6] [1,4,7] [2,5,8]
            if (include[include.Length - 1] - include[0] > 3)
            {
                // we have a sub column
                //int col = _column + _mapOffsetToRowCol[include[0], 1];
                int col = BlockMap.Column(_block, include[0]);
                return new SquareRun(_board, SquareRunType.ColumnRun, col);
            }
            else // we have a sub row
            {
                //int row = _row + _mapOffsetToRowCol[include[0], 0];
                int row = BlockMap.Row(_block, include[0]);
                return new SquareRun(_board, SquareRunType.RowRun, row);
            }
        }

        public void ExcludeBlock(SquareRun blockrun)
        {
            // Given a row or column run, exclude squares from the passed blockrun
            // assumes block and run intersect
            if (this._runtype == SquareRunType.RowRun)
            {
                _exclude = new int[3];
                //_exclude[0] = _mapBlockToRowCol[blockrun._block, 1];
                _exclude[0] = BlockMap.BlockLeftColumn(blockrun._block);
                _exclude[1] = _exclude[0] + 1;
                _exclude[2] = _exclude[0] + 2;
            }
            else if (this._runtype == SquareRunType.ColumnRun)
            {
                _exclude = new int[3];
                //_exclude[0] = _mapBlockToRowCol[blockrun._block, 0];
                _exclude[0] = BlockMap.BlockTopRow(blockrun._block);
                _exclude[1] = _exclude[0] + 1;
                _exclude[2] = _exclude[0] + 2;
            }
            else
                throw new ArgumentException("invalid SquareRun type");
        }
    }
}
