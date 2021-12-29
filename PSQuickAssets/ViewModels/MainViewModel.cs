using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Assets;
using PSQuickAssets.Configuration;
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
        private readonly Config _config;

        public MainViewModel(AssetsViewModel assetsViewModel, WindowManager windowManager, Config config)
        {
            //AssetsViewModel = new AssetsViewModel(new AssetManager(App.AppDataFolder + "/assets/", App.Logger), windowManager, notificationService, config);
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
                        _config.TrySetValue(nameof(_config.ThumbnailSize), ThumbnailSize + 8, out string _);
                    break;
                case MouseWheelDirection.Down:
                    if (ThumbnailSize >= 30)
                        _config.TrySetValue(nameof(_config.ThumbnailSize), ThumbnailSize - 8, out string _);
                    break;
            }
        }
    }
}
