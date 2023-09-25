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
        private bool isSettingsEnable, isConnectEnable, isDisconnectEnable;
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

        public bool IsSettingsEnable
        {
            get => isSettingsEnable;
            set
            {
                isSettingsEnable = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnectEnable
        {
            get => isConnectEnable;
            set
            {
                isConnectEnable = value;
                OnPropertyChanged();
            }
        }

        public bool IsDisconnectEnable
        {
            get => isDisconnectEnable;
            set
            {
                isDisconnectEnable = value;
                OnPropertyChanged();
            }
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
            IsSettingsEnable = true;
            IsConnectEnable = false;
            IsDisconnectEnable = false;
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

            IsConnectEnable = true;
        }

        private async void StartWorking(object obj)
        {
            client = new TClient(settings);
            client.PropertyChanged += OnTClientChanged;

            try
            {
                client.Initialize();
                await client.ConnectAndCheckMsg();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                client.Dispose();
                return;
            }

            IsConnectEnable = false;
            IsSettingsEnable = false;
            IsDisconnectEnable = true;
        }

        private void StopWorking(object obj)
        {
            client.Dispose();
            dialogService.ShowMessage("Отключено!", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);

            IsDisconnectEnable = false;
            IsConnectEnable = true;
            IsSettingsEnable = true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
