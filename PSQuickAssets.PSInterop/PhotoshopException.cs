using System;

namespace PSQuickAssets.PSInterop;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
public class PhotoshopException : Exception
{
    public Status Status { get; }

    public PhotoshopException(Status status, string? message) : base(message)
    {
        Status = status;
    }
}
