using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            set
            {
                regexPattern = value;
            }
        }

        public string Api_id
        {
            get => api_id;
            set 
            {
                api_id = value;
            }
        }

        public string Api_hash
        {
            get => api_hash;
            set
            {
                api_hash = value;
            } 
        }

        public string Phone_Number
        {
            get => phone_number;
            set
            {
                phone_number = value;
            } 
        }

        public long ObservedChannel
        {
            get => channelToWatch;
            set
            {
                channelToWatch = value;
            } 
        }

        public Settings()
        {
            Api_id = "20413628";
            Api_hash = "754dab6a532440af28f8050d9617490d";
            Phone_Number = "+380714177989";
            ObservedChannel = 3268472694;
            RegexPattern = "dasd";
        }
    }
}
