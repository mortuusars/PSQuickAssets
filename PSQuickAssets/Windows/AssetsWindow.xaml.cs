using PureUI;
using PureUI.Resources;
using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace PSQuickAssets.Windows;

public partial class AssetsWindow : PureWindow
{
    public bool HideIconFromTaskbarWhenHidden
    {
        get { return (bool)GetValue(HideIconFromTaskbarWhenHiddenProperty); }
        set { SetValue(HideIconFromTaskbarWhenHiddenProperty, value); }
    }

    public static readonly DependencyProperty HideIconFromTaskbarWhenHiddenProperty =
        DependencyProperty.Register(nameof(HideIconFromTaskbarWhenHidden), typeof(bool), typeof(AssetsWindow), new PropertyMetadata(false));

    public AssetsWindow()
    {
        InitializeComponent();
    }

    #region Hiding/Showing

    private bool _isHidden;
    private WindowState _restoreWindowState;

    public new void Hide()
    {
        _isHidden = true;
        _restoreWindowState = WindowState;
        WindowState = WindowState.Minimized;

        if (HideIconFromTaskbarWhenHidden)
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
        if (WindowState == WindowState.Minimized)
            WindowState = WindowState.Normal;
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

    #region Drag&Drop

    public bool IsDragOver
    {
        get { return (bool)GetValue(IsDragOverProperty); }
        set { SetValue(IsDragOverProperty, value); }
    }

    public static readonly DependencyProperty IsDragOverProperty =
        DependencyProperty.Register(nameof(IsDragOver), typeof(bool), typeof(AssetsWindow), new PropertyMetadata(false));

    private void AssetsWin_PreviewDragEnter(object sender, DragEventArgs e)
    {
        IsDragOver = true;
    }

    private void AssetsWin_PreviewDragLeave(object sender, DragEventArgs e) => IsDragOver = false;
    private void AssetsWin_PreviewDrop(object sender, DragEventArgs e) => IsDragOver = false;

    private void AssetsWin_PreviewDragOver(object sender, DragEventArgs e)
    {
        // Allow only file drop:
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.None;
    }

    #endregion
}
