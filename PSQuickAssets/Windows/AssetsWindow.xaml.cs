using MortuusUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSQuickAssets.Windows;

public partial class AssetsWindow : WindowBase
{
    public bool IsCtrlPressed
    {
        get { return (bool)GetValue(IsCtrlPressedProperty); }
        set { SetValue(IsCtrlPressedProperty, value); }
    }

    public static readonly DependencyProperty IsCtrlPressedProperty =
        DependencyProperty.Register(nameof(IsCtrlPressed), typeof(bool), typeof(AssetsWindow), new PropertyMetadata(false));

    private bool _isHidden;
    private WindowState _windowStateBeforeHiding;

    public AssetsWindow()
    {
        InitializeComponent();
        this.PreviewKeyDown += AssetsWindow_PreviewKeyDown;
        this.PreviewKeyUp += AssetsWindow_PreviewKeyUp;
    }

    //public new void Show()
    //{
    //    base.Show();

    //    if (this.WindowState == WindowState.Minimized)
    //        this.WindowState = WindowState.Normal;
    //}

    //public new void Hide()
    //{
    //    _windowStateBeforeHiding = this.WindowState;
    //    base.Hide();
    //    _isHidden = true;
    //}

    //private void UnhideWindow(object? sender, System.EventArgs e)
    //{
    //    this.WindowState = _windowStateBeforeHiding;
    //}

    private void AssetsWindow_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.LeftCtrl or Key.RightCtrl)
            IsCtrlPressed = false;
    }

    private void AssetsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.LeftCtrl or Key.RightCtrl)
            IsCtrlPressed = true;
    }

    private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Properly raise mouse scroll event without interfering with other controls when .
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
