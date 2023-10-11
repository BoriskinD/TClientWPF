using System;
using TClientWPF.Model;

namespace TClientWPF.Interfaces
{
    interface IWindow
    {
        void ShowSettingsWindow(Settings settings, Action<Settings> callback);
    }
}
