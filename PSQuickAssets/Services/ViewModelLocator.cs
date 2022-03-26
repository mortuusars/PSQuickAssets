using Microsoft.Extensions.DependencyInjection;
using PSQuickAssets.ViewModels;
using System;

namespace PSQuickAssets.Services;

internal class ViewModelLocator
{
    public AssetsWindowViewModel AssetsWindowViewModel { get => DIKernel.ServiceProvider.GetRequiredService<AssetsWindowViewModel>(); }
    public NewAssetsViewModel AssetsViewModel { get => DIKernel.ServiceProvider.GetRequiredService<NewAssetsViewModel>(); }
    public PhotoshopCommands PhotoshopCommandsViewModel { get => DIKernel.ServiceProvider.GetRequiredService<PhotoshopCommands>(); }

    public MainViewModel MainViewModel { get => DIKernel.ServiceProvider.GetRequiredService<MainViewModel>(); }
    public SettingsViewModel SettingsViewModel { get => DIKernel.ServiceProvider.GetRequiredService<SettingsViewModel>(); }
}