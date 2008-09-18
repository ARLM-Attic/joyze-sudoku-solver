using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    static class Debug
    {
        static DebugWindow _dbg = null;

        private static void CheckWindow()
        {
            if (_dbg == null)
            {
                _dbg = new DebugWindow();
                _dbg.Show();
            }
        }

        public static void SetCount(int count)
        {
            /*
            CheckWindow();
            _dbg.Count = count;
             */
        }

        public static void WriteLine(string textData)
        {
            /*
            CheckWindow();
            _dbg.WriteLine(textData);
             */
        }
    }
}
