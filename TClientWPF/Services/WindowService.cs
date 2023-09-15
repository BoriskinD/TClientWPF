using System;
using TClientWPF.Interfaces;
using TClientWPF.Model;
using TClientWPF.Views;

namespace TClientWPF.Services
{
    class WindowService : IWindow
    {
        public void ShowWindow(Settings settings, Action<Settings> callback)
        {
            SettingsView settingsView = new (settings);

            EventHandler eventHandler = null;
            eventHandler = (s, e) =>
            {
                callback(settingsView.settingsViewModel.Settings);
                settingsView.Closed -= eventHandler;
            };
            settingsView.Closed += eventHandler;

            settingsView.ShowDialog();
        }
    }
}
