using MortuusUI;
using System.Windows;
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

    public AssetsWindow()
    {
        InitializeComponent();
        this.PreviewKeyDown += AssetsWindow_PreviewKeyDown;
        this.PreviewKeyUp += AssetsWindow_PreviewKeyUp;
    }

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
}
