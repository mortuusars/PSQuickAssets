using PSQuickAssets.Controls;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSQuickAssets.Views;

public partial class AssetGroupView : UserControl
{
    public AssetGroupView()
    {
        InitializeComponent();
    }

    private void EditableTextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Enter group rename state.
        if (e.ChangedButton == MouseButton.Left)
        {
            e.Handled = true;
            var etb = (EditableTextBlock)sender;
            etb.IsEditing = true;
        }
    }
}