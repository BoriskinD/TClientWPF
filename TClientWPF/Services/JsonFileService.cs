using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using TClientWPF.Model;

namespace TClientWPF.Services
{
    public class JsonFileService : IFile
    {
        public Settings Open(string fileName)
        {
            Settings settings;
            using (FileStream fs = new(fileName, FileMode.Open))
            {
                settings = JsonSerializer.Deserialize<Settings>(fs);
            }
            return settings;
        }

        public void Save(string fileName, Settings currentSettings)
        {
            using FileStream fs = new (fileName, FileMode.Create);
            JsonSerializer.Serialize(fs, currentSettings);
        }
    }
}
