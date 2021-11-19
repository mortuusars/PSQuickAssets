﻿using System.Timers;
using System.Windows.Input;
using PropertyChanged;
using PSQuickAssets.Services.Hotkeys;
using PSQuickAssets.WPF;

namespace PSQuickAssets.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        public Hotkey GlobalHotkey { get; set; }
        public bool CheckUpdates { get; set; }

        public string SavedMessage { get; set; }

        public ICommand SaveCommand { get; set; }

        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(_ => ApplyNewSettings());

            GlobalHotkey = Program.GlobalHotkeyRegistry.HotkeyInfo;
            CheckUpdates = ConfigManager.Config.CheckUpdates;
        }

        private void ApplyNewSettings()
        {
            Program.Instance.RegisterGlobalHotkey(GlobalHotkey.ToString());
            ConfigManager.Config = ConfigManager.Config with { CheckUpdates = CheckUpdates};

            ConfigManager.Save();

            SavedMessage = "Saved";
            Timer timer = new Timer(1000);
            timer.Elapsed += (s, e) => { SavedMessage = ""; timer.Stop(); };
            timer.Start();
        }
    }
}
