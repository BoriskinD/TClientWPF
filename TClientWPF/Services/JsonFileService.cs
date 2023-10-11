using System.IO;
using Newtonsoft.Json;
using TClientWPF.Model;

namespace TClientWPF.Services
{
    public class JsonFileService : IFile
    {
        public Settings Open(string fileName)
        {
            string json = File.ReadAllText(fileName);
            Settings settings = JsonConvert.DeserializeObject<Settings>(json);
            return settings;
        }

        public void Save(string fileName, Settings currentSettings)
        {
            JsonSerializerSettings serializationSettings = new() { DefaultValueHandling = DefaultValueHandling.Ignore };
            string json = JsonConvert.SerializeObject(currentSettings, Formatting.Indented, serializationSettings);
            File.WriteAllText(fileName, json);
        }
    }
}
