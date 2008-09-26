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
        public enum GameMode { EnterPuzzle, Play, EndPlay };
        GameMode _puzzleMode = GameMode.EnterPuzzle;
        SudokuGame _sudoku = new SudokuGame();
        BoardViewModel _board = new BoardViewModel();
        PuzzlePresenter _puzzle;

        int _sudokuStep;

        public Window1()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Window1_KeyDown);

            //attach databinding context to window
            this.DataContext = GameSettings.Settings;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _puzzle = new PuzzlePresenter(PuzzleGrid, _board);
            _puzzle.Mode = PuzzlePresenter.PuzzleMode.Design;

            SetControlStates();
        }

        public void SetControlStates()
        {
            if (_puzzleMode == GameMode.EnterPuzzle)
            {
                cmdSolve.Content = "Start";
                cmdStep.IsEnabled = false;
                chkShowCandidates.IsEnabled = false;
            }
            else if (_puzzleMode == GameMode.Play)
            {
                cmdSolve.Content = "Solve";
                cmdStep.IsEnabled = true;
                chkShowCandidates.IsEnabled = true;
            }
            else // GameMode.EndPlay
            {
                cmdSolve.Content = "Replay";
                GameSettings.Settings.IsCandidatesDisplayed = false;
                chkShowCandidates.IsEnabled = false;
                cmdStep.IsEnabled = false;
            }
        }


        /// ==================================================================================
        /// Event Handling
        /// ==================================================================================
        
        void Window1_KeyDown(object sender, KeyEventArgs e)
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

            //let the puzzle take the input
            _puzzle.KeyPress(key);
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_puzzle != null)
                _puzzle.SizeChanged();
        }

        /// ==================================================================================
        /// Command Handling
        /// ==================================================================================

        private void cmdSolve_Click(object sender, RoutedEventArgs e)
        {
            if (_puzzleMode == GameMode.EnterPuzzle || _puzzleMode == GameMode.EndPlay)
            {
                // move into play mode
                _sudokuStep = _board.Write(_sudoku.ChangeList);
                _sudoku.Solve();
                _board.ResetToStart();
                _puzzleMode = GameMode.Play;
                _puzzle.Mode = PuzzlePresenter.PuzzleMode.Play;

            }
            else if (_puzzleMode == GameMode.Play)
            {
                // display solution and end play
                _board.Read(_sudoku.ChangeList);
                _puzzleMode = GameMode.EndPlay;
                _puzzle.Mode = PuzzlePresenter.PuzzleMode.Examine;
            }

            SetControlStates();
        }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            _board.Reset();
            _sudoku.ChangeList.Clear();
            _puzzleMode = GameMode.EnterPuzzle;
            _puzzle.Mode = PuzzlePresenter.PuzzleMode.Design;
            SetControlStates();
        }

        private void cmdStep_Click(object sender, RoutedEventArgs e)
        {
            if (_puzzleMode == GameMode.Play)
            {
                if (_sudokuStep < _sudoku.ChangeList.Count)
                    _board.Read(_sudoku.ChangeList, _sudokuStep++);

                if (_sudokuStep == _sudoku.ChangeList.Count)
                {
                    _puzzleMode = GameMode.EndPlay;
                    _puzzle.Mode = PuzzlePresenter.PuzzleMode.Examine;
                    SetControlStates();
                }
            }
        }
    }
}
