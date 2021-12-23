using Hardcodet.Wpf.TaskbarNotification;
using MLogger;
using PSQuickAssets.Configuration;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public const string AppName = "PSQuickAssets";
        public static Version Version { get; private set; } = new Version(AppVersion.GetVersionFromAssembly());
        public static string Build { get; private set; } = AppVersion.GetLinkerTime(Assembly.GetEntryAssembly()!).ToString("yyMMddHHmmss");

        public static string AppDataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.AppName);

        public static TaskbarIcon TaskBarIcon { get => (TaskbarIcon)Current.FindResource("TaskBarIcon"); }

        internal static Config Config { get; private set; }
        internal static GlobalHotkeys? GlobalHotkeys { get; private set; }
        internal static WindowManager? WindowManager { get; private set; }
        internal static INotificationService? NotificationService { get; private set; }

        public static ILogger? Logger { get; private set; }


        public App()
        {
            DispatcherUnhandledException += CrashHandler.OnUnhandledException;
        }        

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetTooltipDelay(650);

            ShutdownIfAlreadyOpen();

            NotificationService = new TaskbarNotificationService(TaskBarIcon);

            Logger = new MLoggerSetup(NotificationService).CreateLogger();

            Config = CreateConfig();

            WindowManager = new WindowManager(NotificationService, Config);
            WindowManager.CreateAndShowMainWindow();

            GlobalHotkeys = new GlobalHotkeys(new WindowInteropHelper(WindowManager.MainWindow).Handle, NotificationService, Logger);
            GlobalHotkeys.HotkeyActions.Add(HotkeyUse.ToggleMainWindow, () => WindowManager.ToggleMainWindow());
            GlobalHotkeys.Register(MGlobalHotkeys.WPF.Hotkey.FromString(Config.ShowHideWindowHotkey), HotkeyUse.ToggleMainWindow);

            new Update.Update().CheckUpdatesAsync();
        }

        private static Config CreateConfig()
        {
            string cfgFilePath = "config.json";

            //TODO: All of this is pretty dirty. Should probably restructure it somehow.
            var configFileHandler = new JsonFileConfigHandler(cfgFilePath, Logger);
            var config = new Config(configFileHandler, Logger, saveOnPropertyChanged: true);

            if (!File.Exists(cfgFilePath))
                config.Save();

            config.Load<Config>(configFileHandler);

            return config;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Config?.Save();

            GlobalHotkeys?.Dispose();
            WindowManager?.CloseMainWindow();
            TaskBarIcon?.Dispose();

            base.OnExit(e);
        }

        private static void SetTooltipDelay(int delayMS)
        {
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(delayMS));
        }

        private void ShutdownIfAlreadyOpen()
        {
            Process current = Process.GetCurrentProcess();
            bool isAnotherOpen = Process.GetProcessesByName(current.ProcessName).Any(p => p.Id != current.Id);

            if (isAnotherOpen)
            {
                MessageBox.Show("Another instance of PSQuickAssets is already running.", "PSQuickAssets",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                App.Current.Shutdown();
                Environment.Exit(0); // Kill process if shutdown not worked. Probably only in debug.
            }
        }

        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string message = $"PSQuickAssets has crashed.\n\n{e.Exception}";

            try
            {
                Logger?.Fatal(message, e.Exception);
            }
            catch (Exception) { }

            MessageBox.Show(message, AppName, MessageBoxButton.OK, MessageBoxImage.Error);

            Shutdown();
        }
    }
}
