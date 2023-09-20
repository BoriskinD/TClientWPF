using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TClientWPF
{
    public interface IDialog
    {
        string FilePath { get; set; }
        void ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage icon);
        bool OpenFileDialog();
        bool SaveFileDialog();
    }
}
