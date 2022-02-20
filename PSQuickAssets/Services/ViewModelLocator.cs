using Microsoft.Extensions.DependencyInjection;
using PSQuickAssets.ViewModels;
using System;

namespace PSQuickAssets.Services;

internal class ViewModelLocator
{
    public TaskBarViewModel TaskBarViewModel { get => DIKernel.ServiceProvider.GetRequiredService<TaskBarViewModel>(); }

    public AssetsWindowViewModel AssetsWindowViewModel { get => DIKernel.ServiceProvider.GetRequiredService<AssetsWindowViewModel>(); }
    public AssetsViewModel AssetsViewModel { get => DIKernel.ServiceProvider.GetRequiredService<AssetsViewModel>(); }
    public PhotoshopCommandsViewModel PhotoshopCommandsViewModel { get => DIKernel.ServiceProvider.GetRequiredService<PhotoshopCommandsViewModel>(); }

    public MainViewModel MainViewModel { get => DIKernel.ServiceProvider.GetRequiredService<MainViewModel>(); }
    public SettingsViewModel SettingsViewModel { get => DIKernel.ServiceProvider.GetRequiredService<SettingsViewModel>(); }
}