using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TClientWPF.Interfaces;
using TClientWPF.Model;
using TClientWPF.Services;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

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
        private RelayCommand<CancelEventArgs> hideWindowCommand;
        private RelayCommand showWindowCommand;
        private NotifyTrayIconWrapper notifyIcon;
        private WindowState windowState;
        private bool showInTaskbar;
        private string onlineImagePath, offlineImagePath;
        private string iconPath;
        private bool isSettingsEnable, isConnectEnable, isDisconnectEnable;
        public event PropertyChangedEventHandler PropertyChanged;

        public WindowState WindowState
        {
            get => windowState;
            set
            {
                //ShowInTaskbar = true;
                windowState = value;
                ShowInTaskbar = value != WindowState.Minimized;
            }
        }

        public bool ShowInTaskbar
        {
            get => showInTaskbar;
            set => showInTaskbar = value;
        }

        public RelayCommand<CancelEventArgs> HideWindowCommand
        {
            get => hideWindowCommand;
            set => hideWindowCommand = value;
        }

        public RelayCommand ShowWindowCommand
        {
            get => showWindowCommand;
            set => showWindowCommand = value;
        }

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
            iconPath = "../../Images/TClient.ico";
            notifyIcon = new NotifyTrayIconWrapper(iconPath);
            //notifyIcon.HideWindowRequested += (sender, e) => HideWindow(null);
            //notifyIcon.ShowWindowRequested += (sender, e) => ShowWindow(null);

            dialogService = new DefaultDialogService();
            window = new WindowService();
            SettingsCommand = new RelayCommand(ShowSettings);
            StartCommand = new RelayCommand(StartWorking);
            StopCommand = new RelayCommand(StopWorking);
            HideWindowCommand = new RelayCommand<CancelEventArgs>(HideWindow);
            ShowWindowCommand = new RelayCommand(ShowWindow);
            

            onlineImagePath = "pack://application:,,,/Images/Online.png";
            offlineImagePath = "pack://application:,,,/Images/Offline.png";
            IsSettingsEnable = true;
            IsConnectEnable = false;
            IsDisconnectEnable = false;
        }

        private void ShowWindow()
        {
            //Application.Current.MainWindow.Visibility = Visibility.Visible;
            
        }

        private void HideWindow(CancelEventArgs e)
        {
            //Application.Current.MainWindow.Visibility = Visibility.Hidden;
            //ShowInTaskbar = false;
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            //Hide();
            //base.OnClosing(e);
        }

        private void OnTClientChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

        private void ShowSettings()
        {
            if (settings == null)
                settings = new Settings();

            window.ShowWindow(settings, newSettings =>
            {
                settings = newSettings;
            });

            IsConnectEnable = true;
        }

        private async void StartWorking()
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

        private void StopWorking()
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
