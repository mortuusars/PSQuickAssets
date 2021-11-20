﻿using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class TaskBarViewModel
    {
        public string AppName { get; set; }

        public ICommand ShowWindowCommand { get; }
        public ICommand ToggleTerminalCommand { get; }

        public ICommand SettingsCommand { get; }
        public ICommand ExitCommand { get; }

        public TaskBarViewModel()
        {
            AppName = "PSQuickAssets " + App.Version.ToString();

            ShowWindowCommand = new RelayCommand(_ => App.WindowManager.ToggleMainWindow());
            ToggleTerminalCommand = new RelayCommand(_ => MLoggerSetup.ToggleTerminalWindow());
            SettingsCommand = new RelayCommand(_ => App.WindowManager.ShowSettingsWindow());
            ExitCommand = new RelayCommand(_ => App.Current.Shutdown());
        }
    }
}
