using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PSQuickAssets.Services;
using PureUI;

namespace PSQuickAssets.ViewModels;

internal partial class AssetsWindowViewModel
{
    public AssetsViewModel AssetsViewModel { get; }
    public IStatusService StatusService { get; }
    public IConfig Config { get; }

    public AssetsWindowViewModel(AssetsViewModel assetsViewModel, IStatusService statusService, IConfig config)
    {
        AssetsViewModel = assetsViewModel;
        StatusService = statusService;
        Config = config;
    }

    [ICommand]
    private void ChangeThumbnailSize(MouseWheelDirection direction)
    {
        switch (direction)
        {
            case MouseWheelDirection.Up:
                if (Config.ThumbnailSize <= 142)
                    Config.ThumbnailSize += 8;
                break;
            case MouseWheelDirection.Down:
                if (Config.ThumbnailSize >= 30)
                    Config.ThumbnailSize -= 8;
                break;
        }
    }
}
