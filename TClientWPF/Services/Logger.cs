using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TClientWPF.Services
{
    public class Logger : INotifyPropertyChanged
    {
        private StringBuilder log;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Log => log.ToString();

        public Logger() => log = new StringBuilder();

        public void AddText(string text)
        {
            log.Append($"{DateTime.Now:dd-MM-yyyy HH:mm:ss} {text}\n");
            OnPropertyChanged("Log");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
