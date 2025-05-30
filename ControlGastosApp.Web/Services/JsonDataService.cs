using System.Text.Json;
using System.Text.Json.Serialization;

namespace ControlGastosApp.Web.Services
{
    public class JsonDataService : IJsonDataService
    {
        private readonly string _jsonFilePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private Dictionary<string, object> _data;

        public JsonDataService(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            LoadFile();
        }

        private void LoadFile()
        {
            if (File.Exists(_jsonFilePath))
            {
                var jsonString = File.ReadAllText(_jsonFilePath);
                _data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, _jsonOptions);
            }
            else
            {
                _data = new Dictionary<string, object>();
            }
        }

        private void SaveFile()
        {
            var jsonString = JsonSerializer.Serialize(_data, _jsonOptions);
            File.WriteAllText(_jsonFilePath, jsonString);
        }

        public List<T> LoadData<T>(string property) where T : class
        {
            if (_data.TryGetValue(property, out var value))
            {
                var jsonElement = (JsonElement)value;
                return JsonSerializer.Deserialize<List<T>>(jsonElement.GetRawText(), _jsonOptions);
            }
            return new List<T>();
        }

        public void SaveData<T>(List<T> data, string property) where T : class
        {
            _data[property] = data;
            SaveFile();
        }
    }
} 