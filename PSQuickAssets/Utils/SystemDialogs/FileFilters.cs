namespace PSQuickAssets.Utils;

/// <summary>
/// Holds filters for Select File Dialog.
/// </summary>
public static class FileFilters
{
    /// <summary>
    /// Filter for allowed image formats.
    /// </summary>
    public static string Images { get; } = "Images (*.bmp; *.jpg; *.jpeg; *.gif; *.tiff; *.tif; *.psd; *.psb)|*.bmp; *.jpg; *.jpeg; *.gif; *.tiff; *.tif; *.psd; *.psb";
    /// <summary>
    /// Filter for all files.
    /// </summary>
    public static string AllFiles { get; } = "All files(*.*)|*.*";
}
