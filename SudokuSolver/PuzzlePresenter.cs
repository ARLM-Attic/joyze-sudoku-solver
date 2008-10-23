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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

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

            // consume BoardViewModel property change notifications
            _board.ActivityEvent += new ActivityEventHandler(board_Activity);

        }

        private void CreatePuzzle()
        {
            System.Windows.Controls.Border sqBorder;
            //System.Windows.Controls.TextBlock sqText;
            _puzzleBorder = new Border[9, 9];

            //add controls to each square of the grid
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    sqBorder = new Border();
                    sqBorder.BorderBrush = Brushes.Black;
                    sqBorder.BorderThickness = CalcGridLineThickness(row, col);
                    sqBorder.Style = _puzzleGrid.FindResource("FocusBorderStyle") as Style;

                    ContentControl sqContent = new ContentControl();
                    sqContent.Content = _board[row, col];

                    // add render transforms to textblock for animations
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(new ScaleTransform());
                    tg.Children.Add(new TranslateTransform());
                    sqContent.RenderTransform = tg;

                    sqBorder.Child = sqContent;

                    sqBorder.MouseEnter += new MouseEventHandler(Square_MouseEnter);
                    sqBorder.MouseLeave += new MouseEventHandler(Square_MouseLeave);

                    // keep track of these borders in an array
                    _puzzleBorder[row, col] = sqBorder;

                    // add border to grid
                    Grid.SetRow(sqBorder, row);
                    Grid.SetColumn(sqBorder, col);
                    _puzzleGrid.Children.Add(sqBorder);

                }
        }

        private Thickness CalcGridLineThickness(Border border)
        {
            int row = Grid.GetRow(border);
            int col = Grid.GetColumn(border);

            return CalcGridLineThickness(row, col);
        }

        private Thickness CalcGridLineThickness(int row, int col)
        {
            // make a standard sudoku grid

            int top, left, right, bottom = 0;
            int thick = (int) (_puzzleGrid.ActualHeight + _puzzleGrid.ActualWidth) / 300;
            int thin = thick / 2;

            switch (row)
            {
                case 0:
                case 3:
                case 6:
                    top = thick;
                    bottom = thin;
                    break;
                case 2:
                case 5:
                case 8:
                    top = thin;
                    bottom = thick;
                    break;
                default:
                    top = thin;
                    bottom = thin;
                    break;
            }

            switch (col)
            {
                case 0:
                case 3:
                case 6:
                    left = thick;
                    right = thin;
                    break;
                case 2:
                case 5:
                case 8:
                    left = thin;
                    right = thick;
                    break;
                default:
                    left = thin;
                    right = thin;
                    break;
            }

            return new Thickness(left, top, right, bottom);
        }

        /// ==================================================================================
        /// Properties
        /// ==================================================================================

        public enum PuzzleMode { Design, Play, Examine };

        private PuzzleMode _puzzleMode;

        public PuzzleMode Mode
        {
            get { return _puzzleMode; }
            set
            {
                _puzzleMode = value;
                ClearFocusSquare();
            }
        }

        /// ==================================================================================
        /// Event Handling
        /// ==================================================================================
        
        public void KeyPress(string key)
        {
            if (this.Mode == PuzzleMode.Design && _KeyboardBorderFocus != null)
            {
                //update the BoardViewModel
                int row = Grid.GetRow(_KeyboardBorderFocus);
                int col = Grid.GetColumn(_KeyboardBorderFocus);

                if (_board.IsValid(row, col, key))
                {
                    _board[row, col].IsKnown = true;
                    _board[row, col].IsStart = true;
                    _board[row, col].Value = key;
                    ZoomIn(_KeyboardBorderFocus);
                }
                else
                    FlashBorder(_KeyboardBorderFocus);
            }
        }

        private void Square_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Mode == PuzzleMode.Design)
            {
                _KeyboardBorderFocus = (Border)sender;
            }
        }

        private void Square_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Mode == PuzzleMode.Design)
            {
                ClearFocusSquare();
            }
        }

        private void ClearFocusSquare()
        {
            if (_KeyboardBorderFocus != null)
            {
                _KeyboardBorderFocus = null;
            }
        }

        public void SizeChanged()
        {
            ProcessSquares(new SquareOperation(Resize));
        }


        /// ==================================================================================
        /// Square Format Handling
        /// ==================================================================================
        
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

        private void Resize(Border border)
        {
            SetBorderThickness(border);
        }

        private void SetBorderThickness(Border border)
        {
            border.BorderThickness = CalcGridLineThickness(border);
        }


        /// ==================================================================================
        /// Animations
        /// ==================================================================================

        private void FlashBorder(Border border)
        {
            Storyboard sb = (Storyboard)_puzzleGrid.FindResource("Flash");
            border.BeginStoryboard(sb);
        }

        private void ZoomIn(Border border)
        {
            if (border.Child.GetType() == typeof(ContentControl))
            {

                Storyboard sb = (Storyboard)_puzzleGrid.FindResource("ZoomIn");
                DoubleAnimation daX = (DoubleAnimation)_puzzleGrid.FindName("ZoomInX");
                DoubleAnimation daY = (DoubleAnimation)_puzzleGrid.FindName("ZoomInY");
                daX.From = border.ActualWidth / 4;
                daY.From = border.ActualHeight / 4;
                ((ContentControl)border.Child).BeginStoryboard(sb);
            }
        }

        private void ZoomOut(Border border)
        {
            if (border.Child.GetType() == typeof(ContentControl))
            {
                Storyboard sb = (Storyboard)_puzzleGrid.FindResource("ZoomOut");
                DoubleAnimation daX = (DoubleAnimation)_puzzleGrid.FindName("ZoomOutX");
                DoubleAnimation daY = (DoubleAnimation)_puzzleGrid.FindName("ZoomOutY");
                daX.From = -1 * border.ActualWidth;
                daY.From = -1 * border.ActualHeight;
                ((ContentControl)border.Child).BeginStoryboard(sb);
            }
        }

        /// ==================================================================================
        /// Event Notification Consumer Methods
        /// ==================================================================================


        void board_Activity(object sender, ActivityEventArgs e)
        // watch BoardViewModel changes
        {
            switch (e.Activity)
            {
                case "ReadStep":
                    Border bdr = _puzzleBorder[e.Row, e.Column];
                    ZoomOut(bdr);
                    break;
            }
        }


        /// ==================================================================================
        /// Helper Methods
        /// ==================================================================================


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

    }
}
