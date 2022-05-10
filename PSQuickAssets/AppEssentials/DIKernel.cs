using Microsoft.Extensions.DependencyInjection;
using PSQA.Assets.Repository;
using PSQuickAssets.Models;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using PSQuickAssets.ViewModels;
using PureUI.Themes;
using Serilog;

namespace PSQuickAssets;

internal static class DIServiceProviderCreator
{
    public static IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<INotificationService, TaskbarNotificationService>();
        services.AddSingleton<ILogger>(Logging.CreateLogger());
        services.AddSingleton<IConfig, Config>();

        services.AddSingleton<ThemeManager>();
        services.AddTransient<ThemeService>();

        services.AddSingleton<WindowManager>();
        services.AddSingleton<TerminalHandler>();
        services.AddSingleton<GlobalHotkeys>();
        services.AddTransient<UpdateChecker>();

        services.AddTransient<Startup>();

        services.AddSingleton<IStatusService, StatusService>();

        services.AddSingleton<IPhotoshopInterop, PhotoshopInterop>();
        services.AddSingleton<Photoshop>();
        services.AddSingleton<PhotoshopViewModel>();

        services.AddSingleton<IAssetRepositoryHandler>(p =>
            new DirectoryRepositoryHandler(Folders.AppData + "/catalog/", p.GetRequiredService<ILogger>()));
        services.AddSingleton<AssetRepository>();

        services.AddSingleton<AssetsViewModel>();

        services.AddTransient<AssetsWindowViewModel>();

        services.AddTransient<SettingsViewModel>();

        return services.BuildServiceProvider();
    }
}
