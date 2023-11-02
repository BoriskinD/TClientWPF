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
using System.Text.RegularExpressions;

namespace TClientWPF.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Logger logger;
        private IWindow window;
        private IDialog dialogService;
        private Settings settings;
        private PatternMatching patternMatching;
        private TClient telegramClientWrapper;
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
        private bool isSettingsEnable, isConnectEnable, isDisconnectEnable, isCheckMsgHistoryEnable, isDoubleStatement;
        private bool isAutoreconnect, isAutoreconnectEnable;
        private bool checkingHistoryInProgress;
        public event PropertyChangedEventHandler PropertyChanged;


        public KeyValuePair<long, ChatBase> SelectedChannelData
        {
            get => selectedChannelData;
            set
            {
                selectedChannelData = value;
                telegramClientWrapper.ChannelID = selectedChannelData.Key;
                if (!checkingHistoryInProgress)
                    IsCheckMsgHistoryButtonEnable = true;
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

        public bool IsCheckMsgHistoryButtonEnable
        {
            get => isCheckMsgHistoryEnable;
            set
            {
                isCheckMsgHistoryEnable = value;
                OnPropertyChanged();
            }
        }

        public bool IsSettingsButtonEnable
        {
            get => isSettingsEnable;
            set
            {
                isSettingsEnable = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnectButtonEnable
        {
            get => isConnectEnable;
            set
            {
                isConnectEnable = value;
                OnPropertyChanged();
            }
        }

        public bool IsDisconnectButtonEnable
        {
            get => isDisconnectEnable;
            set
            {
                isDisconnectEnable = value;
                OnPropertyChanged();
            }
        }

        public bool IsAutoreconnect
        {
            get => isAutoreconnect;
            set
            {
                isAutoreconnect = value;
                if (telegramClientWrapper != null)
                    telegramClientWrapper.Autoreconnect = IsAutoreconnect;
            }
        }

        public bool IsDoubleStatementChecked
        {
            get => isDoubleStatement;
            set => isDoubleStatement = value;
        }

        public bool IsAutoreconnectCheckBoxEnable
        {
            get => isAutoreconnectEnable;
            set
            {
                isAutoreconnectEnable = value;
                OnPropertyChanged();
            }
        }

        public string RegexPattern
        {
            set
            {
                if (Regex.IsMatch(value, "^[а-я А-Я a-z A-Z 0-9]*$"))
                {
                    if (IsDoubleStatementChecked)
                    {
                        string[] parts = value.Split(' ');
                        if (parts.Length >= 2)
                            patternMatching.Expression = parts[0] + "|" + parts[1];
                    }
                    else patternMatching.Expression = value;
                }
            }
        }

        public Dictionary<long, ChatBase> ChatsList
        {
            get => telegramClientWrapper?.ChatsList;
            set
            {
                telegramClientWrapper.ChatsList = value;
                OnPropertyChanged();
            }
        }

        public string User => telegramClientWrapper.User?.first_name;

        public string Log => logger.Log;

        public string IsOnline => (telegramClientWrapper?.IsOnline ?? false) ? onlineImagePath : offlineImagePath;

        public int CountOfGeneralFWDMessages => telegramClientWrapper?.CountOfGeneralFWDMessages ?? 0;

        public MainViewModel()
        {
            logger = Logger.GetInstance();
            logger.PropertyChanged += (sender, e) => OnPropertyChanged("Log");
            patternMatching = new PatternMatching();
            telegramClientWrapper = new TClient();
            telegramClientWrapper.PropertyChanged += OnTClientChanged;
            telegramClientWrapper.ConnectionDropped += OnConnectionDropped;
            telegramClientWrapper.ConnectionRestored += (sender, e) => { IsDisconnectButtonEnable = true; };
            dialogService = new DefaultDialogService();
            window = new WindowService();
            SettingsCommand = new RelayCommand(ShowSettings);
            StartCommand = new RelayCommand(Connect);
            StopCommand = new RelayCommand(Disconnect);
            HideWindowCommand = new RelayCommand(HideWindow);
            CloseWindowCommand = new RelayCommand(CloseWindow);
            ShowWindowCommand = new RelayCommand(ShowWindow);
            CheckMsgHistoryCommand = new RelayCommand(CheckHistory);

            onlineImagePath = "pack://application:,,,/Images/Online.png";
            offlineImagePath = "pack://application:,,,/Images/Offline.png";
            IsSettingsButtonEnable = true;
            IsAutoreconnectCheckBoxEnable = true;
            ShowInTaskbar = true;
            IsConnectButtonEnable = false;
            IsDisconnectButtonEnable = false;
            IsCheckMsgHistoryButtonEnable = false;
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

        private void ShowSettings()
        {
            if (settings == null)
                settings = new Settings();

            window.ShowSettingsWindow(settings, newSettings => { settings = newSettings; });
            IsConnectButtonEnable = true;
            logger.AddText($"WARNING: Настройки загружены.");
        }

        private async void CheckHistory()
        {
            checkingHistoryInProgress = true;
            IsCheckMsgHistoryButtonEnable = false;
            string channelName = SelectedChannelData.Value.Title;

            logger.AddText($"WARNING: Начинаем проверку истории сообщений в канале {channelName}...");
            try
            {
                await telegramClientWrapper.CheckHistory();
            }
            catch (Exception ex)
            {
                logger.AddText($"ERROR: Ошибка проверки истории сообщений - {ex.Message}");
                dialogService.ShowMessage(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                checkingHistoryInProgress = false;
                if (telegramClientWrapper.IsOnline)
                    IsCheckMsgHistoryButtonEnable = true;
            }

            logger.AddText($"INFO: История сообщений успешно проверена. Найдено {telegramClientWrapper.CountOfHistoryFWDMessages} сообщений.");
            dialogService.ShowMessage($"Сообщения в канале \"{channelName}\" проверены, было переслано {telegramClientWrapper.CountOfHistoryFWDMessages} сообщений.",
                                      "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);

            telegramClientWrapper.CountOfHistoryFWDMessages = 0;
        }

        private async void Connect()
        {
            telegramClientWrapper.Settings = settings;
            telegramClientWrapper.PatternMatching = patternMatching;
            IsConnectButtonEnable = false;

            logger.AddText($"WARNING: Пытаемся подключиться к серверам Telegram...");
            try
            {
                telegramClientWrapper.Initialize();
                await telegramClientWrapper.Connect();
                await telegramClientWrapper.GetUserChats();
            }
            catch (Exception ex)
            {
                IsConnectButtonEnable = true;
                logger.AddText($"ERROR: Ошибка подключения - {ex.Message}.");
                dialogService.ShowMessage(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                telegramClientWrapper.Dispose();
                return;
            }
            logger.AddText($"INFO: Подключено!");
            logger.AddText($"INFO: Мониторим сообщения...");

            checkingHistoryInProgress = false;
            IsSettingsButtonEnable = false;
            IsDisconnectButtonEnable = true;
            IsAutoreconnectCheckBoxEnable = true;
        }

        private void OnConnectionDropped(object sender, EventArgs e)
        {
            logger.AddText($"WARNING: Проблемы с интернет соединением. ВЫ были отключены от Telegram.");
            ChatsList = null;
            if (!IsAutoreconnect)
            {
                IsAutoreconnectCheckBoxEnable = false;
                IsConnectButtonEnable = true;
                IsSettingsButtonEnable = true;
                IsDisconnectButtonEnable = false;
                IsCheckMsgHistoryButtonEnable = false;
            }
        }

        private void Disconnect()
        {
            logger.AddText($"INFO: Отключаемся от сервера...");
            telegramClientWrapper.Dispose();
            IsDisconnectButtonEnable = false;
            IsCheckMsgHistoryButtonEnable = false;
            IsConnectButtonEnable = true;
            IsSettingsButtonEnable = true;
            ChatsList = null;
            logger.AddText($"INFO: Отключено!");
            dialogService.ShowMessage("Отключено!", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnTClientChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
