using AsyncAwaitBestPractices;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using PureConfig;
using Serilog;
using Serilog.Events;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace PSQuickAssets;

internal interface IConfig : INotifyPropertyChanged
{
    string ShowHideWindowHotkey { get; set; }
    bool AlwaysOnTop { get; set; }
    bool MinimizeWindowInsteadOfHiding { get; set; }
    bool HideWindowIfClickedOutside { get; set; }
    public bool HideWindowWhenAddingAsset { get; set; }
    double ThumbnailSize { get; set; }
    ThumbnailQuality ThumbnailQuality { get; set; }
    bool CheckUpdates { get; set; }

    bool AddMaskIfDocumentHasSelection { get; set; }
    bool UnlinkMask { get; set; }

    bool DebugMode { get; set; }

    void Save();
}

internal class Config : ConfigBase, IConfig
{
    //General
    public string ShowHideWindowHotkey { get => GetValue("Ctrl + Alt + A"); set => SetValue(value); }
    public bool AlwaysOnTop { get => GetValue(true); set => SetValue(value); }
    public bool MinimizeWindowInsteadOfHiding { get => GetValue(false); set => SetValue(value); }
    public bool HideWindowWhenAddingAsset { get => GetValue(true); set => SetValue(value); }
    public bool HideWindowIfClickedOutside { get => GetValue(true); set => SetValue(value); }
    public double ThumbnailSize { get => GetValue(60.0); set => SetValue(value); }
    public ThumbnailQuality ThumbnailQuality { get => GetValue(ThumbnailQuality.Medium); set => SetValue(value); }
    public bool CheckUpdates { get => GetValue(true); set => SetValue(value); }

    //Assets
    public bool AddMaskIfDocumentHasSelection { get => GetValue(true); set => SetValue(value); }
    //TODO: Mask mode.
    public bool UnlinkMask { get => GetValue(true); set => SetValue(value); }

    //Experimental
    public bool DebugMode { get => GetValue(false); set => SetValue(value); }

    private static readonly string _configFilePath = Path.Combine(Folders.AppData, "config.json");
    private readonly ILogger _logger;

    public Config(ILogger logger)
    {
        _logger = logger;
        this.PropertyChanged += (s, e) =>
        {
            Save();

            if (e.PropertyName?.Equals(nameof(DebugMode)) is true)
            {
                Logging.LogLevelSwitch.MinimumLevel = DebugMode ? LogEventLevel.Verbose : LogEventLevel.Information;
                _logger.Information($"[Logger] Changed minimum log level to '{Logging.LogLevelSwitch.MinimumLevel}'.");
            }
        };

        Load();
    }

    public void Load()
    {
        try
        {
            string json = File.ReadAllText(_configFilePath);
            Dictionary<string, object?> properties = new JsonConfigDeserializer().Deserialize<Config>(json);
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
        this.Serialize(new JsonConfigSerializer(new JsonSerializerOptions() { WriteIndented = true }))?
            .WriteToFileAsync(_configFilePath).SafeFireAndForget((ex) => _logger.Error("Config was not saved: {0}", ex.Message));
    }
}