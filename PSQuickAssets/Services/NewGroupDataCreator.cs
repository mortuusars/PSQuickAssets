﻿using PSQuickAssets.Models;
using System.IO;

namespace PSQuickAssets.Services;

internal class NewGroupDataCreator
{
    public Func<int, bool> ThresholdValidation { get; set; } = (_) => true;

    /// <summary>
    /// Creates collection of <see cref="NewGroupData"/> from list of paths.<br></br>
    /// Files will be added first, then folders.
    /// </summary>
    /// <param name="paths">List of file/folder paths.</param>
    /// <param name="includeSubfolders">If set to <see langword="true"/> - subfolders of top-level folders will be added aswell.</param>
    public IEnumerable<NewGroupData> Create(IEnumerable<string> paths, bool includeSubfolders = false)
    {
        ArgumentNullException.ThrowIfNull(paths);

        var (folders, files) = GetFoldersAndFilesFromPaths(paths);

        List<NewGroupData> result = new();

        //Add files:
        if (files.Count > 0)
            result.Add(new NewGroupData(files[0].Directory?.Name, files.Select(f => f.FullName)));

        //Add folders:
        foreach (DirectoryInfo directory in folders)
        {
            if (includeSubfolders)
                CreateGroupDataRecursive(result, directory);
            else
            {
                FileInfo[] dirFiles = directory.GetFiles();
                if (dirFiles.Length > 0)
                    result.Add(new NewGroupData(directory.Name, dirFiles.Select(f => f.FullName)));
            }
        }

        return CheckAndConfirmCount(result) ? result : Enumerable.Empty<NewGroupData>();
    }

    // Recursively adds each directory and subdirectories to the list.
    private static void CreateGroupDataRecursive(IList<NewGroupData> newGroupDatas, DirectoryInfo directory)
    {
        FileInfo[] files = directory.GetFiles();
        if (files.Length > 0)
            newGroupDatas.Add(new NewGroupData(directory.Name, files.Select(f => f.FullName)));

        foreach (var dir in directory.GetDirectories())
            CreateGroupDataRecursive(newGroupDatas, dir);
    }

    /// <summary>
    /// Counts how many assets will be added (across groups) and if it is larger than threshold - confirms creation by calling <see cref="ThresholdValidation"/> func.
    /// </summary>
    /// <param name="groupDatas">Source collection.</param>
    /// <returns><see langword="true"/> if asset count is less than threshold.<br></br>
    /// <see langword="true"/> if user confirms (when larger than threshold).<br></br>
    /// <see langword="false"/> otherwise.</returns>
    private bool CheckAndConfirmCount(IEnumerable<NewGroupData> groupDatas)
    {
        int assetCount = 0;
        foreach (var data in groupDatas)
            assetCount += data.FilePaths.Count();

        return ThresholdValidation(assetCount);
    }

    private (IList<DirectoryInfo> directories, IList<FileInfo> files) GetFoldersAndFilesFromPaths(IEnumerable<string> filePaths)
    {
        List<DirectoryInfo> directories = new();
        List<FileInfo> files = new();

        foreach (var path in filePaths)
        {
            if (File.Exists(path))
                files.Add(new FileInfo(path));
            else if (Directory.Exists(path))
                directories.Add(new DirectoryInfo(path));
        }

        return (directories, files);
    }
}
