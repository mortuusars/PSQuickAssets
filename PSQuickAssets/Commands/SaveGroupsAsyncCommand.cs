using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using PSQuickAssets.Assets;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSQuickAssets.Commands;

internal class SaveGroupsAsyncCommand : IAsyncCommand
{
    public event EventHandler? CanExecuteChanged;

    private readonly AssetsViewModel _assetsViewModel;
    private readonly AssetManager _assetManager;
    private readonly INotificationService _notificationService;

    public SaveGroupsAsyncCommand(AssetsViewModel assetsViewModel, AssetManager assetManager, INotificationService notificationService)
    {
        _assetsViewModel = assetsViewModel;
        _assetManager = assetManager;
        _notificationService = notificationService;
    }

    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => ExecuteAsync().SafeFireAndForget();

    public async Task ExecuteAsync()
    {
        List<AssetGroup> groups = _assetsViewModel.AssetGroups.Select(g => g.Group).ToList();

        Result saveResult = await _assetManager.SaveAsync(groups);

        if (saveResult.IsSuccessful)
            return;

        if (saveResult.Exception is AggregateException aggregateException)
        {
            if (aggregateException.InnerExceptions.Count == _assetsViewModel.AssetGroups.Count)
                _notificationService.Notify(App.AppName, Localization.Instance["FailedToSaveAllGroups"], NotificationIcon.Error);
            else
            {
                string failedGroups = "";
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    failedGroups += exception.Message;
                }

                _notificationService.Notify(App.AppName, Localization.Instance["FailedToSaveGroups"] + $"\n<{failedGroups}>", NotificationIcon.Error);
            }
        }
        else
            _notificationService.Notify(App.AppName, Localization.Instance["FailedToSaveAssetGroups"] + saveResult.Exception?.Message, NotificationIcon.Error);
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}