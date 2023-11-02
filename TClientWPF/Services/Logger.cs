using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TClientWPF.Services
{
    //Реализует Одиночку
    public class Logger : INotifyPropertyChanged
    {
        private StringBuilder log;
        private static Logger instance;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Log => log.ToString();

        private Logger() => log = new StringBuilder();

        public static Logger GetInstance()
        {
            if (instance == null) 
                instance = new Logger();
            return instance;
        }

        public void AddText(string text)
        {
            log.Append($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] {text}\n");
            OnPropertyChanged("Log");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
