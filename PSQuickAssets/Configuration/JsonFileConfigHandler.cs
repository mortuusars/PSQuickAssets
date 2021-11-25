using MLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace PSQuickAssets.Configuration
{
    public class JsonFileConfigHandler : IConfigHandler
    {
        private readonly string _filePath;
        private readonly ILogger? _logger;

        public JsonFileConfigHandler(string filePath, MLogger.ILogger? logger)
        {
            _filePath = filePath;
            _logger = logger;
        }

        public Dictionary<string, object> Load()
        {
            var jsonValues = new Dictionary<string, object>();

            JsonDocument doc;
            PropertyInfo[] configProperties;
            JsonElement.ObjectEnumerator jsonEnumerator;

            try
            {
                string json = File.ReadAllText(_filePath);
                doc = JsonDocument.Parse(json);
                configProperties = (typeof(Config)).GetProperties();
                jsonEnumerator = doc.RootElement.EnumerateObject();
            }
            catch (Exception ex)
            {
                _logger?.Error("[Config - Loading] - Failed to load json file:\n" + ex, ex);
                return jsonValues;
            }


            foreach (var item in jsonEnumerator)
            {
                try
                {
                    var propertyType = configProperties.Where(p => p.Name == item.Name).First().PropertyType;
                    var obj = JsonSerializer.Deserialize(item.Value.GetRawText(), propertyType);
                    jsonValues.Add(item.Name, obj!);
                }
                catch (Exception ex)
                {
                    _logger?.Error("[Config - Loading] - Failed to read and assign property from json:\n" + ex, ex);
                }                
            }

            return jsonValues;
        }

        public void Save<T>(T config) where T : ConfigBase
        {
            try
            {
                string json = JsonSerializer.Serialize(config, config.GetType(), new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception) { }
        }
    }
}
