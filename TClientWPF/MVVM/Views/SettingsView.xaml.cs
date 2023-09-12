using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TClientWPF.Model;
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
            List<BindingExpression> bindings = GetBindings();
            settingsViewModel = new SettingsViewModel(settings);
            DataContext = settingsViewModel;
        }

        private List<BindingExpression> GetBindings()
        {
            List<BindingExpression> textBoxBindings = new()
            {
                BindingOperations.GetBindingExpression(ApiId, TextBox.TextProperty),
                BindingOperations.GetBindingExpression(ApiHash, TextBox.TextProperty),
                BindingOperations.GetBindingExpression(PhoneNumber, TextBox.TextProperty),
                BindingOperations.GetBindingExpression(Channel, TextBox.TextProperty),
                BindingOperations.GetBindingExpression(WTSeek, TextBox.TextProperty)
            };
            return textBoxBindings;
        }
    }
}
