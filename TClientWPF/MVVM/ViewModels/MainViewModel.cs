using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TClientWPF.Interfaces;
using TClientWPF.Model;
using TClientWPF.Services;
using TClientWPF.Views;

namespace TClientWPF.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IWindow window;
        private Settings settings;
        private TClient client;
        private RelayCommand startCommand;
        private RelayCommand settingsCommand;
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand SettingsCommand
        {
            get => settingsCommand;
            set => settingsCommand = value;
        }

        public RelayCommand StartCommand
        {
            get => startCommand;
            set => startCommand = value;
        }

        public MainViewModel()
        {
            settings = new Settings();
            //client = new TClient(settings);
            //Текущий класс (MainViewModel) подписался на событие класса TClient и предоставил собственный обработчик
            //это необходимо для обновления привязок в MainView на основе изменений в TClient
            //client.PropertyChanged += OnTClientChanged;
            window = new WindowService();
            SettingsCommand = new RelayCommand(ShowSettings);
            StartCommand = new RelayCommand(StartWorking);
        }

        private void OnTClientChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Log"))
                OnPropertyChanged("GetLog");
        }

        public string GetLog1
        {
            get => client.Log;
        }

        private void ShowSettings(object obj)
        {
            window.ShowWindow(settings, newSettings =>
            {
                settings = newSettings;
            });
        }

        private void StartWorking(object obj)
        {
            //Task.Factory.StartNew(() => client.ConnectAndCheckMsg());
            client.ConnectAndCheckMsg();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
