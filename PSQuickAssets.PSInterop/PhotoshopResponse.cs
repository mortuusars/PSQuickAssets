using System;
using System.Runtime.InteropServices;

namespace PSQuickAssets.PSInterop;

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
