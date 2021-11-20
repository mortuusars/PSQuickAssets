using System.Collections.ObjectModel;

namespace PSQuickAssets.Models
{
    public class TestAssetGroup
    {
        public string Name { get; set; }
        public ObservableCollection<Asset> Assets { get; set; }

        public TestAssetGroup()
        {
            Name = "Test Group";
            Assets = new ObservableCollection<Asset>
            {
                new Asset()
                {
                    FilePath = @"F:\PROJECTS\PSQuickAssets\PSQASource\PSQuickAssets\Resources\Images\image.png",
                    ThumbnailPath = @"F:\PROJECTS\PSQuickAssets\PSQASource\PSQuickAssets\Resources\Images\image.png"
                },
                new Asset()
                {
                    FilePath = @"F:\PROJECTS\PSQuickAssets\PSQASource\PSQuickAssets\Resources\Images\image.png",
                    ThumbnailPath = @"F:\PROJECTS\PSQuickAssets\PSQASource\PSQuickAssets\Resources\Images\image.png"
                }
            };
        }
    }
}
