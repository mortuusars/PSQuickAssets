using CommunityToolkit.Mvvm.Input;
using PSQA.Core;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

internal partial class PhotoshopCommands
{
    public ObservableCollection<PhotoshopAction> GlobalActions { get; } = new();

    private readonly IPhotoshopInterop _photoshopInterop = new PhotoshopInterop();
    private readonly WindowManager _windowManager;
    private readonly INotificationService _notificationService;
    private readonly IConfig _config;

    public PhotoshopCommands(WindowManager windowManager, INotificationService notificationService, IConfig config)
    {
        _windowManager = windowManager;
        _notificationService = notificationService;
        _config = config;

        //TODO: Global actions
        GlobalActions.Add(new PhotoshopAction("SelectRGBLayer", "Mask"));
    }

    [ICommand]
    private async Task<bool> OpenAsNewDocumentAsync(Asset asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        var result = await _photoshopInterop.OpenImageAsNewDocumentAsync(asset.Path);

        if (result.IsSuccessful is false)
        {
            string errorMessage = Resources.Localization.Instance[$"PSStatus_{result.Status}"];
            _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
        }

        return result.IsSuccessful;
    }

    [ICommand]
    private async Task<bool> AddAsLayerAsync(Asset asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        if (await _photoshopInterop.HasOpenDocumentAsync() is false)
            return await OpenAsNewDocumentAsync(asset);

        PSResult result;

        if (_config.AddMaskIfDocumentHasSelection && await _photoshopInterop.HasSelectionAsync())
            result = await _photoshopInterop.AddImageToDocumentWithMaskAsync(asset.Path, MaskMode.RevealSelection, unlinkMask: _config.UnlinkMask);
        else
            result = await _photoshopInterop.AddImageToDocumentAsync(asset.Path);

        if (result.IsSuccessful)
        {
            return await ExecuteGlobalActions();
        }
        else
        {
            string errorMessage = Resources.Localization.Instance[$"PSStatus_{result.Status}"];
            _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
            return false;
        }
    }

    [ICommand]
    private async Task<bool> ExecuteActionAsync(PhotoshopAction action)
    {
        PSResult result = await _photoshopInterop.ExecuteActionAsync(action.Action, action.Set);

        if (result.IsSuccessful is false)
        {
            string errorMessage = String.Format(Resources.Localization.Instance["Assets_CannotExecuteActionFromSet"], action.Action, action.Set) + $"\n{result.Message}";
            _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
        }

        return result.IsSuccessful;
    }

    [ICommand]
    private async Task<bool> ExecuteGlobalActions()
    {
        foreach (var action in GlobalActions)
        {
            if (await ExecuteActionAsync(action))
                return false;
        }
        return true;
    }
}
