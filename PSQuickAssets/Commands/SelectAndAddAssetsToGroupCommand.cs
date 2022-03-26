using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using PSQuickAssets.Assets;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using PSQuickAssets.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

internal class SelectAndAddAssetsToGroupCommand : IAsyncCommand<AssetGroupViewModel>
{
    public event EventHandler? CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }

    private readonly AssetManager _assetManager;
    private readonly IStatusService _statusService;
    private readonly INotificationService _notificationService;

    public SelectAndAddAssetsToGroupCommand(AssetManager assetManager,
                            IStatusService statusService, INotificationService notificationService)
    {
        _assetManager = assetManager;
        _statusService = statusService;
        _notificationService = notificationService;
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        ExecuteAsync((AssetGroupViewModel)parameter).SafeFireAndForget();
    }

    public async Task ExecuteAsync(AssetGroupViewModel parameter)
    {
        string[] files = SystemDialogs.SelectFiles(Localization.Instance["SelectAssets"], FileFilters.Images, SelectionMode.Multiple);
        if (files.Length == 0)
            return;

        using (_statusService.Loading(Localization.Instance["Assets_AddingAssets"]))
        {
            try
            {
                IEnumerable<Asset> assets = await Task.Run(() => _assetManager.Load(files));
                parameter.AddAssets(assets, DuplicateHandling.Deny);
            }
            catch (Exception ex)
            {
                _notificationService.Notify($"{Localization.Instance["Error_AddingAssetsFailed"]}:\n{ex.Message}", NotificationIcon.Error);
            }
        }
    }

    public void RaiseCanExecuteChanged()
    {
        throw new NotImplementedException();
    }
}
