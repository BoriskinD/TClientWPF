using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using TClientWPF.Model;
using TClientWPF.Services;

namespace TClientWPF.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private Settings settings;
        private IFile fileService;
        private IDialog dialogService;
        private bool twoStatements;

        public Settings Settings
        {
            get => settings;
            set
            {
                settings = value;
                //Событие PropertyChanged не желает вызываться при чтении данных из файла в класс Settings
                //оно всегда по какой-то причине равно null, я так и не разобрался почему, поэтому приходится в ручную вызывать обновление View
                //после измненения полей в Settings
                OnPropertyChanged("RegexPattern");
                OnPropertyChanged("Api_id");
                OnPropertyChanged("Api_hash");
                OnPropertyChanged("Phone_Number");
                OnPropertyChanged("ObservedChannel");
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
            fileService = new JsonFileService();
            dialogService = new DefaultDialogService();
        }

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand =>
                            saveCommand ?? (saveCommand = new RelayCommand(Save));
        
        private void Save(object obj)
        {
            try
            {
                if (dialogService.SaveFileDialog())
                {
                    fileService.Save(dialogService.FilePath, settings);
                    dialogService.ShowMessage("File Saved");
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message);
            } 
        }


        private RelayCommand openCommand;
        public RelayCommand OpenCommand => 
                            openCommand ?? (openCommand = new RelayCommand(Open));

        private void Open(object obj)
        {
            try
            {
                if (dialogService.OpenFileDialog())
                {
                    Settings = fileService.Open(dialogService.FilePath);
                    dialogService.ShowMessage("File opened");
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message);
            }
        }


        private RelayCommand checkCommand;
        public RelayCommand CheckBoxCheckCommand =>
                            checkCommand ?? (checkCommand = new RelayCommand(CheckChanged));

        private void CheckChanged(object obj)
        {
            twoStatements = (bool)obj;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private RelayCommand windowClosingCommand;
        public RelayCommand WindowClosingCommand =>
                            windowClosingCommand ?? (windowClosingCommand = new RelayCommand(OnWindowClosing));

        private void OnWindowClosing(object obj)
        {
            dialogService.ShowMessage("Data saved!");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
