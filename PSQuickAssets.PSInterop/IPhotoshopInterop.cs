using PSQA.Core;
using System.Threading.Tasks;

namespace PSQuickAssets.PSInterop
{
    /// <summary>
    /// Provides methods to call Photoshop.
    /// </summary>
    public interface IPhotoshopInterop
    {
        /// <summary>
        /// Executes specified action.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="set">Set containing that action.</param>
        Task<PSResult> ExecuteActionAsync(string actionName, string set);

        Task<PSResult> AddAsLayerAsync(string filePath, MaskMode? maskMode, bool unlinkMask);

        /// <summary>
        /// Attempts to add given image (filepath) as layer to open document in PS and applies a mask to it if selection was present.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        /// <param name="maskMode">Mask mode.</param>
        /// <param name="unlinkMask">Indicates whether mask should be unlinked after applying.</param>
        Task<PSResult> AddAsLayerWithMaskAsync(string filePath, MaskMode maskMode, bool unlinkMask);
        /// <summary>
        /// Attempts to add given image (filepath) as layer to open document in PS.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        Task<PSResult> AddImageToDocumentAsync(string filePath);
        /// <summary>
        /// Opens image as new document.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        Task<PSResult> OpenImageAsNewDocumentAsync(string filePath);
        /// <summary>
        /// Determines if Active Document has a selection.
        /// </summary>
        Task<bool> HasSelectionAsync();
        /// <summary>
        /// Determines if Photoshop has a Document open.
        /// </summary>
        Task<bool> HasOpenDocumentAsync();
    }
}
