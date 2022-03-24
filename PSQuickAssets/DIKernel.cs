using Microsoft.Extensions.DependencyInjection;
//using Serilog;
using PSQuickAssets.Assets;
using PSQuickAssets.Commands;
using PSQuickAssets.Services;
using PSQuickAssets.Update;
using PSQuickAssets.Utils;
using PSQuickAssets.ViewModels;
using Serilog;
using System;
using System.IO;

namespace PSQuickAssets;

internal static class DIKernel
{
    public static IServiceProvider ServiceProvider { get; } = CreateServiceProvider();

    public static IServiceProvider CreateServiceProvider()
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

        services.AddSingleton<AssetManager>((provider) => new AssetManager(App.AppDataFolder + "/assets/", provider.GetRequiredService<ILogger>()));
        services.AddSingleton<PhotoshopCommands>((provider) => new PhotoshopCommands(provider.GetRequiredService<WindowManager>().FocusPhotoshop, 
            provider.GetRequiredService<INotificationService>(),
            provider.GetRequiredService<IConfig>()));


        RegisterCommands(services);


        services.AddSingleton<AssetsViewModel>();
        services.AddSingleton<NewAssetsViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<AssetsWindowViewModel>();

        services.AddTransient<SettingsViewModel>();

        services.AddTransient<UpdateChecker>();

        return services.BuildServiceProvider();
    }

    private static void RegisterCommands(IServiceCollection services)
    {
        services.AddTransient<SelectAndAddAssetsToGroupCommand>();
    }
}
