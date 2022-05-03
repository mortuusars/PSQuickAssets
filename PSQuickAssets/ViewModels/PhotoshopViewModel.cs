using CommunityToolkit.Mvvm.Input;
using PSQA.Core;
using PSQuickAssets.Models;
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

    private readonly Photoshop _photoshop1;
    private readonly IPhotoshopInterop _photoshop;
    private readonly WindowManager _windowManager;
    private readonly INotificationService _notificationService;
    private readonly IConfig _config;

    public PhotoshopViewModel(Photoshop photoshop, IPhotoshopInterop photoshopInterop, WindowManager windowManager, INotificationService notificationService, IConfig config)
    {
        _photoshop1 = photoshop;
        _photoshop = photoshopInterop;
        _windowManager = windowManager;
        _notificationService = notificationService;
        _config = config;

        AddGlobalActionCommand = new RelayCommand<PhotoshopAction>(a => AddGlobalAction(a!));
        RemoveGlobalActionCommand = new RelayCommand<PhotoshopAction>(a => RemoveGlobalAction(a!));

        AddImageToPhotoshopAsyncCommand = new AsyncRelayCommand<AssetViewModel>(asset => AddImageToPhotoshopAsync(asset!));
        OpenAsNewDocumentCommand = new AsyncRelayCommand<AssetViewModel>(asset => OpenAsNewDocumentAsync(asset!));

        //TODO: Global actions and window for setting them up.
        //AddGlobalAction(new PhotoshopAction("SelectRGBLayer", "Mask"));
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
        PhotoshopResponse response = await _photoshop1.HasOpenDocumentAsync() ?
            await AddAsLayerAsync(asset) :
            await OpenAsNewDocumentAsync(asset);

        if (response.Status == Status.Success)
            await ExecuteGlobalActions();
    }

    private async Task<PhotoshopResponse> AddAsLayerAsync(AssetViewModel asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        PhotoshopResponse response = await _photoshop1.AddAsLayerAsync(asset.FilePath);

        if (response.Status == Status.Success)
            asset.Uses++;
        else
        {
            string errorMessage = ComposeAndLocalizeErrorMessage(response);
            _notificationService.Notify(Localize[nameof(Lang.Assets_AddingToPhotoshopFailed)], errorMessage, NotificationIcon.Error);
        }

        return response;
    }

    private async Task<PhotoshopResponse> OpenAsNewDocumentAsync(AssetViewModel asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        PhotoshopResponse response = await _photoshop1.AddAsDocumentAsync(asset.FilePath);

        if (response.Status == Status.Success)
            asset.Uses++;
        else
        {
            string errorMessage = ComposeAndLocalizeErrorMessage(response);
            _notificationService.Notify(Localize[nameof(Lang.Assets_AddingToPhotoshopFailed)], errorMessage, NotificationIcon.Error);
        }

        return response;
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
        PhotoshopResponse response = await _photoshop1.ExecuteActionAsync(action.Action, action.Set);

        if (response.Status != Status.Success)
        {
            string errorTitle = string.Format(Localize[nameof(Lang.Assets_CannotExecuteActionFromSet)], action.Action, action.Set);
            string errorMessage = ComposeAndLocalizeErrorMessage(response);
            _notificationService.Notify(errorTitle, errorMessage, NotificationIcon.Error);
        }

        return true;
    }

    private string ComposeAndLocalizeErrorMessage(PhotoshopResponse response)
    {
        if (response.Status == Status.Success)
            throw new InvalidOperationException("Cannot create error message from successful response.");

        string status = Localize[$"{nameof(Status)}_{response.Status}"];

        return response.Status == Status.UnknownException || response.Status == Status.UnknownComException || response.Status == Status.Failed ?
            $"{status}\n{response.Message}" :
            status;
    }
}