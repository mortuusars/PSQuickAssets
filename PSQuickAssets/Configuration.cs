using AsyncAwaitBestPractices;
using PureConfig;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PSQuickAssets;

internal class Configuration : ConfigBase, IConfig
{
    public string ShowHideWindowHotkey { get => GetValue("Ctrl + Alt + A"); set => SetValue(value); }
    public bool MinimizeWindowInsteadOfHiding { get => GetValue(false); set => SetValue(value); }
    public bool HideWindowWhenAddingAsset { get => GetValue(true); set => SetValue(value); }
    public double ThumbnailSize { get => GetValue(60.0); set => SetValue(value); }
    public bool AlwaysOnTop { get => GetValue(true); set => SetValue(value); }
    public bool HideWindowIfClickedOutside { get => GetValue(true); set => SetValue(value); }
    public bool CheckUpdates { get => GetValue(true); set => SetValue(value); }
    
    public bool AddMaskIfDocumentHasSelection { get => GetValue(true); set => SetValue(value); }
    public bool UnlinkMask { get => GetValue(true); set => SetValue(value); }

    public bool DebugMode { get => GetValue(false); set => SetValue(value); }

    private static readonly string _configFilePath = Path.Combine(App.AppDataFolder, "config.json");
    private readonly ILogger _logger;

    public Configuration(ILogger logger)
    {
        _logger = logger;
        this.PropertyChanged += (s, e) => Save();
    }

    public void Load()
    {
        try
        {
            string json = File.ReadAllText(_configFilePath);
            Dictionary<string, object?> properties = new JsonConfigDeserializer().Deserialize<Configuration>(json);
            SetProperties(properties);
        }
        catch (Exception ex)
        {
            if (ex is JsonException)
                _logger.Error("Config was not loaded: {0}", ex);
            else
                _logger.Error("Config was not loaded: {0}", ex.Message);
        }
    }

    public void Save()
    {
        try
        {
            var serializer = new JsonConfigSerializer(new JsonSerializerOptions() { WriteIndented = true });
            string? json = this.Serialize(serializer);
            if (json is null)
            {
                _logger.Error("Config was not serialized properly. Serializer returned null.");
                return;
            }
            SaveAsync(json).SafeFireAndForget((ex) => _logger.Error("Config was not saved: {0}", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to save config: {0}", ex.Message);
        }
    }

    private async Task SaveAsync(string serializedConfig)
    {
        await File.WriteAllTextAsync(_configFilePath, serializedConfig);
    }
}
