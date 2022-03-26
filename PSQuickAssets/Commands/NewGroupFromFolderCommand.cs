using AsyncAwaitBestPractices.MVVM;
using PSQuickAssets.Resources;
using PSQuickAssets.Services;
using PSQuickAssets.Utils.SystemDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSQuickAssets.Commands;

internal class NewGroupFromFolderCommand : IAsyncCommand<object>
{
    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;

    public static string IncludeSubfolders { get; } = "IncludeSubfolders";

    private readonly IStatusService _statusService;

    public NewGroupFromFolderCommand(IStatusService statusService)
    {
        _statusService = statusService;
    }

    public void Execute(object? parameter)
    {
        string[] folderPaths = SystemDialogs.SelectFolder(Localization.Instance["SelectFolder"], SelectionMode.Multiple);

        if (folderPaths.Length == 0)
            return;

        //IsLoading = true;
        //foreach (var path in folderPaths)
        //{
        //    await AddGroupFromFolder(path, includeSubfolders);
        //}
        //IsLoading = false;
        //SaveGroupsAsyncCommand.ExecuteAsync().SafeFireAndForget();
    }

    public void RaiseCanExecuteChanged()
    {
        throw new NotImplementedException();
    }

    public async Task ExecuteAsync(object parameter)
    {
        string[] folderPaths = SystemDialogs.SelectFolder(Localization.Instance["SelectFolder"], SelectionMode.Multiple);

        if (folderPaths.Length == 0)
            return;

        return;

        //using (_statusService.LoadingStatus())
        //{
        //    foreach (var path in folderPaths)
        //    {
        //        await AddGroupFromFolder(path, includeSubfolders);
        //    }
        //}
    }
}
