﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public bool AlwaysOnTop { get => _config.AlwaysOnTop; }
        public double ThumbnailSize { get => _config.ThumbnailSize; }

        public AssetsViewModel AssetsViewModel { get; }

        public ICommand IncreaseThumbnailSizeCommand { get; }
        public ICommand DecreaseThumbnailSizeCommand { get; }

        public ICommand SettingsCommand { get; }
        public ICommand ToggleTerminalCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand ShutdownCommand { get; }

        private readonly WindowManager _windowManager;
        private readonly IConfig _config;

        public MainViewModel(AssetsViewModel assetsViewModel, WindowManager windowManager, IConfig config)
        {
            AssetsViewModel = assetsViewModel;

            _windowManager = windowManager;
            _config = config;
            _config.PropertyChanged += Config_PropertyChanged;

            IncreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Up));
            DecreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Down));
            SettingsCommand = new RelayCommand(_windowManager.ToggleSettingsWindow);
            ToggleTerminalCommand = new RelayCommand(App.ToggleTerminalWindow);
            HideCommand = new RelayCommand(_windowManager.HideMainWindow);
            ShutdownCommand = new RelayCommand(App.Current.Shutdown);
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private void Config_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (nameof(_config.AlwaysOnTop).Equals(e.PropertyName)) OnPropertyChanged(nameof(AlwaysOnTop));
            else if (nameof(_config.ThumbnailSize).Equals(e.PropertyName)) OnPropertyChanged(nameof(ThumbnailSize));
        }

        private void ChangeThumbnailSize(MouseWheelDirection direction)
        {
            switch (direction)
            {
                case MouseWheelDirection.Up:
                    if (ThumbnailSize <= 142)
                        _config.ThumbnailSize += 8;
                    break;
                case MouseWheelDirection.Down:
                    if (ThumbnailSize >= 30)
                        _config.ThumbnailSize -= 8;
                    break;
            }
        }
    }
}
