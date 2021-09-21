using System;

namespace PSQuickAssets.ViewModels
{
    public class UpdateViewModel
    {
        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }
        public string Changelog { get; set; }

        public UpdateViewModel(Version currentVersion, Version newVersion, string changelog)
        {
            CurrentVersion = currentVersion.ToString();
            NewVersion = newVersion.ToString();
            Changelog = changelog;
        }
    }
}
