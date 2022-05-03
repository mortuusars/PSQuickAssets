using PSQA.Core;
using PSQuickAssets.PSInterop.Internal;
using PureLib;
using Serilog;
using System;
using System.Diagnostics;

namespace PSQuickAssets.PSInterop;

public class PhotoshopInterop : IPhotoshopInterop
{
    private const string _selectionChannelName = "QuickAssetsMask";
    private readonly Type _photoshopApplicationType;
    private readonly ILogger _logger;

    public PhotoshopInterop(ILogger logger)
    {
        _photoshopApplicationType = Type.GetTypeFromProgID("Photoshop.Application")
            ?? throw new PhotoshopException(Status.ProgIDFailed, "Failed to retrieve Photoshop Type from ProgID");
        _logger = logger;
    }

    public Result HasOpenDocument()
    {
        try
        {
            var _ = GetPhotoshopComObject().ActiveDocument;
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Warning("HasOpenDocument failed: {0}", ex);
            return Result.Fail(ex.Message);
        }
    }

    public Result HasSelection()
    {
        try
        {
            var _ = GetPhotoshopComObject().ActiveDocument.Selection.Bounds;
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Warning("HasSelection failed: {0}", ex);
            return Result.Fail(ex.Message);
        }
    }

    public PhotoshopResponse AddAsLayer(string filePath, MaskMode? maskMode, bool unlinkMask)
    {
        try
        {
            dynamic ps = GetPhotoshopComObject();

            if (maskMode is null)
            {
                PsActions.AddFilePathAsLayer(ps, filePath);
                return PhotoshopResponse.Success;
            }

            bool hasSelection = PsActions.HasSelection(ps);

            if ((maskMode == MaskMode.RevealSelection || maskMode == MaskMode.HideSelection) && hasSelection)
            {
                PsActions.SaveSelectionAsChannel(ps, _selectionChannelName);
                PsActions.AddFilePathAsLayer(ps, filePath);
                PsActions.LoadSelectionFromChannel(ps, _selectionChannelName);
                PsActions.ApplyMaskFromSelection(ps, (MaskMode)maskMode);
                PsActions.DeleteChannel(ps, _selectionChannelName);

                if (unlinkMask)
                    PsActions.UnlinkMask(ps);

                return PhotoshopResponse.Success;
            }

            if (!hasSelection && (maskMode != MaskMode.RevealAll || maskMode != MaskMode.HideAll))
                maskMode = maskMode == MaskMode.RevealSelection ? MaskMode.RevealAll : MaskMode.HideAll;

            PsActions.AddFilePathAsLayer(ps, filePath);
            PsActions.AddMask(ps, (MaskMode)maskMode);
            if (unlinkMask)
                PsActions.UnlinkMask(ps);

            return PhotoshopResponse.Success;
        }
        catch (Exception ex)
        {
            _logger.Error("Cannot add layer to photoshop:" +
                "\nFilePath: '{0}' | MaskMode: '{1}' | UnlinkMask: '{2}'.\n{3}", filePath, maskMode, unlinkMask, ex);
            return PhotoshopResponse.FromException(ex);
        }
    }

    /// <summary>
    /// Opens given filepath in Photoshop as new document.
    /// </summary>
    /// <param name="filePath">Full path to the file.</param>
    public PhotoshopResponse OpenAsNewDocument(string filePath)
    {
        try
        {
            dynamic ps = GetPhotoshopComObject();
            ps.Open(filePath);
            return PhotoshopResponse.Success;
        }
        catch (Exception ex)
        {
            _logger.Error("Cannot add file as document to photoshop:\nFilePath: '{0}'.\n{1}", filePath, ex);
            return PhotoshopResponse.FromException(ex);
        }
    }

    /// <summary>
    /// Executes specified action.
    /// </summary>
    /// <param name="actionName">Name of the action.</param>
    /// <param name="set">Set containing that action.</param>
    public PhotoshopResponse ExecuteAction(string actionName, string set)
    {
        try
        {
            dynamic ps = GetPhotoshopComObject();
            ps.DoAction(actionName, set);
            return PhotoshopResponse.Success;
        }
        catch (Exception ex)
        {
            _logger.Error("Cannot execute action '{0}' from set '{1}': {2}", ex);
            return PhotoshopResponse.FromException(ex);
        }
    }

    /// <summary>
    /// Create instance of Photoshop COM Object.
    /// </summary>
    /// <exception cref="PhotoshopException"></exception>
    private dynamic GetPhotoshopComObject()
    {
        if (Process.GetProcessesByName("Photoshop").Length == 0)
            throw new PhotoshopException(Status.NotRunning, "Photoshop process is not running.");

        return Activator.CreateInstance(_photoshopApplicationType) ??
            throw new PhotoshopException(Status.ComCreationFailed, "Photoshop COM object was not created. Activator.CreateInstance returned null.");
    }
}