using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TClientWPF
{
    public interface IDialog
    {
        string FilePath { get; set; }
        void ShowMessage(string message);
        bool OpenFileDialog();
        bool SaveFileDialog();
    }
}
