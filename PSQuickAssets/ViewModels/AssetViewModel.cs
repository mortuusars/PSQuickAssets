using CommunityToolkit.Mvvm.ComponentModel;
using PSQA.Assets.Repository;
using PSQA.Core;
using PSQuickAssets.Utils;
using System.Drawing;
using System.IO;

namespace PSQuickAssets.ViewModels;

[INotifyPropertyChanged]
public partial class AssetViewModel
{
    public string FilePath { get => Asset.Path; }
    public long FileSize { get; }
    public bool Exists { get; }
    public string Name { get => Path.GetFileNameWithoutExtension(FilePath); }
    public string FileName { get => Path.GetFileName(FilePath); }
    public string Extension { get => Path.GetExtension(FilePath); }
    public Size Dimensions { get => AssetHelper.GetImageDimensions(FilePath); }

    public int Uses
    {
        get => Asset.Uses;
        set
        {
            Asset.Uses = value;
            OnPropertyChanged();
            _assetRepository.Save();
        }
    }

    public Asset Asset { get; }

    private readonly AssetRepository _assetRepository;

    public AssetViewModel(Asset asset, AssetRepository assetRepository)
    {
        Asset = asset;
        _assetRepository = assetRepository;

        FileInfo file = new(FilePath);
        Exists = file.Exists;
        FileSize = Exists ? file.Length : 0;
    }
}
