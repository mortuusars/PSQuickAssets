using AsyncAwaitBestPractices;
using Mortuus.Config;
using Mortuus.Config.Deserialization;
using Mortuus.Config.Serialization;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using Serilog;
using Serilog.Events;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PSQuickAssets;

internal interface IConfig : INotifyPropertyChanged
{
    string ShowHideWindowHotkey { get; set; }
    bool MinimizeWindowInsteadOfHiding { get; set; }
    public bool HideWindowWhenAddingAsset { get; set; }
    double ThumbnailSize { get; set; }

    ThumbnailQuality ThumbnailQuality { get; set; }
    bool AlwaysOnTop { get; set; }
    bool HideWindowIfClickedOutside { get; set; }
    bool CheckUpdates { get; set; }

    bool AddMaskIfDocumentHasSelection { get; set; }
    bool UnlinkMask { get; set; }

    bool DebugMode { get; set; }

    void Save();
}

internal class Config : ConfigBase, IConfig
{
    // General
    public string ShowHideWindowHotkey { get => _showHideWindowHotkey.Value; set => _showHideWindowHotkey.SetValue(value); }
    public bool MinimizeWindowInsteadOfHiding { get => _minimizeWindowInsteadOfHiding.Value; set => _minimizeWindowInsteadOfHiding.SetValue(value); }
    public bool HideWindowWhenAddingAsset { get => _hideWindowWhenAddingAsset.Value; set => _hideWindowWhenAddingAsset.SetValue(value); }
    public double ThumbnailSize { get => _thumbnailSize.Value; set => _thumbnailSize.SetValue(value); }
    public bool AlwaysOnTop { get => _alwaysOnTop.Value; set => _alwaysOnTop.SetValue(value); }
    public bool HideWindowIfClickedOutside { get => _hideWindowIfClickedOutside.Value; set => _hideWindowIfClickedOutside.SetValue(value); }
    public bool CheckUpdates { get => _checkUpdates.Value; set => _checkUpdates.SetValue(value); }

    // Assets
    public bool AddMaskIfDocumentHasSelection { get => _addMaskIfDocumentHasSelection.Value; set => _addMaskIfDocumentHasSelection.SetValue(value); }
    public bool UnlinkMask { get => _unlinkMask.Value; set => _unlinkMask.SetValue(value); }

    // Experimental settings:
    public bool DebugMode { get => _debugMode.Value; set => _debugMode.SetValue(value); }
    public ThumbnailQuality ThumbnailQuality { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private readonly ConfigProperty<string> _showHideWindowHotkey;
    private readonly ConfigProperty<bool> _minimizeWindowInsteadOfHiding;
    private readonly ConfigProperty<bool> _hideWindowWhenAddingAsset;
    private readonly ConfigProperty<double> _thumbnailSize;
    private readonly ConfigProperty<bool> _alwaysOnTop;
    private readonly ConfigProperty<bool> _hideWindowIfClickedOutside;
    private readonly ConfigProperty<bool> _checkUpdates;

    private readonly ConfigProperty<bool> _addMaskIfDocumentHasSelection;
    private readonly ConfigProperty<bool> _unlinkMask;

    private readonly ConfigProperty<bool> _debugMode;

    private ILogger? _logger;
    private static readonly string _configFilePath = Path.Combine(Folders.AppData, "config.json");

    //TODO: Simplify config.

    public Config()
    {
        _showHideWindowHotkey = RegisterProperty(nameof(ShowHideWindowHotkey), "Ctrl + Alt + A");
        _minimizeWindowInsteadOfHiding = RegisterProperty(nameof(MinimizeWindowInsteadOfHiding), true);
        _hideWindowWhenAddingAsset = RegisterProperty(nameof(HideWindowWhenAddingAsset), true);
        _thumbnailSize = RegisterProperty(nameof(ThumbnailSize), 60.0);
        _alwaysOnTop = RegisterProperty(nameof(AlwaysOnTop), true);
        _hideWindowIfClickedOutside = RegisterProperty(nameof(HideWindowIfClickedOutside), true);
        _checkUpdates = RegisterProperty(nameof(CheckUpdates), true);

        _addMaskIfDocumentHasSelection = RegisterProperty(nameof(AddMaskIfDocumentHasSelection), true);
        _unlinkMask = RegisterProperty(nameof(UnlinkMask), true);

        _debugMode = RegisterProperty(nameof(DebugMode), false);

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

//TODO: Remove listener.
internal class ConfigChangeListener
{
    private readonly IConfig _config;
    private readonly ILogger _logger;

    public ConfigChangeListener(IConfig config, ILogger logger)
    {
        _config = config;
        _logger = logger;
    }

    public void Listen()
    {
        _config.PropertyChanged += _config_PropertyChanged;
    }

    private void _config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (nameof(IConfig.DebugMode).Equals(e.PropertyName))
        {
            Logging.LogLevelSwitch.MinimumLevel = _config.DebugMode ? LogEventLevel.Verbose : LogEventLevel.Information;
            _logger.Information($"[Logger] Changed minimum log level to '{Logging.LogLevelSwitch.MinimumLevel}'.");
        }
    }
}
