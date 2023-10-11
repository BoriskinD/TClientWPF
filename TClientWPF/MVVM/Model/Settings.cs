using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace TClientWPF.Model
{
    public class Settings : INotifyPropertyChanged
    {
        private string api_id;
        private string api_hash;
        private string phone_number;
        private long channelToWatch;
        private string regexPattern;

        public event PropertyChangedEventHandler PropertyChanged;

        public string RegexPattern
        {
            get => regexPattern;
            set => regexPattern = value;
        }

        public string Api_id
        {
            get => api_id;
            set => api_id = value;
        }

        public string Api_hash
        {
            get => api_hash;
            set => api_hash = value;
        }

        public string Phone_Number
        {
            get => phone_number;
            set => phone_number = value;
        }

        public long ObservedChannel
        {
            get => channelToWatch;
            set
            {
                channelToWatch = value;
                OnPropertyChanged();
            } 
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
                     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
