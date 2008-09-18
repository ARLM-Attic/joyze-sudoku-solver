using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public delegate void FoundHandler(int row, int col, int value);

    class PossibleSet
    {
        enum Operation { Add, Remove };

        bool[] _digits; // we ignore the 0 index
        int _count;
        bool _known;

        // guest properties
        int _row;
        int _col;
        FoundHandler _foundCallBack;

        public PossibleSet(int row, int col, FoundHandler func)
        {
            _row = row;
            _col = col;
            _foundCallBack = func;
            _digits = new bool[10];
            _known = false;
            _count = 0;
            SetAllValues(false);
        }

        public int Count {get { return _count; } }
        public int Row {get { return _row; }}
        public int Column { get { return _col; } }

        public override bool Equals(object obj)
        {
            // implement value-based equality
            if (obj == null) return false;
            if (Object.ReferenceEquals(this, obj)) return true;
            if (this.GetType() != obj.GetType()) return false;

            //test for value equality of count and the digits array
            PossibleSet set = (PossibleSet)obj;
            if (set.Count != this.Count)
                return false;
            for (int i = 1; i < 10; i++)
                if (set._digits[i] != this._digits[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode() ;
        }

        public static bool operator ==(PossibleSet a, PossibleSet b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            if (a._count != b._count)
                return false;
            for (int i = 1; i < 10; i++)
                if (a._digits[i] != b._digits[i])
                    return false;

            return true;
        }

        public static bool operator !=(PossibleSet a, PossibleSet b)
        {
            return !(a == b);
        }

        public int Value
        {
            // only valid if the set contains one number (is known)
            get
            {
                if (_count == 1)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        if (_digits[i] == true)
                        {
                            return i;
                        }
                    }
                }
                return 0;
            }
        }

        public bool Contains(int value)
        {
            if (value > 0 && value < 10)
                return _digits[value];
            else
                return false;
        }

        public override string ToString()
        {
            string str = String.Empty;

            for (int i = 1; i < 10; i++)
            {
                if (_digits[i] == true)
                {
                    str = str + i.ToString();
                }
            }
            return str;
        }

        public string ToDebugString()
        {
            return this.ToString() + " @ (" + this.Row + "," + this.Column + ")";
        }

        public void AddOneToNine()
        {
            SetAllValues(true);
            _count = 9;
        }

        public void SetKnownValue(int value)
        {
            if (value > 0 && value < 10)
            {
                SetAllValues(false);
                _digits[value] = true;
                _known = true;
                _count = 1;
            }
            else // it's really still unknown
            {
                throw new ArgumentOutOfRangeException("value", "must be between 1 and 9");
            }
        }

        public void SetKnownValueFound(int value)
        {
            SetKnownValue(value);
            _foundCallBack(_row, _col, this.Value);
        }

        public bool IsKnownValue()
        {
            return _known;
        }

        public void Add(int value)
        {
            if (value > 0 && value < 10 & !_known)
            {
                if (_digits[value] == false)
                {
                    _digits[value] = true;
                    _count++;
                }
            }
        }

        public void Add(PossibleSet set)
        {
            // union the two sets tow
            for (int i = 1; i < 10; i++)
            {
                if (set._digits[i] == true && this._digits[i] == false)
                {
                    this.Add(i);
                }
            }
        }

        public void SetPair(int val1, int val2)
        {
            if (val1 > 0 && val1 < 10 && val2 > 0 && val2 < 10)
            {
                SetAllValues(false);
                _digits[val1] = true;
                _digits[val2] = true;
                _known = false;
                _count = 2;
            }
            else
                throw new ArgumentOutOfRangeException("parameters must be between 1 and 9");

        }

        public void Reduce(int value)
        {
            if (!_known)
            {
                Remove(value);
            }
        }

        public void Remove(int value)
        {
            if (_digits[value] == true)
            {
                _digits[value] = false;
                _count = _count - 1;

                //there's only one possibility left!
                //notify parent that a forced value has been found
                if (_count == 1)
                {
                    _known = true;
                    if (_foundCallBack != null)
                        _foundCallBack(_row, _col, this.Value);
                }
                else if (_count == 0)
                    _known = false;
            }
        }

        public bool Remove(PossibleSet set)
        {
            bool progress = false;

            for (int i = 1; i < 10; i++)
            {
                if (set._digits[i] == true && this._digits[i] == true)
                {
                    this.Remove(i);
                    progress = true;
                }
            }
            return progress;
        }

        private void SetAllValues(bool value)
        {
            for (int i = 1; i < 10; i++)
            {
                _digits[i] = value;
            }
        }
    }
}
