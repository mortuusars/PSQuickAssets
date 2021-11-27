namespace PSQuickAssets.PSInterop
{
    public interface IPhotoshopInterop
    {
        PSResult AddImageToDocumentWithMask(string filePath, MaskMode maskMode);

        /// <summary>
        /// Attempts to add given image (filepath) to open document in PS.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        PSResult AddImageToDocument(string filePath);
        /// <summary>
        /// Open image as new document.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        PSResult OpenImage(string filePath);

        PSResult ExecuteAction(string action, string from);
        bool HasSelection();
    }
}
