using System;

namespace PSQuickAssets.ViewModels;

public class UpdateViewModel
{
    public Version CurrentVersion { get; set; } = new Version();
    public Version NewVersion { get; set; } = new Version();
    public string Changelog { get; set; } = string.Empty;
}
