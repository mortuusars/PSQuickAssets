using MLogger;

namespace PSQuickAssets.Configuration;

public class Config : ConfigBase
{
    public string ShowHideWindowHotkey { get; private set; }
    public double ThumbnailSize { get; private set; }
    public bool AlwaysOnTop { get; private set; }
    public bool CheckUpdates { get; private set; }

    public bool AddMaskIfDocumentHasSelection { get; private set; }

    public Config(IConfigSaver configSaver, ILogger? logger) : base(configSaver, logger, true)
    {
        var configHandler = new JsonFileConfigHandler(logger);

        ShowHideWindowHotkey = "Ctrl + Alt + A";
        ThumbnailSize = 60;
        AlwaysOnTop = true;
        CheckUpdates = true;

        AddMaskIfDocumentHasSelection = true;

        Load<Config>(configHandler);
        Save();
    }
}
