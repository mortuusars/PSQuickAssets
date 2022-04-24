namespace PSQuickAssets.WPF;

internal static class ThumbnailQualityExtensions
{
    public static int Value(this ThumbnailQuality thumbnailQuality)
    {
        return thumbnailQuality switch
        {
            ThumbnailQuality.Low => 70,
            ThumbnailQuality.Medium => 150,
            ThumbnailQuality.High => 300,
            _ => throw new ArgumentOutOfRangeException("This state of ThumbnailQuality is not valid.")
        };
    }
}
