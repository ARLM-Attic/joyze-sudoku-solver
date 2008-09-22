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
        BoardViewModel board = new BoardViewModel();
        int sudokuStep;

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

                    // add border to grid
                    Grid.SetRow(sqBorder, row);
                    Grid.SetColumn(sqBorder, col);
                    PuzzleGrid.Children.Add(sqBorder);

                    // bind textblock to Board/Square view model
                    Binding bind = new Binding();
                    bind.Source = board[row, col];
                    bind.Path = new PropertyPath("Value");
                    sqText.SetBinding(TextBlock.TextProperty, bind);
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
        /// Keyboard Input Handling
        /// ==================================================================================
        
        void Window1_KeyDown(object sender, KeyEventArgs e)
        {
            //only process keys if we have a square with input focus
            if (KeyboardBorderFocus != null)
            {
                string key;

                //standard numeric keys 1 to 9
                if (e.Key > Key.D0 && e.Key <= Key.D9)
                {
                    KeyConverter k = new KeyConverter();
                    key = k.ConvertToString(e.Key);
                }
                //number pad keys 1 to 9
                else if (e.Key > Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    KeyConverter k = new KeyConverter();
                    key = (k.ConvertToString(e.Key)).Remove(0, 6);
                }
                //use anything to clear
                else
                    key = String.Empty;

                //change the BoardViewModel
                int row = Grid.GetRow(KeyboardBorderFocus);
                int col = Grid.GetColumn(KeyboardBorderFocus);
                board[row, col].Value = key;

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

        private void SetFontSize(Border border)
        {
            TextBlock tb = ((TextBlock)border.Child);
            if (tb.Text.Length < 2)
                tb.FontSize = PuzzleGrid.ActualHeight / 13;
            else
                tb.FontSize = PuzzleGrid.ActualHeight / 39;
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
            if (sudoku.ChangeList.Count == 0)
            {
                board.Write(sudoku.ChangeList);
                sudoku.Solve();
            }
            board.Read(sudoku.ChangeList);
            cmdStep.IsEnabled = false;
        }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            board.Reset();
            sudoku.ChangeList.Clear();
            cmdStep.IsEnabled = true;
        }

        private void cmdStep_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.ChangeList.Count == 0)
            {
                sudokuStep = board.Write(sudoku.ChangeList);
                sudoku.Solve();
            }

            if (sudokuStep < sudoku.ChangeList.Count)
                board.Read(sudoku.ChangeList, sudokuStep++);

        }
    }
}
