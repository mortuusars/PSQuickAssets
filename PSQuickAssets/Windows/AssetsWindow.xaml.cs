using MortuusUI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PSQuickAssets.Windows;

public partial class AssetsWindow : WindowBase
{
    public AssetsWindow()
    {
        InitializeComponent();
        this.PreviewKeyDown += AssetsWindow_PreviewKeyDown;
        this.PreviewKeyUp += AssetsWindow_PreviewKeyUp;
        this.ContentRendered += (_, _) => _windowStateBeforeHiding = WindowState;
    }

    #region Hiding/Showing

    /// <summary>
    /// Hides the Window.
    /// </summary>
    public new void Hide()
    {
        _windowStateBeforeHiding = this.WindowState;
        this.WindowState = WindowState.Minimized;
        base.Hide();
        _isHidden = true;
    }

    private bool _isHidden;
    private WindowState _windowStateBeforeHiding;
    /// <summary>
    /// Shows/Hides the window depending on which state it is currently in.
    /// </summary>
    public void ToggleVisibility()
    {
        //TODO: still some glitches when hiding. Wrong showing after minimized by clicking on task bar.
        bool minimizeInsteadOfHiding = ((App)App.Current).Config.MinimizeWindowInsteadOfHiding;

        if (_isHidden)
        {
            _isHidden = false;

            // Minimize before showing to avoid window flickering:
            WindowState = WindowState.Minimized;

            this.Show();
            this.Activate();
            WindowState = _windowStateBeforeHiding != WindowState.Minimized ? _windowStateBeforeHiding : WindowState.Normal;
        }
        else
        {
            _windowStateBeforeHiding = WindowState;
            WindowState = WindowState.Minimized;
            _isHidden = true;

            if (!minimizeInsteadOfHiding)
            {
                var hidingTimer = new DispatcherTimer();
                hidingTimer.Interval = TimeSpan.FromSeconds(0.3);
                hidingTimer.Tick += (s, e) =>
                {
                    if (_isHidden) // Check if still hidden.
                        this.Hide();

                    hidingTimer.Stop();
                };
                hidingTimer.Start();
            }
        }
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
