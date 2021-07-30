﻿using System;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using PSQuickAssets.Infrastructure;
using PSQuickAssets.ViewModels;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public static readonly Version Version = new Version("1.0.0");
        public static readonly ViewManager ViewManager = new ViewManager();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ViewManager.CreateAndShowMainView();

            CheckUpdates();
        }

        private async void CheckUpdates()
        {
            var update = await new Update.UpdateChecker().CheckAsync();
            if (update.updateAvailable)
            {
                string message = $"New version available. Visit https://github.com/mortuusars/PSQuickAssets/releases/latest to download.\n\n" +
                    $"Version: {update.versionInfo.Version}\nChangelog: {update.versionInfo.Description}";
                MessageBox.Show(message, "PSQuickAssets Update", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var taskbaricon = (TaskbarIcon)FindResource("TaskBarIcon");
            taskbaricon.DataContext = new TaskBarViewModel();
            //taskbaricon.ShowBalloonTip("PSQuickAssets", "Running", BalloonIcon.Info);
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(650));
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ((TaskbarIcon)FindResource("TaskBarIcon")).Dispose();

            ConfigManager.Write();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"PSQuickAssets has crashed.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
            Shutdown();
        }
    }
}
