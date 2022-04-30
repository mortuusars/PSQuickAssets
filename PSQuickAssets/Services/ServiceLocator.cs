using System;
using Microsoft.Extensions.DependencyInjection;

namespace PSQuickAssets.Services;

internal class ServiceLocator
{
    public IConfig Config { get => DIKernel.ServiceProvider.GetRequiredService<IConfig>(); }
    public WindowManager WindowHandler { get => DIKernel.ServiceProvider.GetRequiredService<WindowManager>(); }
}
