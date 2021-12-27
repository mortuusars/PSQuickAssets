using MLogger;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.Assets.Thumbnails
{
    internal class ThumbnailManager
    {
        /// <summary>
        /// Maximum width of a thumbnail in pixels.
        /// </summary>
        public int MaxWidth { get; set; }
        /// <summary>
        /// Maximum height of a thumbnail in pixels.
        /// </summary>
        public int MaxHeight { get; set; }

        private readonly IThumbnailCreator _thumbnailCreator;
        private readonly ILogger _logger;

        public ThumbnailManager(ILogger logger)
        {
            _thumbnailCreator = new NetVipsThumbnailCreator();
            _logger = logger;

            MaxWidth = 400;
            MaxHeight = 90;
        }

        /// <summary>
        /// Creates a thumbnail from specified filePath and constraints.
        /// </summary>
        /// <param name="filePath">Full path to the image file.</param>
        /// <returns>Created thumbnail or <see langword="null"/> if failed.</returns>
        public BitmapImage? Create(string filePath, int maxWidth, int maxHeight)
        {
            try
            {
                return _thumbnailCreator.CreateThumbnail(filePath, maxWidth, maxHeight);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to create thumbnail:\n" + ex);
                return null;
            }
        }

        /// <summary>
        /// Creates a thumbnail from specified filePath. Default constraints for width and height will be used.
        /// </summary>
        /// <param name="filePath">Full path to the image file.</param>
        /// <returns>Created thumbnail or <see langword="null"/> if failed.</returns>
        public BitmapImage? Create(string filePath) => Create(filePath, MaxWidth, MaxHeight);
    }
}
