using Microsoft.Extensions.DependencyInjection;
using MLogger;
using PSQuickAssets.Assets;
using PSQuickAssets.Services;
using PSQuickAssets.Update;
using PSQuickAssets.ViewModels;
using System;

namespace PSQuickAssets
{
    internal static class DIKernel
    {
        public static IServiceProvider ServiceProvider { get; } = CreateServiceProvider();

        public static IServiceProvider CreateServiceProvider()
        {            
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<INotificationService, TaskbarNotificationService>();
            services.AddSingleton<ILogger>((provider) => LoggerSetup.CreateLogger(LogLevel.Debug, provider.GetRequiredService<INotificationService>()));

            services.AddSingleton<IConfig>(p => Config.Deserialize(p.GetRequiredService<ILogger>()));
            
            services.AddSingleton<WindowManager>();
            services.AddSingleton<GlobalHotkeys>((provider) =>
                    new GlobalHotkeys(provider.GetRequiredService<WindowManager>().GetMainWindowHandle(),
                        provider.GetRequiredService<INotificationService>(),
                        provider.GetRequiredService<ILogger>()));

            services.AddSingleton<AssetManager>((provider) => new AssetManager(App.AppDataFolder + "/assets/", provider.GetRequiredService<ILogger>()));
            services.AddSingleton<PhotoshopCommandsViewModel>();

            services.AddSingleton<AssetsViewModel>();
            services.AddSingleton<MainViewModel>();

            services.AddTransient<TaskBarViewModel>();

            services.AddTransient<SettingsViewModel>();

            services.AddTransient<UpdateChecker>();

            return services.BuildServiceProvider();
        }
    }
}
