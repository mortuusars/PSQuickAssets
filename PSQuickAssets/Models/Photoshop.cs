using PSQA.Core;
using PSQuickAssets.PSInterop;
using Serilog;
using System.Threading.Tasks;

namespace PSQuickAssets.Models;

internal class Photoshop
{
    private readonly IAnotherPhotoshopInterop _photoshopInterop;
    private readonly IConfig _config;

    public Photoshop(IAnotherPhotoshopInterop photoshopInterop, IConfig config)
    {
        _photoshopInterop = photoshopInterop;
        _config = config;
    }

    public async Task<PhotoshopResponse> AddAsLayerAsync(string filePath)
    {
        MaskMode? maskMode = _config.AddMaskToAddedLayer ? _config.MaskMode : null;
        bool unlinkMask = _config.UnlinkMask;

        return await Task.Run(() => _photoshopInterop.AddAsLayer(filePath, maskMode, unlinkMask));
    }

    public async Task<PhotoshopResponse> AddAsDocumentAsync(string filePath)
    {
        return await Task.Run(() => _photoshopInterop.OpenAsNewDocument(filePath));
    }

    public async Task<bool> HasOpenDocumentAsync()
    {
        var result = await Task.Run(() => _photoshopInterop.HasOpenDocument());
        return result.Success;
    }

    public async Task<PhotoshopResponse> ExecuteActionAsync(string name, string set)
    {
        return await Task.Run(() => _photoshopInterop.ExecuteAction(name, set));
    }
}
