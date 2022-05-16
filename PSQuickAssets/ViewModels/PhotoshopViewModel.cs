using CommunityToolkit.Mvvm.Input;
using PSQA.Core;
using PSQuickAssets.Models;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels;

internal partial class PhotoshopViewModel
{
    public ObservableCollection<PhotoshopAction> PostActions { get => PostActionsViewModel.Actions; }
    public PostActionsViewModel PostActionsViewModel { get; }

    private readonly Photoshop _photoshop;
    private readonly WindowManager _windowManager;
    private readonly INotificationService _notificationService;
    private readonly IConfig _config;

    public PhotoshopViewModel(Photoshop photoshop, WindowManager windowManager, INotificationService notificationService, IConfig config)
    {
        _photoshop = photoshop;
        _windowManager = windowManager;
        _notificationService = notificationService;
        _config = config;

        PostActionsViewModel = new PostActionsViewModel(_config);
    }

    [ICommand]
    private async Task AddImageToPhotoshopAsync(AssetViewModel asset)
    {
        PhotoshopResponse response = await _photoshop.HasOpenDocumentAsync() ?
            await AddAsLayerAsync(asset) :
            await OpenAsNewDocumentAsync(asset);

        if (response.Status == Status.Success && _config.ExecuteActionsAfterAdding)
            await ExecutePostActions();
    }

    [ICommand]
    private async Task<PhotoshopResponse> OpenAsNewDocumentAsync(AssetViewModel asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        PhotoshopResponse response = await _photoshop.AddAsDocumentAsync(asset.FilePath);

        if (response.Status == Status.Success)
            asset.Uses++;
        else
        {
            string errorMessage = ComposeAndLocalizeErrorMessage(response);
            _notificationService.Notify(Localize[nameof(Lang.Assets_AddingToPhotoshopFailed)], errorMessage, NotificationIcon.Error);
        }

        return response;
    }

    private async Task<PhotoshopResponse> AddAsLayerAsync(AssetViewModel asset)
    {
        _windowManager.FocusPhotoshop();
        if (_config.HideWindowWhenAddingAsset)
            _windowManager.HideMainWindow();

        PhotoshopResponse response = await _photoshop.AddAsLayerAsync(asset.FilePath);

        if (response.Status == Status.Success)
            asset.Uses++;
        else
        {
            string errorMessage = ComposeAndLocalizeErrorMessage(response);
            _notificationService.Notify(Localize[nameof(Lang.Assets_AddingToPhotoshopFailed)], errorMessage, NotificationIcon.Error);
        }

        return response;
    }

    private async Task<bool> ExecutePostActions()
    {
        foreach (var action in PostActions)
        {
            if (!await ExecuteActionAsync(action))
                return false; // Do not execute further. 
        }
        return true;
    }

    private async Task<bool> ExecuteActionAsync(PhotoshopAction action)
    {
        PhotoshopResponse response = await _photoshop.ExecuteActionAsync(action.Action, action.Set);

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