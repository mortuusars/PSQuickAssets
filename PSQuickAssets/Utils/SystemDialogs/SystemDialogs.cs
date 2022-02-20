﻿using Ookii.Dialogs.Wpf;
using System;

namespace PSQuickAssets.Utils.SystemDialogs;

public enum SelectionMode
{
    Single, Multiple
}

/// <summary>
/// Holds filters for Select File Dialog.
/// </summary>
public static class FileFilters
{
    /// <summary>
    /// Filter for allowed image formats.
    /// </summary>
    public static string Images { get; } = "Images (*.bmp; *.jpg; *.jpeg; *.gif; *.tiff; *.tif; *.psd; *.psb)|*.bmp; *.jpg; *.jpeg; *.gif; *.tiff; *.tif; *.psd; *.psb";
    /// <summary>
    /// Filter for all files.
    /// </summary>
    public static string AllFiles { get; } = "All files(*.*)|*.*";
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
    /// Use <see cref="FileFilters"/> for predefined filters.<br></br>
    /// Multiple filters can be used together if separated with "|"(pipe) symbol.
    /// If multiple filters is provided - first will be selected by default.</param>
    /// <param name="mode">Selection mode.</param>
    /// <returns>Array of selected filepaths. Empty array is returned when nothing was selected or user cancelled operation.</returns>
    public static string[] SelectFiles(string title, string filter, SelectionMode mode)
    {
        VistaOpenFileDialog dialog = new();
        dialog.Filter = filter;
        dialog.Multiselect = mode == SelectionMode.Multiple;
        if (!string.IsNullOrWhiteSpace(title))
            dialog.Title = title;

        return dialog.ShowDialog() is true ? dialog.FileNames : Array.Empty<string>();
    }

    /// <summary>
    /// Asks user to select a folder.
    /// </summary>
    /// <param name="title">Title of the dialog.</param>
    /// <param name="mode">Selection mode.</param>
    /// <param name="showNewFolderButton">Show the "New Folder" button.</param>
    /// <returns>Array of selected paths. (One element if <see cref="SelectionMode.Single"/>). Empty array if none selected or user cancelled operation.</returns>
    public static string[] SelectFolder(string title, SelectionMode mode, bool showNewFolderButton = false)
    {
        VistaFolderBrowserDialog dialog = new();
        dialog.Multiselect = mode == SelectionMode.Multiple;
        dialog.ShowNewFolderButton = showNewFolderButton;
        if (!string.IsNullOrWhiteSpace(title))
        {
            dialog.UseDescriptionForTitle = true;
            dialog.Description = title;
        }

        return dialog.ShowDialog() is true ? dialog.SelectedPaths : Array.Empty<string>();
    }
}
