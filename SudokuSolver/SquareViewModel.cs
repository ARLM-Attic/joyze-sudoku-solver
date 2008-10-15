using System;
using System.Collections.Generic;
using System.ComponentModel; // INotifyPropertyChanged
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    class SquareViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propname)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
        }


        public SquareViewModel()
        {
            _candidates = new PossibleSet(0, 0, null);
        }

        private bool _isstart = false;
        public bool IsStart
        {
            get { return _isstart; }
            set { _isstart = value; }
        }

        private bool _isknown = false;
        public bool IsKnown
        {
            get { return _isknown; }
            set { _isknown = value; }
        }

        private PossibleSet _candidates;
        public PossibleSet Candidates
        {
            get { return _candidates; }
        }

        private string _value = String.Empty;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                Notify("Value");
            }
        }

        public void RefreshCandidates()
        {
            Notify("Candidates");
        }

        public int ToInt()
        {
            if (_value != String.Empty)
                return Convert.ToInt32(this._value);
            else
                return 0;
        }

        public void Reset()
        {
            this._candidates.Clear();
            this.Value = String.Empty;
            this.IsKnown = false;
            this.IsStart = false;
        }

    }
}
