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
                OnPropertyChanged();
            } 
        }

        //public string RegexPattern
        //{
        //    get => Settings.RegexPattern;
        //    set
        //    {
        //        if (twoStatements)
        //        {
        //            string[] parts = value.Split(' ');
        //            Settings.RegexPattern = @"\w*" + parts[0] + "\\w|" +
        //                                    "\\w*" + parts[1] + "\\w*";
        //        }
        //        else
        //        {
        //            Settings.RegexPattern = value;
        //        }
        //        OnPropertyChanged();
        //    }
        //}

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
                    fileService.Save(dialogService.FilePath, Settings);
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
