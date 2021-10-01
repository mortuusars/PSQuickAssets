using Hardcodet.Wpf.TaskbarNotification;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace PSQuickAssets
{
    public partial class App : Application
    {
        public static Version Version { get; } = new Version("1.2.0");

        public static GlobalHotkeyRegistry GlobalHotkeyRegistry { get; private set; }
        public static ViewManager ViewManager { get; private set; }

        private TaskbarIcon _taskBarIcon;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (IsAnotherInstanceOpen())
            {
                MessageBox.Show("Another instance of PSQuickAssets is already running", "PSQuickAssets", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
            }

            GlobalHotkeyRegistry = new GlobalHotkeyRegistry();
            ViewManager = new ViewManager(new OpenDialogService());

            InitTaskbarIcon();
            SetTooltipDelay(650);

            ViewManager.CreateAndShowMainWindow();
            RegisterGlobalHotkey(ConfigManager.Config.Hotkey);

            new Update.Update().CheckUpdatesAsync();
        }

        public void RegisterGlobalHotkey(string hotkey)
        {
            IntPtr mainWindowHandle = new WindowInteropHelper(ViewManager.MainView).Handle;

            if (GlobalHotkeyRegistry.Register(new Hotkey(hotkey), mainWindowHandle, OnGlobalHotkeyPressed, out string errorMessage))
            {
                ConfigManager.Config = ConfigManager.Config with { Hotkey = GlobalHotkeyRegistry.HotkeyInfo.ToString() };
                ConfigManager.Save();
            }
            else
                MessageBox.Show(errorMessage);
        }

        private void OnGlobalHotkeyPressed() => ViewManager.ToggleMainWindow();

        private bool IsAnotherInstanceOpen()
        {
            var current = Process.GetCurrentProcess();

            foreach (var process in Process.GetProcessesByName(current.ProcessName))
            {
                if (process.Id != current.Id && process.MainModule.FileName == current.MainModule.FileName)
                    return true;
            }

            return false;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ConfigManager.Save();

            GlobalHotkeyRegistry.Dispose();
            ViewManager.CloseMainWindow();
            _taskBarIcon.Dispose();

            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"PSQuickAssets has crashed.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
            Shutdown();
        }

        private void InitTaskbarIcon()
        {
            _taskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
        }

        private static void SetTooltipDelay(int delayMS)
        {
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(delayMS));
        }
    }
}
