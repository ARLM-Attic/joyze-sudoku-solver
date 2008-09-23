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

        public int ToInt()
        {
            if (_value != String.Empty)
                return Convert.ToInt32(this.Value);
            else
                return 0;
        }

        public void Reset()
        {
            this.Value = String.Empty;
        }

    }
}
