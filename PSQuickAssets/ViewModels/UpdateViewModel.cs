using PSQuickAssets.Resources;
using System;

namespace PSQuickAssets.ViewModels
{
    public class UpdateViewModel
    {
        public string WindowTitle { get; set; }

        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }
        public string Changelog { get; set; }

        public UpdateViewModel(Version currentVersion, Version newVersion, string changelog)
        {
            WindowTitle = $"{App.AppName } {Localization.Instance["Update"]}";

            CurrentVersion = currentVersion.ToString();
            NewVersion = newVersion.ToString();
            Changelog = changelog;
        }
    }
}
