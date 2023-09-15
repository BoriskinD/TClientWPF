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
        private IDialog dialogService;
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
            dialogService = new DefaultDialogService();
            window = new WindowService();
            SettingsCommand = new RelayCommand(ShowSettings);
            StartCommand = new RelayCommand(StartWorking);
        }

        private void OnTClientChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Log")
                OnPropertyChanged("GetLog");
        }

        public string GetLog
        {
            get => client?.Log;
        }

        private void ShowSettings(object obj)
        {
            if (settings == null)
                settings = new Settings();

            window.ShowWindow(settings, newSettings =>
            {
                settings = newSettings;
            });
        }

        private async void StartWorking(object obj)
        {
            if (settings != null)
            {
                client = new TClient(settings);
                client.PropertyChanged += OnTClientChanged;
                await client.ConnectAndCheckMsg();
            }
            else
            {
                dialogService.ShowMessage("Ошибка! Нет настроек.");
                return;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
