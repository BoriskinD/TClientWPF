using System;
using TClientWPF.Model;

namespace TClientWPF.Interfaces
{
    interface IWindow
    {
        void ShowWindow(Settings settings, Action<Settings> callback);
    }
}
