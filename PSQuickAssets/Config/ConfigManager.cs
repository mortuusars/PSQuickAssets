using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace PSQuickAssets
{
    public static class ConfigManager
    {
        private const string CONFIG_FILENAME = "config.json";
        private static readonly string configFilePath = Environment.CurrentDirectory + $"/{CONFIG_FILENAME}";

        public static Config Config { get; set; } = Read();

        public static void Save()
        {
            string jsonString = JsonSerializer.Serialize(Config, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(configFilePath, jsonString);
        }

        private static Config Read()
        {
            try
            {
                string jsonString = File.ReadAllText(configFilePath);
                return JsonSerializer.Deserialize<Config>(jsonString);
            }
            catch (Exception)
            {
                return new Config()
                {
                    Directories = new List<string>(),
                    Hotkey = "Ctrl + Alt + F8",
                    CheckUpdates = true
                };
            }
        }

    }
}
