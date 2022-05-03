using PSQA.Core;
using PureLib;

namespace PSQuickAssets.PSInterop;

public interface IAnotherPhotoshopInterop
{
    PhotoshopResponse AddAsLayer(string filePath, MaskMode? maskMode, bool unlinkMask);
    PhotoshopResponse ExecuteAction(string actionName, string set);
    Result HasOpenDocument();
    Result HasSelection();
    PhotoshopResponse OpenAsNewDocument(string filePath);
}