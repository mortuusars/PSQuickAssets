using MLogger;
using MLogger.LogWriters;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;

namespace PSQuickAssets
{
    internal class Program
    {
        public static Program Instance { get => _program; }
        private static Program _program;

        public static ILogger Logger { get; private set; }
        public static GlobalHotkeyRegistry GlobalHotkeyRegistry { get; private set; }
        public static ViewManager ViewManager { get; private set; }

        private Program(string[] args)
        {
            SetupLogging();

            GlobalHotkeyRegistry = new GlobalHotkeyRegistry();
            ViewManager = new ViewManager(new OpenDialogService());

            ViewManager.CreateAndShowMainWindow();
            RegisterGlobalHotkey(ConfigManager.Config.Hotkey);

            new Update.Update().CheckUpdatesAsync();
        }

        public static Program Init(string[] args)
        {
            Program program = new Program(args);
            _program = program;
            return program;
        }

        private void SetupLogging()
        {
            Logger = new Logger(LogLevel.Debug);
            try
            {
                Directory.CreateDirectory(Path.Combine(App.AppDataFolder, "logs"));
                Logger.Loggers.Add(new FileWriter("logs\\log.txt"));
                Logger.Loggers.Add(new DebugWriter());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log setup failed:\n\n" + ex);
            }
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
    }
}
