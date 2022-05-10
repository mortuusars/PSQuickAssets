using Microsoft.Extensions.DependencyInjection;

namespace PSQuickAssets.Services;

internal class ServiceLocator
{
    public IConfig Config { get => App.ServiceProvider.GetRequiredService<IConfig>(); }
    public WindowManager WindowHandler { get => App.ServiceProvider.GetRequiredService<WindowManager>(); }
}
