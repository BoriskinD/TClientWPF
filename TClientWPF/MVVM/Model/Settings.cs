namespace TClientWPF.Model
{
    public class Settings
    {
        private string api_id;
        private string api_hash;
        private string phone_number;

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
    }
}
