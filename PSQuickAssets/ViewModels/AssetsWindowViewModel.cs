using CommunityToolkit.Mvvm.Input;
using MortuusUI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

internal class AssetsWindowViewModel
{
    public AssetsViewModel AssetsViewModel { get; }
    public IConfig Config { get; }

    public ICommand IncreaseThumbnailSizeCommand { get; }
    public ICommand DecreaseThumbnailSizeCommand { get; }

    public AssetsWindowViewModel(AssetsViewModel assetsViewModel, IConfig config)
    {
        AssetsViewModel = assetsViewModel;
        Config = config;

        IncreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Up));
        DecreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Down));
    }

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
