using Microsoft.Extensions.DependencyInjection;
using PSQA.Assets.Repository;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using PSQuickAssets.ViewModels;
using Serilog;

namespace PSQuickAssets;

internal static class DIKernel
{
    public static IServiceProvider ServiceProvider { get; } = CreateServiceProvider();

    private static IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<IStatusService, StatusService>();
        services.AddSingleton<INotificationService, TaskbarNotificationService>();
        services.AddSingleton<ILogger>(Logging.CreateLogger());
        services.AddSingleton<IConfig, Config>();

        services.AddSingleton<WindowManager>();
        services.AddSingleton<TerminalHandler>();
        services.AddSingleton<GlobalHotkeys>((provider) =>
                new GlobalHotkeys(provider.GetRequiredService<WindowManager>().GetMainWindowHandle(),
                    provider.GetRequiredService<INotificationService>(),
                    provider.GetRequiredService<ILogger>()));

        services.AddSingleton<IPhotoshopInterop, PhotoshopInterop>();
        services.AddSingleton<PhotoshopViewModel>();

        services.AddSingleton<IAssetRepositoryHandler>(p =>
            new DirectoryRepositoryHandler(Folders.AppData + "/catalog/", p.GetRequiredService<ILogger>()));
        services.AddSingleton<AssetRepository>();

        services.AddSingleton<AssetsViewModel>();

        services.AddTransient<AssetsWindowViewModel>();

        services.AddTransient<SettingsViewModel>();

        services.AddTransient<UpdateChecker>();

        return services.BuildServiceProvider();
    }
}
