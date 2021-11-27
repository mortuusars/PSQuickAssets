using PSQuickAssets.Models;
using PSQuickAssets.PSInterop;
using PSQuickAssets.Services;
using PSQuickAssets.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels;

public class AssetUseViewModel
{
    public ICommand OpenImageCommand { get; }

    public ICommand AddImageAsLayerCommand { get; }
    public ICommand AddImageAsLayerWithMaskCommand { get; }
    public ICommand ExecuteActionCommand { get; }

    private readonly WindowManager _windowManager;
    private readonly INotificationService _notificationService;

    internal AssetUseViewModel(WindowManager windowManager, INotificationService notificationService)
    {
        _windowManager = windowManager;
        _notificationService = notificationService;

        OpenImageCommand = new RelayCommand(asset => OpenImageAsync((Asset)asset));
    }

    public async void OpenImageAsync(Asset asset)
    {
        //ActivatePhotoshop();

        //IPhotoshopInterop photoshopInterop = new PhotoshopInterop();
        //PSResult psResult = await Task.Run(() => photoshopInterop.OpenImage(asset.FilePath));
    }

    //public async void AddImageAsLayerAsync(string filePath)
    //{
    //    ActivatePhotoshop();

    //    IPhotoshopInterop photoshopInterop = new PhotoshopInterop();
    //    PSResult psResult = await Task.Run(() => photoshopInterop.AddImageToDocument(filePath));

    //    if (psResult.Status == PSStatus.NoDocumentsOpen)
    //        psResult = await Task.Run(() => photoshopInterop.OpenImage(filePath));

    //    if (psResult.Status != PSStatus.Success)
    //    {
    //        string errorMessage = Resources.Localization.Instance[$"PSStatus_{psResult.Status}"];
    //        _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
    //    }
    //}

    //public async void AddImageAsLayerWithMaskAsync(string filePath)
    //{
    //    ActivatePhotoshop();

    //    IPhotoshopInterop photoshopInterop = new PhotoshopInterop();
    //    PSResult psResult = await Task.Run(() => photoshopInterop.AddImageToDocumentWithMask(filePath, MaskMode.RevealSelection));

    //    if (psResult.Status == PSStatus.NoSelection)
    //        psResult = await Task.Run(() => photoshopInterop.AddImageToDocument(filePath));

    //    if (psResult.Status == PSStatus.NoDocumentsOpen)
    //    {
    //        psResult = await Task.Run(() => photoshopInterop.OpenImage(filePath));
    //        return;
    //    }

    //    if (psResult.Status != PSStatus.Success)
    //    {
    //        string errorMessage = Resources.Localization.Instance[$"PSStatus_{psResult.Status}"];
    //        _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
    //        return;
    //    }

    //    await ExecuteActionAsync(photoshopInterop, "SelectRGBLayer", "Mask");
    //}

    //private void ActivatePhotoshop()
    //{
    //    _windowManager.HideMainWindow();
    //    WindowControl.FocusWindow("photoshop");
    //}

    //public async Task ExecuteActionAsync(IPhotoshopInterop photoshopInterop, string action, string from)
    //{
    //    PSResult psResult = await Task.Run(() => photoshopInterop.ExecuteAction(action, from));

    //    if (psResult.Status != PSStatus.Success)
    //    {
    //        string errorMessage = String.Format(Resources.Localization.Instance["Assets_CannotExecuteActionFromSet"], action, from) + $"\n{psResult.ResultMessage}";
    //        _notificationService.Notify(App.AppName, errorMessage, NotificationIcon.Error);
    //    }
    //}
}
