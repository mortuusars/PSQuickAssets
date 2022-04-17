using PSQuickAssets.PSInterop.Internal;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PSQuickAssets.PSInterop
{
    /// <summary>
    /// Provides methods to call photoshop.
    /// </summary>
    public class PhotoshopInterop : IPhotoshopInterop
    {
        private const string _selectionChannelName = "QuickAssetsMask";

        /// <summary>
        /// Executes specified action.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="set">Set containing that action.</param>
        public async Task<PSResult> ExecuteActionAsync(string actionName, string set)
        {
            return await Task.Run(() => ExecuteAction(() =>
            {
                dynamic ps = CreatePSInstance();
                ps.DoAction(actionName, set);
            }));
        }

        /// <summary>
        /// Attempts to add given image (filepath) as layer to open document in PS and applies a mask to it if selection was present.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        /// <param name="maskMode">Mask mode.</param>
        public async Task<PSResult> AddImageToDocumentWithMaskAsync(string filePath, MaskMode maskMode, bool unlinkMask)
        {
            return await Task.Run(() => ExecuteAction(() =>
            {
                dynamic ps = CreatePSInstance();

                PsActions.SaveSelectionAsChannel(ps, _selectionChannelName);
                PsActions.AddFilePathAsLayer(ps, filePath);
                PsActions.LoadSelectionFromChannel(ps, _selectionChannelName);
                PsActions.ApplyMaskFromSelection(ps, maskMode);

                PsActions.DeleteChannel(ps, _selectionChannelName);

                if (unlinkMask)
                    PsActions.UnlinkMask(ps);
            }, filePath));
        }


        /// <summary>
        /// Attempts to add given image (filepath) as layer to open document in PS.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        public async Task<PSResult> AddImageToDocumentAsync(string filePath)
        {
            return await Task.Run(() => ExecuteAction(() =>
            {
                dynamic ps = CreatePSInstance();

                PsActions.AddFilePathAsLayer(ps, filePath);
            }, filePath));
        }

        /// <summary>
        /// Opens image as new document.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        public async Task<PSResult> OpenImageAsNewDocumentAsync(string filePath)
        {
            return await Task.Run(() => ExecuteAction(() =>
            {
                dynamic ps = CreatePSInstance();
                ps.Open(filePath);
            }, filePath));
        }

        /// <summary>
        /// Determines if Active Document has a selection.
        /// </summary>
        public async Task<bool> HasSelectionAsync()
        {
            var result = await Task.Run(() => ExecuteAction(() =>
            {
                dynamic ps = CreatePSInstance();
                var selectionBounds = ps.ActiveDocument.Selection.Bounds;
            }));

            return result.Status == Status.Success;
        }

        /// <summary>
        /// Determines if Photoshop has a Document open.
        /// </summary>
        public async Task<bool> HasOpenDocumentAsync()
        {
            var result = await Task.Run(() => ExecuteAction(() =>
            {
                dynamic ps = CreatePSInstance();
                var document = ps.ActiveDocument;
            }));

            return result.Status == Status.Success;
        }

        /// <summary>
        /// Create instance of Photoshop COM Object.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static dynamic CreatePSInstance()
        {
            Type psType = Type.GetTypeFromProgID("Photoshop.Application") ?? throw new NullReferenceException("Failed to retrieve Photoshop Type from ProgID");
            return Activator.CreateInstance(psType) ?? throw new NullReferenceException("Failed to create Photoshop instance.");
        }

        /// <summary>
        /// Executes passed call to photoshop. Catches any exceptions.
        /// </summary>
        /// <param name="action">Code, that will be executed safely.</param>
        /// <param name="filePath">Image filepath, if call is related to files.</param>
        /// <returns>Result of the executed action.</returns>
        private PSResult ExecuteAction(Action action, string filePath = "")
        {
            if (Process.GetProcessesByName("Photoshop").Length == 0)
                return new PSResult(Status.NotRunning, filePath, "Photoshop is not running");

            try
            {
                action();
                return new PSResult(Status.Success, filePath);
            }
            catch (COMException comException)
            {
                return PSResult.FromException(comException, filePath);
            }
            catch (Exception exception)
            {
                return PSResult.FromException(exception, filePath);
            }
        }
    }
}
