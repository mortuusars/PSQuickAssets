using System.Windows.Input;

namespace PSQuickAssets.Resources;

internal static class Shortcuts
{
    public static KeyGesture Settings { get; } = new KeyGesture(Key.P, ModifierKeys.Control, "Ctrl+P");
    public static KeyGesture HideMainWindow { get; } = new KeyGesture(Key.Escape, ModifierKeys.None, "Esc");
    public static KeyGesture Exit { get; } = new KeyGesture(Key.Escape, ModifierKeys.Shift, "Shift+Esc");

    public static KeyGesture CollapseAssetGroup { get; } = new KeyGesture(Key.M, ModifierKeys.Control, "Ctrl+M");
    public static KeyGesture RenameAssetGroup { get; } = new KeyGesture(Key.F2, ModifierKeys.None, "F2");


    public static MouseGesture AssetAdd { get; } = new MouseGesture(MouseAction.LeftClick);
    public static MouseGesture AssetAddAsNewDocument { get; } = new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control);
    public static KeyGesture AssetCopyFilePath { get; } = new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl+C");
    public static KeyGesture AssetOpenInShell { get; } = new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl+O");
    public static KeyGesture AssetShowInExplorer { get; } = new KeyGesture(Key.E, ModifierKeys.Control, "Ctrl+E");

}
