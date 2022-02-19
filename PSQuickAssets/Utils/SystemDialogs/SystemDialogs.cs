using Ookii.Dialogs.Wpf;
using System;
using System.Windows.Markup;

namespace PSQuickAssets.Utils.SystemDialogs
{
    public enum SelectionMode
    {
        Single, Multiple
    }

    /// <summary>
    /// Holds filters for Select File Dialog.
    /// </summary>
    public static class FileFilters
    {
        public const string Images = "Images (*.bmp; *.jpg; *.jpeg; *.gif; *.tiff; *.tif; *.psd; *.psb)|*.bmp; *.jpg; *.jpeg; *.gif; *.tiff; *.tif; *.psd; *.psb";
        public const string AllFiles = "All files(*.*)|*.*";
    }

    /// <summary>
    /// Provides functionality to open system dialogs.
    /// </summary>
    public static class SystemDialogs
    {
        /// <summary>
        /// Asks user to select file/s.
        /// </summary>
        /// <param name="title">Title of the dialog.</param>
        /// <param name="filter">Controlls what type of files can be seen in dialog.
        /// Use <see cref="FileFilters"/> for predefined filters. 
        /// Multiple filters can be used together if separated with "|"(pipe) symbol.
        /// If multiple filters is provided - first will be selected by default.</param>
        /// <param name="mode">Selection mode.</param>
        /// <returns>Array of selected filepaths. Empty array is returned when nothing was selected or user cancelled operation.</returns>
        public static string[] SelectFiles(string title, string filter, SelectionMode mode)
        {
            var dialog = new VistaOpenFileDialog()
            {
                Title = title,
                Filter = filter,
                Multiselect = mode == SelectionMode.Multiple
            };

            bool? result = dialog.ShowDialog();

            if (result is true)
                return dialog.FileNames;
            else
                return Array.Empty<string>();
        }

        /// <summary>
        /// Asks user to select a folder.
        /// </summary>
        /// <param name="title">Title of the dialog.</param>
        /// <param name="mode">Selection mode.</param>
        /// <returns>Array of selected paths. (One element if <see cref="SelectionMode.Single"/>). Empty array if none selected or user cancelled operation.</returns>
        public static string[] SelectFolder(string title, SelectionMode mode)
        {
            VistaFolderBrowserDialog dialog = new()
            {
                UseDescriptionForTitle = true,
                Description = title,
                Multiselect = mode == SelectionMode.Multiple,
                ShowNewFolderButton = false,
            };

            return dialog.ShowDialog() is true ? dialog.SelectedPaths : Array.Empty<string>();
        }
    }
}
