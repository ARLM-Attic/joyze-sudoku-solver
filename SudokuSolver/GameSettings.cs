using System;
using System.Collections.Generic;
using System.ComponentModel; // INotifyPropertyChanged
using System.Linq;
using System.Text;

namespace SudokuSolver
{

    public class GameSettings : INotifyPropertyChanged
    {
        // INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propname)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
        }

        // thread-safe singleton pattern

        private static readonly GameSettings _instance = new GameSettings();

        private GameSettings()
        {
            _showCandidates = false;
        }

        public static GameSettings Settings
        {
            get
            {
                return _instance;
            }
        }

        // class members

        private bool _showCandidates;

        public bool IsCandidatesDisplayed
        {
            get { return _showCandidates; }
            set
            {
                _showCandidates = value;
                Notify("IsCandidatesDisplayed");
            }
        }

    }
}
