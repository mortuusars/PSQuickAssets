﻿using AsyncAwaitBestPractices;
using PSQA.Core;
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
    bool HideIconFromTaskbar { get; set; }
    public bool HideWindowWhenAddingAsset { get; set; }
    bool HideWindowIfClickedOutside { get; set; }
    double ThumbnailSize { get; set; }
    ThumbnailQuality ThumbnailQuality { get; set; }
    bool CheckUpdates { get; set; }
    bool DebugMode { get; set; }

    string Theme { get; set; }

    bool AddMaskToAddedLayer { get; set; }
    MaskMode MaskMode { get; set; }
    bool UnlinkMask { get; set; }
    bool ExecuteActionsAfterAdding { get; set; }
    PhotoshopAction[] ActionsAfterAdding { get; set; }

    void Save();
}

internal class Config : ConfigBase, IConfig
{
    //General
    public string ShowHideWindowHotkey { get => GetValue("Ctrl + Alt + A"); set => SetValue(value); }
    public bool AlwaysOnTop { get => GetValue(true); set => SetValue(value); }
    public bool HideIconFromTaskbar { get => GetValue(false); set => SetValue(value); }
    public bool HideWindowWhenAddingAsset { get => GetValue(true); set => SetValue(value); }
    public bool HideWindowIfClickedOutside { get => GetValue(true); set => SetValue(value); }
    public double ThumbnailSize { get => GetValue(60.0); set => SetValue(value); }
    public ThumbnailQuality ThumbnailQuality { get => GetValue(ThumbnailQuality.Medium); set => SetValue(value); }
    public bool CheckUpdates { get => GetValue(true); set => SetValue(value); }
    public bool DebugMode { get => GetValue(false); set => SetValue(value); }

    //Appearance
    public string Theme { get => GetValue(string.Empty); set => SetValue(value); }

    //Assets
    public bool AddMaskToAddedLayer { get => GetValue(true); set => SetValue(value); }
    public MaskMode MaskMode { get => GetValue(MaskMode.RevealSelection); set => SetValue(value); }
    public bool UnlinkMask { get => GetValue(true); set => SetValue(value); }
    public bool ExecuteActionsAfterAdding { get => GetValue(false); set => SetValue(value); }
    public PhotoshopAction[] ActionsAfterAdding { get => GetValue(Array.Empty<PhotoshopAction>()); set => SetValue(value); }

    public static string FilePath { get; } = Path.Combine(Folders.AppData, "config.json");
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
            string json = File.ReadAllText(FilePath);
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
            .WriteToFileAsync(FilePath).SafeFireAndForget((ex) => _logger.Error("Config was not saved: {0}", ex.Message));
    }
}