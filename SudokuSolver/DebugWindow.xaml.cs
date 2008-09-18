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
using System.Windows.Shapes;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        int _count;

        public DebugWindow()
        {
            InitializeComponent();
        }

        public int Count
        {
            get { return _count; }
            set 
            {
                _count = value;
                txbCount.Text = _count.ToString();
            }
        }

        public void WriteLine(string outStr)
        {
            txtConsole.AppendText(outStr + Environment.NewLine);
        }
    }
}
