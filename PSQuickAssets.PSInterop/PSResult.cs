using PSQuickAssets.PSInterop.Internal;
using System;
using System.Runtime.InteropServices;

namespace PSQuickAssets.PSInterop;

public record PSResult
{
    /// <summary>
    /// Describes status of a PS call. Contains all possible results.
    /// </summary>
    public Status Status { get; init; }

    /// <summary>
    /// Image Filepath that was used when calling PS. Can be empty if call was not file related.
    /// </summary>
    public string FilePath { get; init; }
    
    /// <summary>
    /// Message that contains additional information about result.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Contains Exception that was produced in a PS call that failed.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Indicates whether result was successful.
    /// </summary>
    public bool IsSuccessful { get => Status is Status.Success; }

    public PSResult(Status status, string filePath, string message)
    {
        Status = status;
        FilePath = filePath;
        Message = message;
    }

    public PSResult(Status status, string filePath) : this(status, filePath, string.Empty) { }
    public PSResult(Status status) : this(status, string.Empty, string.Empty) { }

    /// <summary>
    /// Generates PSResult from COM Exception.
    /// </summary>
    internal static PSResult FromException(COMException exception, string filePath = "", string message = "")
    {
        return ResultCreator.FromCOMException(exception, filePath, message);
    }

    /// <summary>
    /// Generates PSResult from regular Exception.
    /// </summary>
    internal static PSResult FromException(Exception exception, string filePath = "", string message = "")
    {
        return ResultCreator.FromException(exception, filePath, message);
    }
}
