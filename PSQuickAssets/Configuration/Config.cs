using MLogger;

namespace PSQuickAssets.Configuration;

public class Config : BaseConfig
{
    public string ShowHideWindowHotkey { get; private set; }
    public bool CheckUpdates { get; private set; }

    public Config(IConfigHandler configHandler, ILogger logger, bool saveOnPropertyChanged) : base(configHandler, logger, saveOnPropertyChanged)
    {
        ShowHideWindowHotkey = "Ctrl + Alt + A";
        CheckUpdates = true;
    }
}
