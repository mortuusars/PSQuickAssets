using CommunityToolkit.Mvvm.Input;
using PSQA.Core;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

internal class PhotoshopViewModel
{
    public ObservableCollection<PhotoshopAction> GlobalActions { get; } = new();

    public IRelayCommand AddGlobalActionCommand { get; }
    public IRelayCommand RemoveGlobalActionCommand { get; }

    public IAsyncRelayCommand AddImageToPhotoshopAsyncCommand { get; }
    public IAsyncRelayCommand OpenAsNewDocumentCommand { get; }

    private readonly IPhotoshopInterop _photoshop;
    private readonly WindowManager _windowManager;
    private readonly INotificationService _notificationService;
    private readonly IConfig _config;

    public PhotoshopViewModel(IPhotoshopInterop photoshopInterop, WindowManager windowManager, INotificationService notificationService, IConfig config)
    {
        _photoshop = photoshopInterop;
        _windowManager = windowManager;
        _notificationService = notificationService;
        _config = config;

        AddGlobalActionCommand = new RelayCommand<PhotoshopAction>(a => AddGlobalAction(a!));
        RemoveGlobalActionCommand = new RelayCommand<PhotoshopAction>(a => RemoveGlobalAction(a!));

        AddImageToPhotoshopAsyncCommand = new AsyncRelayCommand<string>(path => AddImageToPhotoshopAsync(path!));
        OpenAsNewDocumentCommand = new AsyncRelayCommand<string>(path => OpenAsNewDocumentAsync(path!));

        //TODO: Global actions and window for setting them up.
        AddGlobalAction(new PhotoshopAction("SelectRGBLayer", "Mask"));
    }

    private void AddGlobalAction(PhotoshopAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        // Duplicates can be added because action may be used multiple times and in different order.

        if (action != PhotoshopAction.Empty)
            GlobalActions.Add(action);
    }

    private void RemoveGlobalAction(PhotoshopAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        GlobalActions.Remove(action);
    }


    private async Task<bool> AddImageToPhotoshopAsync(string filePath)
    {
        PSResult result = await _photoshop.HasOpenDocumentAsync() ?
            await AddAsLayerAsync(filePath) :
            await OpenAsNewDocumentAsync(filePath);

        if (result.Success)
            return await ExecuteGlobalActions();

        return false;
    }

    private async Task<PSResult> AddAsLayerAsync(string filePath)
    {
        PSResult result;

        if (_config.AddMaskIfDocumentHasSelection && await _photoshop.HasSelectionAsync())
        {
            MaskMode maskMode = MaskMode.RevealSelection;
            bool unlinkMask = _config.UnlinkMask;
            result = await _photoshop.AddAsLayerWithMaskAsync(filePath, maskMode, unlinkMask);
        }
        else
            result = await _photoshop.AddImageToDocumentAsync(filePath);

        if (result.Failed)
            _notificationService.Notify(Localize[nameof(Lang.Assets_AddingToPhotoshopFailed)],
                Localize[$"PSStatus_{result.Status}"], NotificationIcon.Error);

        return result;
    }

    private async Task<PSResult> OpenAsNewDocumentAsync(string filePath)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        PSResult result = await _photoshop.OpenImageAsNewDocumentAsync(filePath);

        if (result.Failed)
            _notificationService.Notify(Localize[nameof(Lang.Assets_AddingToPhotoshopFailed)],
                Localize[$"PSStatus_{result.Status}"], NotificationIcon.Error);

        return result;
    }

    private async Task<bool> ExecuteGlobalActions()
    {
        foreach (var action in GlobalActions)
        {
            if (!await ExecuteActionAsync(action))
                return false; // Do not execute further. 
        }
        return true;
    }

    private async Task<bool> ExecuteActionAsync(PhotoshopAction action)
    {
        PSResult result = await _photoshop.ExecuteActionAsync(action.Action, action.Set);

        if (result.Failed)
        {
            _notificationService.Notify(
                string.Format(Localize["Assets_CannotExecuteActionFromSet"], action.Action, action.Set) + $"\n{result.Message}",
                NotificationIcon.Error);
        }

        return true;
    }
}