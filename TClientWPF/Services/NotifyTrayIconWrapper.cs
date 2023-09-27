using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TClientWPF.Services
{
    class NotifyTrayIconWrapper
    {
        private readonly NotifyIcon notifyIcon;

        public event EventHandler ShowWindowRequested;
        public event EventHandler HideWindowRequested;
        public event EventHandler ExitRequested;

        public NotifyTrayIconWrapper(string iconPath)
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(iconPath),
                Visible = true
            };

            InitializeContextMenu();
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new();
            contextMenu.Items.Add("Открыть").Click += (sender, e) => ShowWindowRequested?.Invoke(this, EventArgs.Empty);
            //contextMenu.Items.Add("Скрыть приложение").Click += (sender, e) => HideWindowRequested?.Invoke(this, EventArgs.Empty);
            contextMenu.Items.Add("Выход").Click += (sender, e) => ExitRequested?.Invoke(this, EventArgs.Empty);

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowWindowRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }
    }
}
