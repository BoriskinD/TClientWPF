using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
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
        private RelayCommand checkBoxCheckCommand;
        private RelayCommand navigateUri;
        private bool twoStatements;
        public event PropertyChangedEventHandler PropertyChanged;

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

        public RelayCommand CheckBoxCheckCommand
        {
            get => checkBoxCheckCommand;
            set => checkBoxCheckCommand = value;
        }

        public RelayCommand NavigateCommand
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

        public string RegexPattern
        {
            get => Settings.RegexPattern;
            set
            {
                if (twoStatements)
                {
                    string[] parts = value.Split(' ');
                    Settings.RegexPattern = @"\w*" + parts[0] + "\\w|" +
                                            "\\w*" + parts[1] + "\\w*";
                }
                else
                {
                    Settings.RegexPattern = value;
                }
                OnPropertyChanged();
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
            CheckBoxCheckCommand = new RelayCommand(CheckChanged);
            NavigateCommand = new RelayCommand(NavigateUri);
            fileService = new JsonFileService();
            dialogService = new DefaultDialogService();
        }

        private void NavigateUri(object obj)
        {
            if (obj is string uri)
                Process.Start(new ProcessStartInfo(uri));
        }
                     
        private void Save(object obj)
        {
            try
            {
                if (dialogService.SaveFileDialog())
                {
                    fileService.Save(dialogService.FilePath, settings);
                    dialogService.ShowMessage("File Saved", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Open(object obj)
        {
            try
            {
                if (dialogService.OpenFileDialog())
                {
                    Settings = fileService.Open(dialogService.FilePath);
                    dialogService.ShowMessage("File opened", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckChanged(object obj)
        {
            twoStatements = (bool)obj;
        }

        //private RelayCommand windowClosingCommand;
        //public RelayCommand WindowClosingCommand =>
        //                    windowClosingCommand ?? (windowClosingCommand = new RelayCommand(OnWindowClosing));

        //private void OnWindowClosing(object obj)
        //{
        //    dialogService.ShowMessage("Data saved!");
        //}

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
