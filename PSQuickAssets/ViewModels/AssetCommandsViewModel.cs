using AsyncAwaitBestPractices.MVVM;
using PSQuickAssets.Configuration;
using PSQuickAssets.Models;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

public class AssetCommandsViewModel
{
    public IList<PhotoshopAction> Actions { get; }

    public AsyncCommand<Asset> OpenImageCommand { get; }
    public AsyncCommand<Asset> AddImageAsLayerCommand { get; }
    public AsyncCommand<PhotoshopAction> ExecuteActionCommand { get; }

    private readonly IPhotoshopInterop _photoshopInterop;
    private readonly WindowManager _windowManager;
    private readonly INotificationService _notificationService;
    private readonly Config _config;

    internal AssetCommandsViewModel(WindowManager windowManager, INotificationService notificationService, Config config)
    {
        _windowManager = windowManager;
        _notificationService = notificationService;
        _config = config;

        _photoshopInterop = new PhotoshopInterop();

        Actions = new List<PhotoshopAction>();

        OpenImageCommand = new AsyncCommand<Asset>(asset => OpenImageAsNewDocumentAsync(asset!));
        AddImageAsLayerCommand = new AsyncCommand<Asset>(asset => AddImageToPhotoshopAsync(asset!));
        ExecuteActionCommand = new AsyncCommand<PhotoshopAction>(action => ExecuteActionAsync(action!));
    }

    private async Task<bool> OpenImageAsNewDocumentAsync(Asset asset)
    {
        FocusPhotoshop();

        var result = await _photoshopInterop.OpenImageAsNewDocumentAsync(asset.FilePath);

        if (result.IsSuccessful)
            return true;
        else
        {
            string errorMessage = Resources.Localization.Instance[$"PSStatus_{result.Status}"];
            _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
            return false;
        }
    }

    private async Task<bool> AddImageToPhotoshopAsync(Asset asset)
    {
        FocusPhotoshop();

        if (await _photoshopInterop.HasOpenDocumentAsync() is false)
            return await OpenImageAsNewDocumentAsync(asset);

        PSResult result;

        if (_config.AddMaskIfDocumentHasSelection && await _photoshopInterop.HasSelectionAsync())
            result = await _photoshopInterop.AddImageToDocumentWithMaskAsync(asset.FilePath, MaskMode.RevealSelection);
            //TODO: Option to disable Unlink Mask
        else
            result = await _photoshopInterop.AddImageToDocumentAsync(asset.FilePath);

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

        if (result.IsSuccessful)
            return true;
        else
        {
            string errorMessage = String.Format(Resources.Localization.Instance["Assets_CannotExecuteActionFromSet"], action.Action, action.Set) + $"\n{result.Message}";
            _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
            return false;
        }
    }

    private async Task<bool> ExecuteGlobalActions()
    {
        foreach (var action in Actions)
        {
            if (await ExecuteActionAsync(action))
                return false;
        }
        return true;
    }

    private void FocusPhotoshop()
    {
        //TODO: Decouple from this class. Add an option to not hide assets window on placing.
        _windowManager.HideMainWindow();
        WindowControl.FocusWindow("photoshop");
    }
}
