using Microsoft.Win32;
using System.Windows;

namespace TClientWPF.Services
{
    public class DefaultDialogService : IDialog
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Открыть файл";
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Файл настроек |*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.FileName = "Настройки.json";
            saveFileDialog.Filter = "Файл настроек |*.json";
            saveFileDialog.Title = "Сохранить файл";

            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage icon) => 
                    MessageBox.Show(message, caption, button, icon);
    }
}
