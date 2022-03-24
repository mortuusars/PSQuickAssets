﻿using CommunityToolkit.Mvvm.Input;
using PSQuickAssets.Services;
using System.Windows.Input;
using PSQuickAssets.WPF;
using MortuusUI.Extensions;
using System;

namespace PSQuickAssets.ViewModels;

internal class AssetsWindowViewModel
{
    public NewAssetsViewModel AssetsViewModel { get; }
    public IStatusService StatusService { get; }
    public IConfig Config { get; }

    public ICommand IncreaseThumbnailSizeCommand { get; }
    public ICommand DecreaseThumbnailSizeCommand { get; }

    public AssetsWindowViewModel(NewAssetsViewModel assetsViewModel, IStatusService statusService, IConfig config)
    {
        AssetsViewModel = assetsViewModel;
        StatusService = statusService;
        Config = config;

        IncreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Up));
        DecreaseThumbnailSizeCommand = new RelayCommand(() => ChangeThumbnailSize(MouseWheelDirection.Down));
    }

    /// <summary>
    /// Constructor for Designer. Do not use when app is running.
    /// </summary>
    public AssetsWindowViewModel()
    {
        if (!App.Current.IsInDesignMode())
            throw new InvalidOperationException("This constructor is only for Designer.");

        AssetsViewModel = null!;
        StatusService = null!;
        Config = null!;
        IncreaseThumbnailSizeCommand = null!;
        DecreaseThumbnailSizeCommand = null!;
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
