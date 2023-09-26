using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using TClientWPF.ViewModel;
using System.IO;

namespace TClientWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = new Icon("../../Images/TClient.ico"),
            };
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            ShowInTaskbar = false;
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }
    }
}