using PSQuickAssets.Assets;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

public class ShowAssetInExplorerCommand : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public static ShowAssetInExplorerCommand Instance
    {
        get
        {
            if (_instance is null)
                _instance = new ShowAssetInExplorerCommand();
            return _instance;
        }
    }
    private static ShowAssetInExplorerCommand? _instance;

    private ShowAssetInExplorerCommand() { }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

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