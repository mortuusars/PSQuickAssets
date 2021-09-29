using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;

namespace PSQuickAssets.Models
{
    public class AssetGroup : ObservableObject
    {
        public string Name { get; private set; }

        public ObservableCollection<Asset> Assets { get; }

        public AssetGroup(string name)
        {
            Name = name;
            Assets = new ObservableCollection<Asset>();
        }

        public bool AddAsset(Asset asset, bool allowDuplicates = false)
        {
            if (asset is null || (!allowDuplicates && IsInGroup(asset)))
                return false;

            Assets.Add(asset);
            return true;
        }

        public bool IsInGroup(Asset asset)
        {
            return Assets.Contains(asset);
        }

        public bool Rename(string name)
        {
            if (Name != name)
            {
                Name = name;
                OnPropertyChanged(nameof(Name));
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (Asset asset in Assets)
                sb.Append("\n").Append("\t").Append(asset.FileName);

            return $"Group Name: \"{Name}\"\nAssets: {{{sb}\n}}";
        }
    }
}
