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
        private PatternMatching patternMatching;
        private TClient client;
        private RelayCommand startCommand;
        private RelayCommand stopCommand;
        private RelayCommand settingsCommand;
        private RelayCommand hideWindowCommand;
        private RelayCommand showWindowCommand;
        private RelayCommand checkMsgHistoryCommand;
        private RelayCommand closeWindowCommand;
        private NotifyIconWrapper notifyIconWrapper;
        private WindowState windowState;
        private KeyValuePair<long, ChatBase> selectedChannelData;
        private bool showInTaskbar;
        private string onlineImagePath, offlineImagePath;
        private string iconPath;
        private bool isSettingsEnable, isConnectEnable, isDisconnectEnable, isCheckMsgHistoryEnable, isChecked;
        public event PropertyChangedEventHandler PropertyChanged;

        public KeyValuePair<long, ChatBase> SelectedChannelData
        {
            get => selectedChannelData;
            set 
            {
                selectedChannelData = value;
                client.ChannelID = selectedChannelData.Key;
                IsCheckMsgHistoryEnable = true;
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

        public RelayCommand CloseWindowCommand
        {
            get => closeWindowCommand;
            set => closeWindowCommand = value;
        }

        public RelayCommand CheckMsgHistoryCommand
        {
            get => checkMsgHistoryCommand;
            set => checkMsgHistoryCommand = value;
        }

        public RelayCommand HideWindowCommand
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

        public bool IsCheckMsgHistoryEnable
        {
            get => isCheckMsgHistoryEnable;
            set
            {
                isCheckMsgHistoryEnable = value;
                OnPropertyChanged();
            }
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

        public bool IsChecked
        {
            get => isChecked;
            set => isChecked = value;
        }

        public string RegexPattern
        {
            set
            {
                if (isChecked)
                {
                    string[] parts = value.Split(' ');
                    if (parts.Length >= 2)
                    {
                        patternMatching.Expression = "\\w*" + parts[0] + "\\w|" +
                                                     "\\w*" + parts[1] + "\\w*";
                    }
                }
                else patternMatching.Expression = "\\w*" + value + "\\w";
            }
        }

        public Dictionary<long, ChatBase> ChatsList
        {
            get => client?.ChatsList;
            set
            {
                client.ChatsList = value;
                OnPropertyChanged();
            }
        }

        public string User => client?.User.first_name;

        public string Log => client?.Log;

        public string IsOnline => (client?.IsOnline ?? false) ? onlineImagePath : offlineImagePath;

        public int CountOfForwardedMsg => client?.CountOfForwardedMsg ?? 0;

        public MainViewModel()
        {
            patternMatching = new PatternMatching();
            dialogService = new DefaultDialogService();
            window = new WindowService();
            SettingsCommand = new RelayCommand(ShowSettings);
            StartCommand = new RelayCommand(StartWorking);
            StopCommand = new RelayCommand(StopWorking);
            HideWindowCommand = new RelayCommand(HideWindow);
            CloseWindowCommand = new RelayCommand(CloseWindow);
            ShowWindowCommand = new RelayCommand(ShowWindow);
            CheckMsgHistoryCommand = new RelayCommand(CheckMessageHistory);


            onlineImagePath = "pack://application:,,,/Images/Online.png";
            offlineImagePath = "pack://application:,,,/Images/Offline.png";
            IsSettingsEnable = true;
            IsConnectEnable = false;
            IsDisconnectEnable = false;
            IsCheckMsgHistoryEnable = false;
            ShowInTaskbar = true;
        }

        private async void CheckMessageHistory()
        {
            try
            {
                await client.MonitorChannel();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dialogService.ShowMessage($"Сообщения в канале '{SelectedChannelData.Value.Title}' проверены, было переслано {client.CountOfForwardedMsg} сообщений.",
                                      "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
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

        //Сворачивание приложения
        private void HideWindow()
        {
            WindowState = WindowState.Minimized;
            ShowInTaskbar = true;
        }

        //Закрытие на крестик приложения
        private void CloseWindow()
        {
            if (notifyIconWrapper == null)
            {
                iconPath = "Images/TClient.ico";
                notifyIconWrapper = new NotifyIconWrapper(iconPath);
                notifyIconWrapper.ShowWindowRequested += (sender, e) => ShowWindow();
                notifyIconWrapper.ExitRequested += (sender, e) => CloseProgramm();
            }

            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void OnTClientChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

        private void ShowSettings()
        {
            if (settings == null)
                settings = new Settings();
                

            window.ShowSettingsWindow(settings, newSettings =>
            {
                settings = newSettings;
            });

            IsConnectEnable = true;
        }

        private async void StartWorking()
        {
            client = new TClient(settings, patternMatching);
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

            IsDisconnectEnable = false;
            IsConnectEnable = true;
            IsSettingsEnable = true;
            IsCheckMsgHistoryEnable = false;

            ChatsList = null;

            dialogService.ShowMessage("Отключено!", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
