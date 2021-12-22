using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Assets;
using PSQuickAssets.Configuration;
using PSQuickAssets.Services;
using PSQuickAssets.WPF;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public bool AlwaysOnTop { get => _config.AlwaysOnTop; }

        public AssetsViewModel AssetsViewModel { get; }

        public double ThumbnailSize { get => _thumbnailSize; set { _thumbnailSize = value; OnPropertyChanged(nameof(ThumbnailSize)); } }
        private double _thumbnailSize;

        public Config Config { get => _config; }

        public ICommand IncreaseThumbnailSizeCommand { get; }
        public ICommand DecreaseThumbnailSizeCommand { get; }

        public ICommand SettingsCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand ShutdownCommand { get; }

        private readonly WindowManager _windowManager;
        private readonly INotificationService _notificationService;
        private readonly Config _config;

        internal MainViewModel(WindowManager windowManager, INotificationService notificationService, Config config)
        {
            AssetsViewModel = new AssetsViewModel(new AssetLoader(), new AssetAtlas(new AtlasLoader(), new AtlasSaver(), "save.json"), windowManager, notificationService, config);

            _windowManager = windowManager;
            _notificationService = notificationService;
            _config = config;

            Config.PropertyChanged += (_, property) => { if (property.PropertyName?.Equals(nameof(_config.AlwaysOnTop)) ?? false) OnPropertyChanged(nameof(AlwaysOnTop)); };

            _thumbnailSize = Config.ThumbnailSize;

            IncreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Up));
            DecreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Down));

            SettingsCommand = new RelayCommand(_windowManager.ShowSettingsWindow);
            HideCommand = new RelayCommand(_windowManager.HideMainWindow);
            ShutdownCommand = new RelayCommand(App.Current.Shutdown);
        }

        private void ChangeThumbnailSize(MouseWheelDirection direction)
        {
            switch (direction)
            {
                case MouseWheelDirection.Up:
                    if (ThumbnailSize <= 142)
                        ThumbnailSize = ThumbnailSize += 8;
                    break;
                case MouseWheelDirection.Down:
                    if (ThumbnailSize >= 38)
                        ThumbnailSize = ThumbnailSize -= 8;
                    break;
            }

            _config.TrySetValue(nameof(_config.ThumbnailSize), ThumbnailSize, out string _);
        }
    }
}
