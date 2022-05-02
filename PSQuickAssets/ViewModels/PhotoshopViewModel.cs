using CommunityToolkit.Mvvm.Input;
using PSQA.Core;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

internal class PhotoshopViewModel
{
    public ObservableCollection<PhotoshopAction> GlobalActions { get; } = new();

    public IRelayCommand<PhotoshopAction> AddGlobalActionCommand { get; }
    public IRelayCommand<PhotoshopAction> RemoveGlobalActionCommand { get; }

    public IAsyncRelayCommand<AssetViewModel> AddImageToPhotoshopAsyncCommand { get; }
    public IAsyncRelayCommand<AssetViewModel> OpenAsNewDocumentCommand { get; }

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

        AddImageToPhotoshopAsyncCommand = new AsyncRelayCommand<AssetViewModel>(asset => AddImageToPhotoshopAsync(asset!));
        OpenAsNewDocumentCommand = new AsyncRelayCommand<AssetViewModel>(asset => OpenAsNewDocumentAsync(asset!));

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

    private async Task AddImageToPhotoshopAsync(AssetViewModel asset)
    {
        PSResult result = await _photoshop.HasOpenDocumentAsync() ?
            await AddAsLayerAsync(asset) :
            await OpenAsNewDocumentAsync(asset);

        if (result.Success)
            await ExecuteGlobalActions();
    }

    private async Task<PSResult> AddAsLayerAsync(AssetViewModel asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        PSResult result;

        MaskMode? maskMode = _config.AddMaskToAddedLayer ? _config.MaskMode : null;

        result = await _photoshop.AddAsLayerAsync(asset.FilePath, maskMode, _config.UnlinkMask);

        //if (_config.AddMaskToAddedLayer && await _photoshop.HasSelectionAsync())
        //{
        //    MaskMode maskMode = MaskMode.RevealSelection;
        //    bool unlinkMask = _config.UnlinkMask;
        //    result = await _photoshop.AddAsLayerWithMaskAsync(asset.FilePath, maskMode, unlinkMask);
        //}
        //else
        //    result = await _photoshop.AddImageToDocumentAsync(asset.FilePath);

        if (result.Success)
            asset.Uses++;

        if (result.Failed)
            _notificationService.Notify(Localize[nameof(Lang.Assets_AddingToPhotoshopFailed)],
                Localize[$"PSStatus_{result.Status}"], NotificationIcon.Error);

        return result;
    }

    private async Task<PSResult> OpenAsNewDocumentAsync(AssetViewModel asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        PSResult result = await _photoshop.OpenImageAsNewDocumentAsync(asset.FilePath);

        if (result.Success)
            asset.Uses++;

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