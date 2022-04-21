using Microsoft.Extensions.DependencyInjection;
using PSQA.Assets.Repository;
using PSQuickAssets.Services;
using PSQuickAssets.Update;
using PSQuickAssets.ViewModels;
using Serilog;
using System;

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
        services.AddSingleton<IConfig>(p => Config.Deserialize(p.GetRequiredService<ILogger>()));
        services.AddSingleton<ConfigChangeListener>();

        services.AddSingleton<WindowManager>();
        services.AddSingleton<TerminalHandler>();
        services.AddSingleton<GlobalHotkeys>((provider) =>
                new GlobalHotkeys(provider.GetRequiredService<WindowManager>().GetMainWindowHandle(),
                    provider.GetRequiredService<INotificationService>(),
                    provider.GetRequiredService<ILogger>()));

        services.AddSingleton<PhotoshopCommands>();

        services.AddSingleton<IAssetRepositoryHandler>(p =>
            new DirectoryRepositoryHandler(App.AppDataFolder + "/catalog/", p.GetRequiredService<ILogger>()));
        services.AddSingleton<AssetRepository>();

        services.AddSingleton<AssetsViewModel>();

        services.AddTransient<AssetsWindowViewModel>();

        services.AddTransient<SettingsViewModel>();

        services.AddTransient<UpdateChecker>();

        return services.BuildServiceProvider();
    }
}
