namespace PSQuickAssets.PSInterop
{
    public record PSResult(PSStatus Status, string FilePath, string ResultMessage);
}
