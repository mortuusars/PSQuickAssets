using PureUI;
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
        PreviewKeyDown += SettingsWindow_PreviewKeyDown;
    }

    private void SettingsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if(e.Key == Key.Space)
        {
            var foc = Keyboard.FocusedElement;
        }
    }
    private void AddActionButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        e.Handled = true;
        if (!AddingActionPopup.IsOpen)
            AddingActionPopup.IsOpen = true;
        Keyboard.Focus(AddingActionNameBox);
    }

    private void AddingActionCancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        e.Handled = true;
        AddingActionPopup.IsOpen = false;
    }

    // Closes both popups.
    private void Popup_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            sender.CastTo<Popup>().IsOpen = false;
        }
    }

    private void AddingActionAddButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        AddingActionPopup.IsOpen = false;
    }

    private void AddingActionPopup_Opened(object sender, EventArgs e)
    {
        Keyboard.Focus(AddingActionNameBox);
    }

    private void MoveFocusDown_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);

        if (e.OriginalSource is FrameworkElement sourceElement)
            sourceElement.MoveFocus(request);
        else
            SystemSounds.Asterisk.Play();
    }
}
