using Microsoft.Toolkit.Mvvm.ComponentModel;
using PSQuickAssets.Assets;
using PSQuickAssets.Configuration;
using PSQuickAssets.Services;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public bool AlwaysOnTop { get => _config.AlwaysOnTop; }

        public AssetsViewModel AssetsViewModel { get; }

        public double ThumbnailSize { get; } = 55;

        public ICommand SettingsCommand { get; }
        public ICommand HideCommand { get; }
        public ICommand ShutdownCommand { get; } 

        private readonly WindowManager _windowManager;
        private readonly Config _config;

        internal MainViewModel(WindowManager windowManager, INotificationService notificationService, Config config)
        {
            AssetsViewModel = new AssetsViewModel(new AssetLoader(), new AssetAtlas(new AtlasLoader(), new AtlasSaver(), "save.json"), windowManager, notificationService, config);

            _windowManager = windowManager;
            _config = config;

            _config.ConfigPropertyChanged += (property) => { if (property.Equals(nameof(_config.AlwaysOnTop))) OnPropertyChanged(nameof(AlwaysOnTop)); };

            SettingsCommand = new RelayCommand(_ => _windowManager.ShowSettingsWindow());
            HideCommand = new RelayCommand(_ => _windowManager.HideMainWindow());
            ShutdownCommand = new RelayCommand(_ => App.Current.Shutdown());
        }
    }
}
