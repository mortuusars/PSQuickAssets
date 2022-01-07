using PSQuickAssets.Assets;
using PSQuickAssets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.ViewModels
{
    public class TestAssetsViewModel
    {
        public ObservableCollection<AssetGroupViewModel> AssetGroups { get; }

        public TestAssetsViewModel()
        {
            //AssetGroups = new ObservableCollection<AssetGroupViewModel>();

            //var group = new AssetGroup();

            //for (int i = 0; i < 8; i++)
            //{
            //    group.AddAsset(new Asset() { Path = "C://asd/" + i, Thumbnail = CreateThumb() }, DuplicateHandling.Allow);
            //}

            //AssetGroups.Add(group);
            //AssetGroups.Add(group);
        }

        private BitmapImage CreateThumb()
        {
            return ThumbnailCreator.FromFile(@"D:\CODE PROJECTS\PSQuickAssets\PSQASource\PSQuickAssets\Resources\Images\test.jpg", 60, ConstrainTo.Height);
        }
    }
}
