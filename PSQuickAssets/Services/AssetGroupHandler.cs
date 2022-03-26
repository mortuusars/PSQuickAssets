using PSQuickAssets.Assets;
using PSQuickAssets.Resources;
using PSQuickAssets.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQuickAssets.Services;

internal class AssetGroupHandler
{
    private readonly ICollection<AssetGroupViewModel> _assetGroupsCollection;
    private readonly AssetManager _assetManager;
    private readonly ILogger _logger;

    public AssetGroupHandler(ICollection<AssetGroupViewModel> assetGroupsCollection, AssetManager assetManager, ILogger logger)
    {
        _assetGroupsCollection = assetGroupsCollection;
        _assetManager = assetManager;
        _logger = logger;
    }

    /// <summary>
    /// Saves asset groups.
    /// </summary>
    /// <returns>Exception if something failed or <see langword="null"/> if saved successfully.</returns>
    public async Task<AssetSavingResult> SaveGroupsAsync()
    {
        AssetGroup[] groups = _assetGroupsCollection.Select(g => g.Group).ToArray();
        return await _assetManager.SaveAsync(groups);
    }

    /// <summary>
    /// Loads stored groups and adds them to collection of groups.
    /// </summary>
    public async Task LoadStoredGroupsAsync()
    {
        await foreach (var group in _assetManager.LoadStoredGroupsAsync())
        {
            if (group is not null)
                _assetGroupsCollection.Add(CreateGroupViewModel(group));
        }
    }

    /// <summary>
    /// Checks if a group with specified name exists in <see cref="AssetGroups"/>.
    /// </summary>
    /// <returns><see langword="true"/> if group with that name exists.</returns>
    public bool IsGroupExists(string groupName) => _assetGroupsCollection.Any(group => group.Name.Equals(groupName));

    /// <summary>
    /// Loads and adds assets to the specified group.
    /// </summary>
    public async Task AddAssetsToGroupAsync(AssetGroupViewModel group, IEnumerable<string> filePaths)
    {
        var assets = await Task.Run(() => _assetManager.Load(filePaths));
        group.AddAssets(assets, DuplicateHandling.Deny);
    }

    /// <summary>
    /// Creates new group with generic name and adds assets to this group.
    /// </summary>
    /// <param name="filePaths">Full paths to the image files.</param>
    public async Task AddGroupFromFilesAsync(IList<string> filePaths)
    {
        if (filePaths.Count == 0)
            return;

        AssetGroupViewModel group = CreateEmptyGroup(GenerateNewGroupName());

        await AddAssetsToGroupAsync(group, filePaths);
    }

    /// <summary>
    /// Adds a new group from folder path. All supported image files will be added to the group as assets.
    /// </summary>
    /// <param name="folderPath">Full path to the folder.</param>
    /// <param name="includeSubfolders">Indicates whether subfolders should be included.</param>
    public async Task AddGroupFromFolderAsync(string folderPath, bool includeSubfolders)
    {
        if (string.IsNullOrEmpty(folderPath))
            return;

        string[] files = Directory.GetFiles(folderPath);

        if (files.Length != 0)
        {
            AssetGroupViewModel groupVM = CreateEmptyGroup(new DirectoryInfo(folderPath).Name);
            await AddAssetsToGroupAsync(groupVM, files);

            if (groupVM.Group.Assets.Count == 0)
                RemoveGroup(groupVM);
        }

        if (includeSubfolders)
        {
            foreach (var folder in Directory.GetDirectories(folderPath))
                await AddGroupFromFolderAsync(folder, includeSubfolders);
        }
    }

    /// <summary>
    /// Creates empty group with specified name. Group will be added to <see cref="AssetGroups"/>.
    /// </summary>
    /// <param name="groupName">Name for a group. If left empty - group name will be generated with "new" suffix.</param>
    public AssetGroupViewModel CreateEmptyGroup(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            groupName = GenerateNewGroupName();

        AssetGroup group = new(groupName);
        var vm = CreateGroupViewModel(group);
        _assetGroupsCollection.Add(vm);
        _logger.Debug("[Asset Groups] Empty asset group created.");
        return vm;
    }

    /// <summary>
    /// Removes group from <see cref="AssetGroups"/>.
    /// </summary>
    /// <param name="assetGroupViewModel">Group to remove.</param>
    /// <returns><see langword="true"/> if group was removed.</returns>
    public void RemoveGroup(AssetGroupViewModel? assetGroupViewModel)
    {
        if (assetGroupViewModel is not null && _assetGroupsCollection.Remove(assetGroupViewModel))
            _logger.Information($"[Asset Groups] Removed group '{assetGroupViewModel.Name}'.");
    }

    /// <summary>
    /// Creates a group view model from existing <see cref="AssetGroup"/>.
    /// </summary>
    /// <returns>Created group viewmodel.</returns>
    public AssetGroupViewModel CreateGroupViewModel(AssetGroup assetGroup)
    {
        string groupName = assetGroup.Name;
        if (IsGroupExists(groupName))
        {
            groupName = $"{groupName} {Localization.Instance["New"]}";
            assetGroup.Name = groupName;
        }

        _logger.Information($"[Asset Groups] Group view model '{groupName}' created.");
        return new AssetGroupViewModel(assetGroup, _logger);
    }

    /// <summary>
    /// Generates generic name for a new group.
    /// </summary>
    /// <returns>Generated name.</returns>
    private string GenerateNewGroupName()
    {
        string group = Localization.Instance["Group"];
        int genericGroupCount = _assetGroupsCollection.Select(g => g.Name.Contains(group, StringComparison.OrdinalIgnoreCase)).Count();
        return genericGroupCount == 0 ? group : group + " " + (genericGroupCount + 1);
    }
}
