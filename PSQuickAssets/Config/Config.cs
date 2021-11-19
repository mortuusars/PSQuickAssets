using System.Collections.Generic;

namespace PSQuickAssets;

public record Config
{
    public List<string> Directories { get; init; }
    public string Hotkey { get; init; }
    public bool CheckUpdates { get; init; }
}
