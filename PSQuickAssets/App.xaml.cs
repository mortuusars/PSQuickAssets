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
        public static Version Version { get; private set; } = new Version(GetVersionFromAssembly());
        public static string Build { get; private set; } = BuildTime.GetLinkerTime(Assembly.GetEntryAssembly()!).ToString("yyMMddHHmmss");

        public static string AppDataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.AppName);

        internal static Config Config { get; private set; }
        internal static GlobalHotkeys? GlobalHotkeys { get; private set; }
        internal static WindowManager? WindowManager { get; private set; }
        internal static INotificationService? NotificationService { get; private set; }

        public static ILogger? Logger { get; private set; }

        public static TaskbarIcon? _taskBarIcon;

        public App()
        {
            DispatcherUnhandledException += CrashHandler.OnUnhandledException;
            ShutdownIfAlreadyOpen();
        }

        private static string GetVersionFromAssembly()
        {
            try
            {
                string info = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
                return info.Substring(0, info.IndexOf('+'));
            }
            catch (Exception)
            {
                return "99.99.99";
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetTooltipDelay(650);

            InitTaskbarIcon();
            NotificationService = new TaskbarNotificationService(_taskBarIcon!);

            Logger = new MLoggerSetup(NotificationService).CreateLogger();

            var configFileHandler = new JsonFileConfigHandler("config.json", Logger);
            Config = new Config(configFileHandler, Logger, saveOnPropertyChanged: true).Load<Config>();

            WindowManager = new WindowManager(NotificationService);
            WindowManager.CreateAndShowMainWindow();

            GlobalHotkeys = new GlobalHotkeys(new WindowInteropHelper(WindowManager.MainWindow).Handle, NotificationService, Logger);
            GlobalHotkeys.HotkeyActions.Add(HotkeyUse.ToggleMainWindow, () => WindowManager.ToggleMainWindow());
            GlobalHotkeys.Register(MGlobalHotkeys.Hotkey.FromString(Config.ShowHideWindowHotkey), HotkeyUse.ToggleMainWindow);

            new Update.Update().CheckUpdatesAsync();
        }        

        protected override void OnExit(ExitEventArgs e)
        {
            Config?.Save();

            GlobalHotkeys?.Dispose();
            WindowManager?.CloseMainWindow();
            _taskBarIcon?.Dispose();

            base.OnExit(e);
        }

        private void InitTaskbarIcon()
        {
            _taskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
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
                Shutdown();
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
