using AsyncAwaitBestPractices.MVVM;
using PSQuickAssets.Assets;
using PSQuickAssets.Models;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

internal class PhotoshopCommands
{
    public IList<PhotoshopAction> GlobalActions { get; }

    public IAsyncCommand<Asset> OpenImageCommand { get; }
    public IAsyncCommand<Asset> AddImageAsLayerCommand { get; }
    public IAsyncCommand<PhotoshopAction> ExecuteActionCommand { get; }

    private readonly IPhotoshopInterop _photoshopInterop;

    private readonly Action _focusPhotoshopAction;
    private readonly INotificationService _notificationService;
    private readonly IConfig _config;

    public PhotoshopCommands(Action focusPhotoshopAction, INotificationService notificationService, IConfig config)
    {
        _focusPhotoshopAction = focusPhotoshopAction;
        _notificationService = notificationService;
        _config = config;

        _photoshopInterop = new PhotoshopInterop();

        GlobalActions = new List<PhotoshopAction>();
        //TODO: Global actions
        GlobalActions.Add(new PhotoshopAction("SelectRGBLayer", "Mask"));

        OpenImageCommand = new AsyncCommand<Asset>(asset => OpenImageAsNewDocumentAsync(asset!));
        AddImageAsLayerCommand = new AsyncCommand<Asset>(asset => AddImageToPhotoshopAsync(asset!));
        ExecuteActionCommand = new AsyncCommand<PhotoshopAction>(action => ExecuteActionAsync(action!));
    }


    private async Task<bool> OpenImageAsNewDocumentAsync(Asset asset)
    {
        FocusPhotoshop();

        var result = await _photoshopInterop.OpenImageAsNewDocumentAsync(asset.Path);

        if (result.IsSuccessful is false)
        {
            string errorMessage = Resources.Localization.Instance[$"PSStatus_{result.Status}"];
            _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
        }

        return result.IsSuccessful;
    }

    private async Task<bool> AddImageToPhotoshopAsync(Asset asset)
    {
        FocusPhotoshop();

        if (await _photoshopInterop.HasOpenDocumentAsync() is false)
            return await OpenImageAsNewDocumentAsync(asset);

        PSResult result;

        if (_config.AddMaskIfDocumentHasSelection && await _photoshopInterop.HasSelectionAsync())
            result = await _photoshopInterop.AddImageToDocumentWithMaskAsync(asset.Path, MaskMode.RevealSelection);
            //TODO: Option to disable Unlink Mask
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

    private async Task<bool> ExecuteGlobalActions()
    {
        foreach (var action in GlobalActions)
        {
            if (await ExecuteActionAsync(action))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Focuses Photoshop window.
    /// </summary>
    private void FocusPhotoshop() => _focusPhotoshopAction.Invoke();
}
