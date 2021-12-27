using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace PSQuickAssets.Assets;

public enum DuplicateHandling
{
    Deny,
    Allow
}

public class AssetGroup : ObservableObject
{

    /// <summary>
    /// Occurs at any change in the group. E.g adding/removing assets, renaming, etc.
    /// </summary>
    public event Action? GroupChanged;

    /// <summary>
    /// Name of the group.<br></br>
    /// If new name is same as old - name will not be updated.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    public string Name
    {
        get => _name;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(Name), "Name of a group cannot be null.");

            if (!_name.Equals(value))
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                GroupChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// Collection of assets in the group.
    /// </summary>
    public ObservableCollection<Asset> Assets
    {
        get => _assets;
        set
        {
            _assets = value;
            OnPropertyChanged(nameof(Assets));
            GroupChanged?.Invoke();
            _assets.CollectionChanged += (_, _) => GroupChanged?.Invoke();
        }
    }

    /// <summary>
    /// Indicates that group contents are visible.
    /// </summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value; 
                OnPropertyChanged(nameof(IsExpanded));
                GroupChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// Group with empty name and no assets.
    /// </summary>
    public static readonly AssetGroup Empty = new AssetGroup(string.Empty);

    private string _name;
    private bool _isExpanded;
    private ObservableCollection<Asset> _assets;

    /// <summary>
    /// Initializes Asset Group.
    /// </summary>
    /// <param name="name">Name of the group. Cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    public AssetGroup(string name)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));

        _name = name;
        _assets = new ObservableCollection<Asset>();
        _assets.CollectionChanged += (_, _) => GroupChanged?.Invoke();
        _isExpanded = true;
    }

    /// <summary>
    /// Initializes Asset Group with a new guid as a name.
    /// </summary>
    public AssetGroup() : this(Guid.NewGuid().ToString()) { }

    /// <summary>
    /// Adds asset to the group.
    /// </summary>
    /// <param name="asset">Asset to add.</param>
    /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
    /// <returns><see langword="true"/> if added successfully, otherwise <see langword="false"/>.</returns>
    public bool AddAsset(Asset asset, DuplicateHandling duplicateHandling)
    {
        if (duplicateHandling is DuplicateHandling.Deny && HasAsset(asset))
            return false;

        Assets.Add(asset);
        GroupChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Adds multiple assets to the group.
    /// </summary>
    /// <param name="assets">Asset collection to add.</param>
    /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
    /// <returns>List of assets that were NOT added.</returns>
    public List<Asset> AddMultipleAssets(IEnumerable<Asset> assets, DuplicateHandling duplicateHandling)
    {
        var notAddedList = new List<Asset>();

        foreach (var asset in assets)
        {
            if (duplicateHandling is DuplicateHandling.Allow || !HasAsset(asset))
                Assets.Add(asset);
            else
                notAddedList.Add(asset);
        }

        GroupChanged?.Invoke();
        return notAddedList;
    }

    /// <summary>
    /// Checks if asset with the same FILEPATH is already in the group.
    /// </summary>
    /// <returns><see langword="true"/> if asset is in the group. Otherwise <see langword="false"/>.</returns>
    public bool HasAsset(Asset asset)
    {
        return Assets.Any(a => a.Path == asset.Path);
    }

    /// <summary>
    /// Prints group name and all of group's assets.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (Asset asset in Assets)
            sb.Append('\n').Append('\t').Append(asset.FileName);

        return $"Group: {{\"{Name}\"\nAssets: {{{sb}\n}}\n}}";
    }
}