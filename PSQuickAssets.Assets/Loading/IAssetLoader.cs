namespace PSQuickAssets.Assets
{
    internal interface IAssetLoader
    {
        Asset Load(string filePath);
    }
}