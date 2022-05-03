using PSQA.Core;
using PSQuickAssets.PSInterop.Internal;
using PureLib;
using Serilog;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PSQuickAssets.PSInterop;

public enum PSErrorCode
{
    ERR_RETRY_LATER = -2147417846,
    ERR_NO_SUCH_ELEMENT = -2147352565,
    ERR_INVALID_PATH = -2147220271,
    ERR_ILLEGAL_ARGUMENT = -2147220262,
    ERR_INVALID_FILE_FORMAT = -2147213504,
    ERR_USER_CANCELLED = -2147213497,
    ERR_GENERAL_PS_ERROR = -2147212704
}

public record PhotoshopResponse(Status Status, string Message)
{
    public static PhotoshopResponse Success { get => new PhotoshopResponse(Status.Success, string.Empty); }

    public static PhotoshopResponse FromException(Exception ex)
    {
        if (ex is PhotoshopException psEx)
            return new PhotoshopResponse(psEx.Status, psEx.Message);

        Status status = ex is COMException comEx ? StatusFromComErrorCode(comEx.ErrorCode) : Status.UnknownException;
        return new PhotoshopResponse(status, ex.Message);
    }

    private static Status StatusFromComErrorCode(int errorCode)
    {
        try
        {
            PSErrorCode psErrorCode = (PSErrorCode)errorCode;
            return psErrorCode switch
            {
                PSErrorCode.ERR_GENERAL_PS_ERROR => Status.Failed,
                PSErrorCode.ERR_RETRY_LATER => Status.Busy,
                PSErrorCode.ERR_INVALID_FILE_FORMAT => Status.InvalidFileFormat,
                PSErrorCode.ERR_ILLEGAL_ARGUMENT => Status.InvalidFileFormat,
                PSErrorCode.ERR_INVALID_PATH => Status.InvalidFilePath,
                PSErrorCode.ERR_USER_CANCELLED => Status.Cancelled,
                PSErrorCode.ERR_NO_SUCH_ELEMENT => Status.NoSelection,
                _ => Status.UnknownException,
            };
        }
        catch (Exception)
        {
            return Status.UnknownException;
        }
    }
}

public class AnotherPhotoshopInterop : IAnotherPhotoshopInterop
{
    private const string _selectionChannelName = "QuickAssetsMask";
    private readonly Type _photoshopApplicationType;
    private readonly ILogger _logger;

    public AnotherPhotoshopInterop(ILogger logger)
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
