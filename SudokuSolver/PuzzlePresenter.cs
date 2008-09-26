using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SudokuSolver
{

    class PuzzlePresenter
    {
        // Responsible for presentation and interaction with the puzzle grid
        // Depends on the BoardViewModel

        Grid _puzzleGrid;
        Border _KeyboardBorderFocus;
        Border[,] _puzzleBorder;
        BoardViewModel _board;

        public PuzzlePresenter(System.Windows.Controls.Grid container, BoardViewModel boardmodel)
        {
            _puzzleGrid = container;
            _board = boardmodel;
            CreatePuzzle();
        }

        private void CreatePuzzle()
        {
            System.Windows.Controls.Border sqBorder;
            System.Windows.Controls.TextBlock sqText;

            _puzzleBorder = new Border[9, 9];

            //add controls to each square of the grid
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    // each square has a border containing a textblock
                    sqText = new TextBlock();
                    sqText.TextAlignment = TextAlignment.Center;

                    sqBorder = new Border();
                    sqBorder.BorderBrush = Brushes.Black;
                    sqBorder.BorderThickness = CalcGridLineThickness(row, col);
                    sqBorder.Child = sqText;

                    sqBorder.MouseEnter += new MouseEventHandler(Square_MouseEnter);
                    sqBorder.MouseLeave += new MouseEventHandler(Square_MouseLeave);

                    SetFontSize(sqBorder);

                    // keep track of these borders in an array
                    _puzzleBorder[row, col] = sqBorder;

                    // add border to grid
                    Grid.SetRow(sqBorder, row);
                    Grid.SetColumn(sqBorder, col);
                    _puzzleGrid.Children.Add(sqBorder);

                    // bind textblock to Board/Square view model
                    Binding bind = new Binding();
                    bind.Source = _board[row, col];
                    bind.Path = new PropertyPath("Value");
                    sqText.SetBinding(TextBlock.TextProperty, bind);

                    // bind PuzzlePresenter to Square property change notification
                    _board[row, col].PropertyChanged += new PropertyChangedEventHandler(square_PropertyChanged);
                }
        }

        private Thickness CalcGridLineThickness(int row, int col)
        {
            // make a standard sudoku grid
            int top, left, right, bottom = 0;

            switch (row)
            {
                case 0:
                case 3:
                case 6:
                    top = 2;
                    bottom = 1;
                    break;
                case 2:
                case 5:
                case 8:
                    top = 1;
                    bottom = 2;
                    break;
                default:
                    top = 1;
                    bottom = 1;
                    break;
            }

            switch (col)
            {
                case 0:
                case 3:
                case 6:
                    left = 2;
                    right = 1;
                    break;
                case 2:
                case 5:
                case 8:
                    left = 1;
                    right = 2;
                    break;
                default:
                    left = 1;
                    right = 1;
                    break;
            }

            return new Thickness(left, top, right, bottom);
        }

        /// ==================================================================================
        /// Event Handling
        /// ==================================================================================
        
        public void KeyPress(string key, Window1.GameMode puzzleMode)
        {
            if (_KeyboardBorderFocus != null)
            {
                //change the BoardViewModel
                int row = Grid.GetRow(_KeyboardBorderFocus);
                int col = Grid.GetColumn(_KeyboardBorderFocus);

                if (_board.IsValid(row, col, key))
                {
                    _board[row, col].Value = key;
                    if (puzzleMode == Window1.GameMode.EnterPuzzle)
                    {
                        _board[row, col].IsStart = true;
                        _board[row, col].IsKnown = true;
                    }
                }
                SetFontSize(_KeyboardBorderFocus);
            }
        }

        private void Square_MouseEnter(object sender, MouseEventArgs e)
        {
            _KeyboardBorderFocus = (Border)sender;
            _KeyboardBorderFocus.Background = Brushes.Yellow;
        }

        private void Square_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = Brushes.White;
            _KeyboardBorderFocus = null;
        }

        delegate void SquareOperation(Border border);

        private void ProcessSquares(SquareOperation func)
        {
            //loop through puzzle grid and call an operation on each border control it contains
            foreach (UIElement child in _puzzleGrid.Children)
            {
                if (child.GetType() == typeof(System.Windows.Controls.Border))
                {
                    func((Border)child);
                }
            }
        }

        private void SetFontSize(Border border)
        {
            int row = Grid.GetRow(border);
            int col = Grid.GetColumn(border);

            TextBlock tb = ((TextBlock)border.Child);
            if (_board[row,col].IsKnown)
                tb.FontSize = _puzzleGrid.ActualHeight / 13;
            else
                tb.FontSize = _puzzleGrid.ActualHeight / 39;
        }

        public void SizeChanged()
        {
            ProcessSquares(new SquareOperation(SetFontSize));
        }

        void square_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Value":
                    SetFontSize(FindPuzzleSquare((SquareViewModel)sender));
                    break;
            }
        }

        private bool FindSquareRowCol(SquareViewModel square, ref int row, ref int col)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (_board[i, j] == square)
                    {
                        row = i;
                        col = j;
                        return true;
                    }
            return false;
        }

        private Border FindPuzzleSquare(SquareViewModel square)
        {
            int row = 0;
            int col = 0;

            if (FindSquareRowCol(square, ref row, ref col))
            {
                return _puzzleBorder[row, col];
            }
            else
                return null;
        }
    }
}
