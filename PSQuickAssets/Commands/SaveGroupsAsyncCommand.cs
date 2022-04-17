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
using System.Windows;

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
        //List<AssetGroup> groups = _assetsViewModel.AssetGroups.Select(g => g.Group).ToList();

        //AssetSavingResult saveResult = await _assetManager.SaveAsync(groups);

        //if (saveResult.IsSuccessful)
        //    return;

        //string errorMessage = Resources.Localization.Instance["FailedToSaveGroups"];

        //foreach (var group in saveResult.FailedGroups)
        //{
        //    errorMessage += $"\n{group.Key.Name} : {group.Value.Message}";
        //}

        //_notificationService.Notify(errorMessage, NotificationIcon.Error, 
        //    () => MessageBox.Show(errorMessage, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error));
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}