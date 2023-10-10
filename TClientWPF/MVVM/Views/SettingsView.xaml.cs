using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TClientWPF.Model;
using TClientWPF.Services;
using TClientWPF.ViewModels;

namespace TClientWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsViewModel settingsViewModel;

        public SettingsView(Settings settings)
        {
            InitializeComponent();
            settingsViewModel = new SettingsViewModel(settings);
            DataContext = settingsViewModel;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
