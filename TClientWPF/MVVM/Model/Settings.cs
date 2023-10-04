using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace TClientWPF.Model
{
    public class Settings
    {
        private string api_id;
        private string api_hash;
        private string phone_number;
        private long channelToWatch;
        private string regexPattern;

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

        [JsonIgnore]
        public long ObservedChannel
        {
            get => channelToWatch;
            set => channelToWatch = value;
        }

        public Settings()
        {
            //Api_id = "757557";
            //Api_hash = "brlkwjeh43";
            //Phone_Number = "+2222";
            //ObservedChannel = 3268472694;
            //RegexPattern = "dasd";
        }
    }
}
