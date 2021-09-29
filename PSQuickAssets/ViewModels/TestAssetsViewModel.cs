using PSQuickAssets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQuickAssets.ViewModels
{
    public class TestAssetsViewModel
    {
        public ObservableCollection<AssetGroup> AssetGroups { get; }

        public TestAssetsViewModel()
        {
            AssetGroups = new ObservableCollection<AssetGroup>();

            var group = new AssetGroup("Test Asd");

            for (int i = 0; i < 4; i++)
            {
                group.AddAsset(new Asset() { ThumbnailPath = "../../../../Resources/Images/test.jpg", FilePath = "C://asd/" + i });
            }

            AssetGroups.Add(group);
            AssetGroups.Add(group);
        }
    }
}
