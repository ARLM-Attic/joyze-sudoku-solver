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

        private string _value = String.Empty;

        public SquareViewModel()
        {
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Notify("Value");
            }
        }

        public void Reset()
        {
            this.Value = String.Empty;
        }

    }
}
