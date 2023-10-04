using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TClientWPF.Interfaces;
using TClientWPF.Model;
using TClientWPF.Services;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using TL;

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
        private NotifyIconWrapper notifyIconWrapper;
        private WindowState windowState;
        private KeyValuePair<long, ChatBase> selectedChannelData;
        private bool showInTaskbar;
        private string onlineImagePath, offlineImagePath;
        private string iconPath;
        private string regexPattern;
        private bool isSettingsEnable, isConnectEnable, isDisconnectEnable;
        public event PropertyChangedEventHandler PropertyChanged;

        public KeyValuePair<long, ChatBase> SelectedChannelData
        {
            get => selectedChannelData;
            set 
            {
                selectedChannelData = value;
                settings.ObservedChannel = selectedChannelData.Key;
            }
        }

        public WindowState WindowState
        {
            get => windowState;
            set
            {
                windowState = value;
                OnPropertyChanged();
            }
        }

        public bool ShowInTaskbar
        {
            get => showInTaskbar;
            set
            {
                showInTaskbar = value;
                OnPropertyChanged();
            }
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

        public string RegexPattern
        {
            get => regexPattern;
            set
            {
                regexPattern = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<long, ChatBase> ChatsList => client?.ChatsList;

        public string User => client?.User.first_name;

        public string Log => client?.Log;

        public string IsOnline => (client?.IsOnline ?? false) ? onlineImagePath : offlineImagePath;

        public int CountOfForwardedMsg => client?.CountOfForwardedMsg ?? 0;

        public MainViewModel()
        {
            iconPath = "../../Images/TClient.ico";
            notifyIconWrapper = new NotifyIconWrapper(iconPath);
            notifyIconWrapper.ShowWindowRequested += (sender, e) => ShowWindow();
            notifyIconWrapper.ExitRequested += (sender, e) => CloseProgramm();

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
            ShowInTaskbar = true;
        }

        private void CloseProgramm()
        {
            notifyIconWrapper.Dispose();
            Application.Current.Shutdown();
        }

        private void ShowWindow()
        {
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
        }

        private void HideWindow(CancelEventArgs e)
        {
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void OnTClientChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

        private void ShowSettings()
        {
            if (settings == null)
                settings = new Settings();
                

            window.ShowWindow(settings, newSettings =>
            {
                settings = newSettings;
                RegexPattern = settings.RegexPattern;
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
                await client.LoginAndStartWorking();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
