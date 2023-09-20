using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TL;
using TClientWPF.Interfaces;
using TClientWPF.Model;
using TClientWPF.Services;
using TClientWPF.Views;
using System.Windows;

namespace TClientWPF.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IWindow window;
        private IDialog dialogService;
        private Settings settings;
        private TClient client;
        private RelayCommand startCommand;
        private RelayCommand stopCommand;
        private RelayCommand settingsCommand;
        private string onlineImagePath, offlineImagePath;
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

        public RelayCommand StopCommand
        {
            get => stopCommand;
            set => stopCommand = value;
        }

        public string User => client?.User.first_name;

        public string Log => client?.Log;

        public string IsOnline => (client?.IsOnline ?? false) ? onlineImagePath : offlineImagePath;

        public int CountOfForwardedMsg => client?.CountOfForwardedMsg ?? 0;

        public MainViewModel()
        {
            
            dialogService = new DefaultDialogService();
            window = new WindowService();
            SettingsCommand = new RelayCommand(ShowSettings);
            StartCommand = new RelayCommand(StartWorking);
            StopCommand = new RelayCommand(StopWorking);
            onlineImagePath = "pack://application:,,,/Images/Online.png";
            offlineImagePath = "pack://application:,,,/Images/Offline.png";
        }

        private void OnTClientChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

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
            }
            else
            { 
                dialogService.ShowMessage("Ошибка! Нет настроек.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //if (client.IsOnline)
            //{
            //    dialogService.ShowMessage("Подключение уже было выполнено!.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}    
                
            try
            {
                client.Initialize();
                await client.ConnectAndCheckMsg();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                client.Dispose();
            }
        }

        private void StopWorking(object obj)
        {
            client.Dispose();
            dialogService.ShowMessage("Disconnected", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
