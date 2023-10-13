using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using TClientWPF.Model;
using TClientWPF.Services;

namespace TClientWPF.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private Settings settings;
        private IFile fileService;
        private IDialog dialogService;
        private RelayCommand openCommand;
        private RelayCommand saveCommand;
        private RelayCommand<string> navigateUri;
        private RelayCommand closeWindowCommand;
        public Action CloseWindowAction;
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand CloseWindowCommand
        {
            get => closeWindowCommand;
            set => closeWindowCommand = value;
        }

        public RelayCommand SaveCommand
        {
            get => saveCommand;
            set => saveCommand = value;
        }

        public RelayCommand OpenCommand
        {
            get => openCommand;
            set => openCommand = value;
        }

        public RelayCommand<string> NavigateCommand
        {
            get => navigateUri;
            set => navigateUri = value;
        }

        public Settings Settings
        {
            get => settings;
            set
            {
                settings = value;
                UpdateView();
            }
        }

        public string Api_id
        {
            get => Settings.Api_id;
            set
            {
                Settings.Api_id = value;
                OnPropertyChanged();
            }
        }

        public string Api_hash
        {
            get => Settings.Api_hash;
            set
            {
                Settings.Api_hash = value;
                OnPropertyChanged();
            }
        }

        public string Phone_Number
        {
            get => Settings.Phone_Number;
            set
            {
                Settings.Phone_Number = value;
                OnPropertyChanged();
            }
        }

        public long ObservedChannel
        {
            get => Settings.ObservedChannel;
            set
            {
                Settings.ObservedChannel = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel(Settings settings)
        {
            Settings = settings;
            SaveCommand = new RelayCommand(Save);
            OpenCommand = new RelayCommand(Open);
            NavigateCommand = new RelayCommand<string>(NavigateUri);
            CloseWindowCommand = new RelayCommand(CloseWindow);
            fileService = new JsonFileService();
            dialogService = new DefaultDialogService();
        }

        private void CloseWindow() => CloseWindowAction?.Invoke();

        private void NavigateUri(string uri) => Process.Start(new ProcessStartInfo(uri));
                     
        private void Save()
        {
            try
            {
                if (dialogService.SaveFileDialog())
                {
                    fileService.Save(dialogService.FilePath, settings);
                    dialogService.ShowMessage("Файл сохранён", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Open()
        {
            try
            {
                if (dialogService.OpenFileDialog())
                {
                    Settings = fileService.Open(dialogService.FilePath);
                    dialogService.ShowMessage("Файл открыт", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateView()
        {
            OnPropertyChanged("RegexPattern");
            OnPropertyChanged("Api_id");
            OnPropertyChanged("Api_hash");
            OnPropertyChanged("Phone_Number");
            OnPropertyChanged("ObservedChannel");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
