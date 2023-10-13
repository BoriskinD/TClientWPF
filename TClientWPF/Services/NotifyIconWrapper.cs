using System;
using System.Drawing;
using System.Windows.Forms;

namespace TClientWPF.Services
{
    class NotifyIconWrapper : IDisposable
    {
        private readonly NotifyIcon notifyIcon;

        public event EventHandler ShowWindowRequested;
        public event EventHandler ExitRequested;

        public NotifyIconWrapper(string iconPath)
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(iconPath),
                Visible = true,
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = "TelegramWatcher",
                BalloonTipText = "Программа свернута в трей. Нажмите ПКМ для вызова меню."
            };
            notifyIcon.ShowBalloonTip(3000);

            InitializeContextMenu();
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new();
            contextMenu.Items.Add("Открыть").Click += (sender, e) => ShowWindowRequested?.Invoke(this, EventArgs.Empty);
            contextMenu.Items.Add("Выход").Click += (sender, e) => ExitRequested?.Invoke(this, EventArgs.Empty);

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ShowWindowRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => notifyIcon.Dispose();
    }
}
