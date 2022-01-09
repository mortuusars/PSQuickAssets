using PSQuickAssets.Assets;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace PSQuickAssets.Commands;
public class OpenInExplorerCommand : ICommand
{
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
    public static OpenInExplorerCommand Instance
    {
        get
        {
            if (_instance is null)
                _instance = new OpenInExplorerCommand();
            return _instance;
        }
    }
    private static OpenInExplorerCommand? _instance;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        var asset = (Asset)parameter;
        string args = $"/e, /select, \"{asset.Path}\"";

        ProcessStartInfo info = new("explorer", args);
        Process.Start(info);
    }
}