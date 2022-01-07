using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PSQuickAssets.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace PSQuickAssets.ViewModels
{
    public class AssetGroupViewModel : ObservableObject
    {
        public string Name { get => Group.Name; set => Rename(value); }

        public bool IsExpanded { get => Group.IsExpanded; 
            set
            {
                if (Group.IsExpanded != value)
                {
                    Group.IsExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        public ICollectionView Assets { get => CollectionViewSource.GetDefaultView(Group.Assets); }

        public AssetGroup Group { get; }

        public ICommand ToggleExpandedCommand { get; }
        public ICommand RemoveAssetCommand { get; }

        public AssetGroupViewModel(AssetGroup assetGroup)
        {
            Group = assetGroup;

            ToggleExpandedCommand = new RelayCommand(() => IsExpanded = !IsExpanded);
            RemoveAssetCommand = new RelayCommand<Asset>(a => RemoveAsset(a));
        }

        /// <summary>
        /// Adds asset to the group.
        /// </summary>
        /// <param name="asset">Asset to add.</param>
        /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
        /// <returns><see langword="true"/> if added successfully, otherwise <see langword="false"/>.</returns>
        public bool AddAsset(Asset asset, DuplicateHandling duplicateHandling)
        {
            if (duplicateHandling is DuplicateHandling.Deny && HasAsset(asset.Path))
                return false;

            Group.Assets.Add(asset);
            return true;
        }

        /// <summary>
        /// Adds multiple assets to the group.
        /// </summary>
        /// <param name="assets">Asset collection to add.</param>
        /// <param name="duplicateHandling">Specify how the duplicates should be handled. If set to Deny - asset will not be added if it is already in the group.</param>
        /// <returns>List of assets that were NOT added.</returns>
        public List<Asset> AddAssets(IEnumerable<Asset> assets, DuplicateHandling duplicateHandling)
        {
            var notAddedList = new List<Asset>();

            foreach (var asset in assets)
            {
                if (!AddAsset(asset, duplicateHandling))
                    notAddedList.Add(asset);
            }

            return notAddedList;
        }

        /// <summary>
        /// Removes asset from a group.
        /// </summary>
        /// <param name="asset">Asset to remove.</param>
        /// <returns><see langword="true"/> if successfully removed. Otherwise <see langword="false"/>.</returns>
        public bool RemoveAsset(Asset? asset)
        {
            bool result = asset is not null && Group.Assets.Remove(asset);
            if (result)
                OnPropertyChanged(nameof(Assets));
            return result;
        }

        /// <summary>
        /// Checks if asset with the same FILEPATH is already in the group.
        /// </summary>
        /// <returns><see langword="true"/> if asset is in the group. Otherwise <see langword="false"/>.</returns>
        public bool HasAsset(string filePath)
        {
            if (filePath is null)
                throw new ArgumentNullException(nameof(filePath));

            return Group.Assets.Any(a => a.Path == filePath);
        }

        /// <summary>
        /// Renames asset group.
        /// </summary>
        /// <param name="name"></param>
        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || Group.Name.Equals(name))
                return;

            Group.Name = name;
            OnPropertyChanged(nameof(Name));
        }
    }
}
