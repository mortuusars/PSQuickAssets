using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

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

    private void HotkeySettingItem_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && HotkeyPicker.FindChildOfType<TextBox>() is TextBox hotkeyPickerBox)
        {
            Keyboard.Focus(hotkeyPickerBox);
            e.Handled = true;
        }
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Fade-in animation of tab content.

        if (e.OriginalSource is not TabControl tabControl || !tabControl.IsLoaded)
            return; // This event handler catches SelectionChanged of child controls too. We interested only in TabControl events. 

        DoubleAnimation anim = new(0.0d, 1.0d, new Duration(TimeSpan.FromSeconds(0.15)));
        Storyboard st = new();
        tabControl.FindChildByName("contentPanel")?
            .CastTo<Border>()
            .BeginAnimation(Border.OpacityProperty, anim);
    }
}
