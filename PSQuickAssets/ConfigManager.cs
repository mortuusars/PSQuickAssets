using System;
using System.IO;
using System.Text.Json;

namespace PSQuickAssets
{
    public static class ConfigManager
    {
        private const string CONFIG_FILENAME = "config.json";
        private static readonly string configFilePath = Environment.CurrentDirectory + $"/{CONFIG_FILENAME}";

        public static Config Config { get; set; } = Read();

        public static string GetFolder()
        {
            return Config.Folder;
        }

        public static void Write()
        {
            string jsonString = JsonSerializer.Serialize(Config);
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
                return new Config();
            }
        }

    }
}
