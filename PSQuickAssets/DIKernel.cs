using Microsoft.Extensions.DependencyInjection;
using MLogger;
using PSQuickAssets.Assets;
using PSQuickAssets.Configuration;
using PSQuickAssets.Services;
using PSQuickAssets.Update;
using PSQuickAssets.ViewModels;
using System;
using System.IO;

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

            services.AddSingleton<IConfigSaver, JsonFileConfigHandler>();
            services.AddSingleton<Config>();
            
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

        //private static Config CreateConfig(IServiceProvider provider, string configJsonFilePath)
        //{
        //    //TODO: All of this is pretty dirty. Should probably restructure it somehow.
        //    var configFileHandler = provider.GetRequiredService<JsonFileConfigHandler>();
        //    var logger = provider.GetRequiredService<ILogger>();
        //    var config = new Config(configFileHandler, logger, saveOnPropertyChanged: true);

        //    if (!File.Exists(configJsonFilePath))
        //        config.Save();

        //    config.Load<Config>(configFileHandler);

        //    return config;
        //}
    }
}
