using PSQuickAssets.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

public class OpenAssetWithShellCommand : ICommand
{
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
    public static OpenAssetWithShellCommand Instance
    {
        get
        {
            if (_instance is null)
                _instance = new OpenAssetWithShellCommand();
            return _instance;
        }
    }
    private static OpenAssetWithShellCommand? _instance;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        var asset = (Asset)parameter;

        ProcessStartInfo info = new(asset.Path) { UseShellExecute = true };
        Process.Start(info);
    }
}