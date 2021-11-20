﻿using MLogger;
using MLogger.LogWriters;
using MLogger.Terminal;
using MLogger.Terminal.Models;
using MLogger.Terminal.Views;
using PSQuickAssets.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace PSQuickAssets
{
    internal class Program
    {
        public static Program Instance { get => _program; }
        private static Program _program;

        public static GlobalHotkeys GlobalHotkeys { get; private set; }
        public static WindowManager WindowManager { get; private set; }

        public static ILogger Logger { get; private set; }
        private static Terminal _terminal;

        private Program(string[] args)
        {
            SetupLogging();

            WindowManager = new WindowManager();
            WindowManager.CreateAndShowMainWindow();

            GlobalHotkeys = new GlobalHotkeys(WindowManager, Logger);
            GlobalHotkeys.HotkeyActions.Add(HotkeyUse.ToggleMainWindow, () => WindowManager.ToggleMainWindow());
            GlobalHotkeys.Register(MGlobalHotkeys.Hotkey.FromString(ConfigManager.Config.Hotkey), HotkeyUse.ToggleMainWindow);

            new Update.Update().CheckUpdatesAsync();
        }

        public static Program Init(string[] args)
        {
            Program program = new Program(args);
            _program = program;
            return program;
        }

        public static void ToggleTerminalWindow()
        {
            var terminalWindow = App.Current.Windows.OfType<TerminalWindow>().FirstOrDefault();

            if (terminalWindow is not null)
                terminalWindow.Close();
            else
                _terminal.ShowWindow();
        }

        private void SetupLogging()
        {
            Logger = new Logger(LogLevel.Debug);
            try
            {
                string logsFolder = Path.Combine(App.AppDataFolder, "logs");
                Directory.CreateDirectory(logsFolder);
                Logger.Loggers.Add(new FileWriter(Path.Combine(logsFolder, "log.txt")));
                Logger.Loggers.Add(new DebugWriter());

                _terminal = new Terminal("PSQuickAssets Terminal");
                _terminal.Commands
                    .Add(new ConsoleCommand("appdatafolder", "Opens PSQuickAssets data folder in explorer", (_) => OpenAppdataFolder()))
                    //.Add(new ConsoleCommand("register", "Registers new global hotkey", (hotkey) =>
                    //{
                    //    GlobalHotkeys.TryRegister(new Hotkey(hotkey), () => Logger.Info($"{hotkey} is pressed!"), out string errorMessage);
                    //}))
                    //.Add(new ConsoleCommand("unregister", "Registers new global hotkey", (hotkey) =>
                    //{
                    //    GlobalHotkeys.Remove(new Hotkey(hotkey));
                    //}))
                    .Add(new ConsoleCommand("exit", "Exits the app", (_) => App.Current.Shutdown()));

                Logger.Loggers.Add(_terminal);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log setup failed:\n\n" + ex);
            }
        }

        private void OpenAppdataFolder()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(App.AppDataFolder);
            processStartInfo.UseShellExecute = true;
            Process.Start(processStartInfo);
        }

        private void OnGlobalHotkeyPressed() => WindowManager.ToggleMainWindow();
    }
}
