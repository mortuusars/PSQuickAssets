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
            Config config;
            
            try
            {
                string jsonString = File.ReadAllText(configFilePath);
                config = JsonSerializer.Deserialize<Config>(jsonString);
            }
            catch (Exception)
            {
                config = new Config()
                {
                    Directories = new List<string>(),
                    Hotkey = "Ctrl + Alt + F8",
                    CheckUpdates = true
                };
            }

            if (config.Hotkey is null)
                config = config with { Hotkey = "Ctrl + Alt + F8" };

            return config;
        }

    }
}
