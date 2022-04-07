using MortuusUI;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PSQuickAssets.Windows;

public partial class AssetsWindow : WindowBase
{
    public bool ShouldMinimizeInsteadOfHiding
    {
        get { return (bool)GetValue(ShouldMinimizeInsteadOfHidingProperty); }
        set { SetValue(ShouldMinimizeInsteadOfHidingProperty, value); }
    }

    public static readonly DependencyProperty ShouldMinimizeInsteadOfHidingProperty =
        DependencyProperty.Register(nameof(ShouldMinimizeInsteadOfHiding), typeof(bool), typeof(AssetsWindow), new PropertyMetadata(false));

    public AssetsWindow()
    {
        InitializeComponent();
        this.PreviewKeyDown += AssetsWindow_PreviewKeyDown;
        this.PreviewKeyUp += AssetsWindow_PreviewKeyUp;
        this.StateLoaded += (_, _) =>
        {
            _restoreWindowState = WindowState;
        };
    }

    #region Hiding/Showing

    private bool _isHidden;
    private WindowState _restoreWindowState;

    public new void Hide()
    {
        _isHidden = true;
        _restoreWindowState = WindowState;
        WindowState = WindowState.Minimized;

        if (!ShouldMinimizeInsteadOfHiding)
        {
            // Hide window after a delay to allow minimizing animation to play.
            var hidingTimer = new DispatcherTimer();
            hidingTimer.Interval = TimeSpan.FromSeconds(0.3);
            hidingTimer.Tick += (s, e) =>
            {
                if (_isHidden) // Check if still hidden.
                    base.Hide();

                hidingTimer.Stop();
            };
            hidingTimer.Start();
        }
    }

    public new void Show()
    {
        _isHidden = false;
        base.Show();
        WindowState = _restoreWindowState == WindowState.Minimized ? WindowState.Normal : _restoreWindowState;
    }

    public void ToggleVisibility()
    {
        if (_isHidden)
            Show();
        else if (WindowState == WindowState.Minimized)
            WindowState = _restoreWindowState == WindowState.Minimized ? WindowState.Normal : _restoreWindowState;
        else
            Hide();
    }

    #endregion

    #region CtrlHeld
    //TODO: This is ugly:
    public bool IsCtrlPressed
    {
        get { return (bool)GetValue(IsCtrlPressedProperty); }
        set { SetValue(IsCtrlPressedProperty, value); }
    }

    public static readonly DependencyProperty IsCtrlPressedProperty =
        DependencyProperty.Register(nameof(IsCtrlPressed), typeof(bool), typeof(AssetsWindow), new PropertyMetadata(false));

    private void AssetsWindow_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.LeftCtrl or Key.RightCtrl)
            IsCtrlPressed = false;

        if (e.Key is Key.Space)
        {
            //var active = App.Current.GetActiveWindow();
        }
    }

    private void AssetsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.LeftCtrl or Key.RightCtrl)
            IsCtrlPressed = true;
    }

    #endregion

    //TODO: Move to attached property?
    private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Properly raise mouse scroll event without interfering with other controls when Ctrl, Shift, or Alt is pressed.
        if (sender is ItemsControl && !e.Handled && Keyboard.Modifiers != ModifierKeys.None)
        {
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            var parent = ((Control)sender).Parent as UIElement;
            parent?.RaiseEvent(eventArg);
        }
    }
}
