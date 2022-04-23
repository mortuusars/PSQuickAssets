using Microsoft.Extensions.DependencyInjection;
using PSQuickAssets.ViewModels;

namespace PSQuickAssets.Services;

internal class ViewModelLocator
{
    public AssetsWindowViewModel AssetsWindowViewModel { get => App.ServiceProvider.GetRequiredService<AssetsWindowViewModel>(); }
    public AssetsViewModel AssetsViewModel { get => App.ServiceProvider.GetRequiredService<AssetsViewModel>(); }
    public PhotoshopViewModel PhotoshopViewModel { get => App.ServiceProvider.GetRequiredService<PhotoshopViewModel>(); }

    public SettingsViewModel SettingsViewModel { get => App.ServiceProvider.GetRequiredService<SettingsViewModel>(); }
}