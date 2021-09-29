using System;
using Ookii.Dialogs.Wpf;

namespace PSQuickAssets.Services
{
    public interface IOpenDialogService
    {
        /// <summary>
        /// Prompts uset to select file(s).
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filter"></param>
        /// <param name="selectMultiple"></param>
        /// <returns>FilePaths of selected files.</returns>
        string[] ShowSelectFilesDialog(string title, string filter, bool selectMultiple);
        /// <summary>
        /// Prompts user to select a folder.
        /// </summary>
        /// <returns>Selected folder path.</returns>
        string ShowSelectFolderDialog();
    }

    public class OpenDialogService : IOpenDialogService
    {
        public string[] ShowSelectFilesDialog(string title, string filter, bool selectMultiple)
        {
            Microsoft.Win32.OpenFileDialog dialog = new();
            dialog.Title = title;
            dialog.Multiselect = selectMultiple;
            dialog.Filter = filter;
            bool? result = dialog.ShowDialog();

            if (result is true)
                return dialog.FileNames;
            else
                return Array.Empty<string>();

        }

        public string ShowSelectFolderDialog()
        {
            VistaFolderBrowserDialog dialog = new();
            return dialog.ShowDialog() is true ? dialog.SelectedPath : string.Empty;
        }
    }
}
