using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Border KeyboardBorderFocus;
        SudokuGame sudoku = new SudokuGame();

        public Window1()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Window1_KeyDown);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Border sqBorder;
            System.Windows.Controls.TextBlock sqText;

            //add controls to each square of the grid
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    // each square has a border containing a textblock
                    sqText = new TextBlock();
                    sqText.TextAlignment = TextAlignment.Center;
                    
                    sqBorder = new Border();
                    sqBorder.BorderBrush = Brushes.Black;
                    sqBorder.BorderThickness = CalcGridLineThickness(row,col);
                    sqBorder.Child = sqText;

                    sqBorder.MouseEnter += new MouseEventHandler(Square_MouseEnter);
                    sqBorder.MouseLeave += new MouseEventHandler(Square_MouseLeave);

                    SetFontSize(sqBorder);

                    // add square to grid
                    Grid.SetRow(sqBorder, row);
                    Grid.SetColumn(sqBorder, col);
                    PuzzleGrid.Children.Add(sqBorder);
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

        private void SetFontSize(Border border)
        {
            TextBlock tb = ((TextBlock)border.Child);
            if (tb.Text.Length < 2)
                tb.FontSize = PuzzleGrid.ActualHeight / 13;
            else
                tb.FontSize = PuzzleGrid.ActualHeight / 39;
        }

        private void SetBackground(Border border)
        {
        }

        /// ==================================================================================
        /// Keyboard Input Handling
        /// ==================================================================================
        
        void Window1_KeyDown(object sender, KeyEventArgs e)
        {
            //only process keys if we have a square with input focus
            if (KeyboardBorderFocus != null)
            {
                TextBlock tb = (TextBlock)KeyboardBorderFocus.Child;

                //standard numeric keys 1 to 9
                if (e.Key > Key.D0 && e.Key <= Key.D9)
                {
                    KeyConverter k = new KeyConverter();
                    tb.Text = k.ConvertToString(e.Key);
                }
                //number pad keys 1 to 9
                else if (e.Key > Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    KeyConverter k = new KeyConverter();
                    tb.Text = (k.ConvertToString(e.Key)).Remove(0, 6);
                }
                //use anything to clear
                else
                    tb.Text = "";

                ProcessSquares(new SquareOperation(ClearUnknownSquares));
                SetFontSize(KeyboardBorderFocus);
            }
        }

        private void Square_MouseEnter(object sender, MouseEventArgs e)
        {
            KeyboardBorderFocus = (Border)sender;
            KeyboardBorderFocus.Background = Brushes.Yellow;
        }

        private void Square_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = Brushes.White;
            KeyboardBorderFocus = null;
        }

        /// ==================================================================================
        /// Grid Operations
        /// ==================================================================================

        delegate void SquareOperation(Border border);

        private void ProcessSquares(SquareOperation func)
        {
            //loop through puzzle grid and call an operation on each border control it contains
            foreach (UIElement child in PuzzleGrid.Children)
            {
                if (child.GetType() == typeof(System.Windows.Controls.Border))
                {
                    func((Border)child);
                }
            }
        }

        private void ClearSquares(Border border)
        {
            TextBlock tb = ((TextBlock)border.Child);
            tb.Text = String.Empty;
            tb.Background = Brushes.Transparent;
            SetFontSize(border);
        }

        private void ClearUnknownSquares(Border border)
        {
            TextBlock tb = ((TextBlock)border.Child);
            if (tb.Text.Length > 1)
            {
                tb.Text = String.Empty;
            }
        }


        private void CopySquareToBoard(Border border)
        {
            int row = Grid.GetRow(border);
            int col = Grid.GetColumn(border);
            TextBlock tb = ((TextBlock)border.Child);
            if (tb.Text != String.Empty)
            {
                int val = Convert.ToInt32(tb.Text);
                sudoku.Board[row, col].SetKnownValue(val);
                //tb.Background = Brushes.AliceBlue;
            }
        }

        private void CopyBoardToSquare(Border border)
        {
            int row = Grid.GetRow(border);
            int col = Grid.GetColumn(border);
            TextBlock tb = ((TextBlock)border.Child);
            tb.Text = sudoku.Board[row, col].ToString(); ;
            SetFontSize(border);
        }

        private void CopyBoardToGrid()
        {
            ProcessSquares(new SquareOperation(CopyBoardToSquare));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ProcessSquares(new SquareOperation(SetFontSize));
        }

        /// ==================================================================================
        /// Command Handling
        /// ==================================================================================

        private void cmdSolve_Click(object sender, RoutedEventArgs e)
        {
            ProcessSquares(new SquareOperation(CopySquareToBoard));
            sudoku.Solve();
            CopyBoardToGrid();
        }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            sudoku.Clear();
            ProcessSquares(new SquareOperation(ClearSquares));
        }

        private void cmdStep_Click(object sender, RoutedEventArgs e)
        {
            sudoku.FindAnswers();
            CopyBoardToGrid();
        }
    }
}
