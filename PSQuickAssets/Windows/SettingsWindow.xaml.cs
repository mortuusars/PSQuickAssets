using System.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PSQuickAssets.Windows;

public partial class SettingsWindow : PureWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void MoveFocusDown_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);

        if (e.OriginalSource is FrameworkElement sourceElement)
            sourceElement.MoveFocus(request);
        else
            SystemSounds.Asterisk.Play();
    }

    private void Popup_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            sender.CastTo<Popup>().IsOpen = false;
        }
    }

    private void AddingActionPopup_Opened(object sender, EventArgs e)
    {
        Keyboard.Focus(AddingActionNameBox);
    }
}
