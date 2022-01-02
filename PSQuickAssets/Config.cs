using AsyncAwaitBestPractices;
using MLogger;
using Mortuus.Config;
using Mortuus.Config.Deserialization;
using Mortuus.Config.Serialization;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PSQuickAssets;

internal interface IConfig : INotifyPropertyChanged
{
    string ShowHideWindowHotkey { get; set; }
    double ThumbnailSize { get; set; }
    bool AlwaysOnTop { get; set; }
    bool AddMaskIfDocumentHasSelection { get; set; }
    bool CheckUpdates { get; set; }

    void Save();
}

internal class Config : ConfigBase, IConfig
{
    public string ShowHideWindowHotkey { get => _showHideWindowHotkey.Value; set => _showHideWindowHotkey.SetValue(value); }
    public double ThumbnailSize { get => _thumbnailSize.Value; set => _thumbnailSize.SetValue(value); }
    public bool AlwaysOnTop { get => _alwaysOnTop.Value; set => _alwaysOnTop.SetValue(value); }
    public bool AddMaskIfDocumentHasSelection { get => _addMaskIfDocumentHasSelection.Value; set => _addMaskIfDocumentHasSelection.SetValue(value); }
    public bool CheckUpdates { get => _checkUpdates.Value; set => _checkUpdates.SetValue(value); }

    private readonly ConfigProperty<string> _showHideWindowHotkey;
    private readonly ConfigProperty<double> _thumbnailSize;
    private readonly ConfigProperty<bool> _alwaysOnTop;
    private readonly ConfigProperty<bool> _addMaskIfDocumentHasSelection;
    private readonly ConfigProperty<bool> _checkUpdates;

    private ILogger? _logger;
    private static readonly string _configFilePath = Path.Combine(App.AppDataFolder, "config.json");

    public Config()
    {
        _showHideWindowHotkey = RegisterProperty(nameof(ShowHideWindowHotkey), "Ctrl + Alt + A");
        _thumbnailSize = RegisterProperty(nameof(ThumbnailSize), 60.0);
        _alwaysOnTop = RegisterProperty(nameof(AlwaysOnTop), true);
        _addMaskIfDocumentHasSelection = RegisterProperty(nameof(AddMaskIfDocumentHasSelection), true);
        _checkUpdates = RegisterProperty(nameof(CheckUpdates), true);

        Serializer = new JsonConfigSerializer(msg => _logger?.Error($"[Config] {msg}"))
        {
            JsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true }
        };

        SaveOnPropertyChanged = true;
    }

    public static Config Deserialize(ILogger logger)
    {
        string json = "";
        try { json = File.ReadAllText(_configFilePath); }
        catch (System.Exception ex) { logger.Error($"[Config] Failed to read '{_configFilePath}': {ex.Message}"); }

        var newConfig = new JsonByPropertyDeserializer(msg => logger.Error(msg)).Deserialize<Config>(json) ?? new Config();
        newConfig._logger = logger;
        return newConfig;
    }

    public override void Save(string? serializedConfig)
    {
        if (serializedConfig is not null)
            SaveAsync(serializedConfig).SafeFireAndForget(ex => _logger?.Error($"[Config] Saving failed: {ex.Message}"));
    }
    private async Task SaveAsync(string serializedConfig) => await File.WriteAllTextAsync(_configFilePath, serializedConfig);

    internal void SetLogger(ILogger logger) => _logger = logger;
}
