using System;
using System.Runtime.InteropServices;

namespace PSQuickAssets.PSInterop.Internal
{
    internal static class ResultCreator
    {
        private const int ERR_GENERAL_PS_ERROR = -2147212704;
        private const int ERR_RETRY_LATER = -2147417846;
        private const int ERR_INVALID_FILE_FORMAT = -2147213504;
        private const int ERR_ILLEGAL_ARGUMENT = -2147220262;
        private const int ERR_INVALID_PATH = -2147220271;
        private const int ERR_USER_CANCELLED = -2147213497;
        private const int ERR_NO_SUCH_ELEMENT = -2147352565;

        /// <summary>
        /// Generates PSResult from COM exception. Sets <see cref="PSResult.Exception"/> to this exception.
        /// </summary>
        internal static PSResult FromCOMException(COMException ex, string filePath = "", string message = "")
        {
            Status status = ex.ErrorCode is ERR_GENERAL_PS_ERROR ? StatusFromGeneralError(ex) : StatusFromCode(ex);
            return new PSResult(status, filePath, message == "" ? ex.Message : message) { Exception = ex };
        }

        /// <summary>
        /// Generates PSResult from regular exception. Sets <see cref="PSResult.Exception"/> to this exception.
        /// </summary>
        internal static PSResult FromException(Exception exception, string filePath = "", string message = "")
        {
            return new PSResult(Status.UnknownException, filePath, message == "" ? exception.Message : message) { Exception = exception };
        }

        private static Status StatusFromCode(COMException exception)
        {
            return exception.ErrorCode switch
            {
                ERR_RETRY_LATER => Status.Busy,
                ERR_USER_CANCELLED => Status.Cancelled,
                ERR_INVALID_FILE_FORMAT => Status.InvalidFileFormat,
                ERR_INVALID_PATH => Status.InvalidFilePath,
                ERR_ILLEGAL_ARGUMENT => Status.IllegalArgument,
                ERR_NO_SUCH_ELEMENT => Status.NoSelection,
                _ => Status.UnknownComException
            };
        }

        private static Status StatusFromGeneralError(COMException exception)
        {
            if (exception.Message.Contains("The command \"Place\" is not currently available"))
                return Status.NoDocumentsOpen;
            else if (exception.Message.Contains("The command \"Duplicate\" is not currently available"))
                return Status.NoSelection;
            else if (exception.Message.Contains("no parser or file format can open the file"))
                return Status.InvalidFileFormat;
            else if (exception.Message.Contains("file could not be found"))
                return Status.FileNotFound;

            return Status.UnknownException;
        }
    }
}
