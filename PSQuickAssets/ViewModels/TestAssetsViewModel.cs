using PSQuickAssets.Assets;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.ViewModels
{
    internal class TestAssetsViewModel
    {
        public ObservableCollection<AssetGroupViewModel> AssetGroups { get; }

        public TestAssetsViewModel()
        {
            AssetGroups = new ObservableCollection<AssetGroupViewModel>();

            var group = new AssetGroup("Test");
            var groupVM = new AssetGroupViewModel(group, null, new MLogger.Logger(MLogger.LogLevel.Debug));

            for (int i = 0; i < 8; i++)
            {
                groupVM.AddAsset(new Asset() { Path = "C://asd/" + i, Thumbnail = CreateThumb() }, DuplicateHandling.Allow);
            }

            AssetGroups.Add(groupVM);
            AssetGroups.Add(groupVM);
        }

        private BitmapImage CreateThumb()
        {
            return ThumbnailCreator.FromFile(@"D:\CODE PROJECTS\PSQuickAssets\PSQASource\PSQuickAssets\Resources\Images\test.jpg", 60, ConstrainTo.Height);
        }
    }
}
