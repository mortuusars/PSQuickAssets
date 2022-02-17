using AsyncAwaitBestPractices;
using PSQuickAssets.Controls;
using PSQuickAssets.Views;
using PSQuickAssets.Windows.State;
using PSQuickAssets.WPF;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PSQuickAssets.Windows
{
    public partial class MainWindow : Window
    {
        private const string _MAIN_VIEW_STATE_FILE = "state.json";
        private static readonly TimeSpan _fadeInDuration = TimeSpan.FromMilliseconds(80);
        private static readonly TimeSpan _fadeOutDuration = TimeSpan.FromMilliseconds(150);

        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register(nameof(IsShowing), typeof(bool), typeof(MainWindow), new PropertyMetadata(false, OnIsShowingChanged));
        
        public static readonly DependencyProperty IsCtrlHeldProperty =
            DependencyProperty.Register(nameof(IsCtrlHeld), typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        /// <summary>
        /// Indicates whether the window is currently showing.
        /// </summary>
        public bool IsShowing
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        public bool IsCtrlHeld
        {
            get { return (bool)GetValue(IsCtrlHeldProperty); }
            set { SetValue(IsCtrlHeldProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            IsVisibleChanged += OnVisibilityChanged;

            RestoreState();
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        public new void Show()
        {
            base.Show();
            Activate();
            IsShowing = true;
        }

        /// <summary>
        /// Plays short fade out animation before hiding.
        /// </summary>
        public void HideWithAnimation() => IsShowing = false;

        protected override void OnDeactivated(EventArgs e)
        {
            bool isOtherAppWindowOpen = false;

            foreach (var window in App.Current.Windows)
            {
                var w = (Window)window;
                if (w.IsVisible && w != this)
                    isOtherAppWindowOpen = true;
            }

            if (((App)App.Current).Config.HideWindowIfClickedOutside && !isOtherAppWindowOpen)
                HideWithAnimation();
            base.OnDeactivated(e);
        }

        private static void OnIsShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not MainWindow window || e.NewValue == e.OldValue)
                return;

            if (e.NewValue is true)
            {
                window.LayoutRoot.Opacity = 0;
                var anim = new DoubleAnimation(0.0, 1.0, new Duration(_fadeInDuration));
                anim.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
                window.LayoutRoot.BeginAnimation(Grid.OpacityProperty, anim);
            }
            else if (e.NewValue is false)
            {
                var anim = new DoubleAnimation(1.0, 0.0, new Duration(_fadeOutDuration));
                anim.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
                anim.Completed += (_, _) => { window.Hide(); };
                window.LayoutRoot.BeginAnimation(Grid.OpacityProperty, anim);
            }
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                SaveState().SafeFireAndForget();
        }

        #region STATE

        private async Task SaveState()
        {
            var state = new ViewState()
            {
                Left = this.Left,
                Top = this.Top,
                Width = this.Width,
                Height = this.Height,
            };

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions() { WriteIndented = true });

            try
            {
                await File.WriteAllTextAsync(_MAIN_VIEW_STATE_FILE, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save window state: " + ex.Message);
            }
        }

        private void RestoreState()
        {
            var state = ReadStateFromFile(_MAIN_VIEW_STATE_FILE);

            this.Left = state.Left;
            this.Top = state.Top;
            this.Width = state.Width;
            this.Height = state.Height;
        }

        private static ViewState ReadStateFromFile(string filepath)
        {
            try
            {
                var json = File.ReadAllText(filepath);
                return JsonSerializer.Deserialize<ViewState>(json) ?? throw new NullReferenceException("Deserialized ViewState was null.");
            }
            catch (Exception)
            {
                return new ViewState()
                {
                    Left = (WpfScreenHelper.Screen.PrimaryScreen.Bounds.Right / 2) - 200,
                    Top = (WpfScreenHelper.Screen.PrimaryScreen.Bounds.Bottom / 2) - 200,
                    Width = 600,
                    Height = 500
                };
            }
        }

        #endregion

        #region RESIZE

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private enum ResizeDirection { Left = 61441, Right = 61442, Top = 61443, Bottom = 61446, BottomRight = 61448, }

        private void CornerResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var hwndSource = (HwndSource)PresentationSource.FromVisual((Visual)sender);
            SendMessage(hwndSource.Handle, 0x112, (IntPtr)ResizeDirection.BottomRight, IntPtr.Zero);
        }

        private void BG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                e.Handled = true;
            }
        }

        #endregion

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                IsCtrlHeld = true;
            else if (e.Key == Key.E && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //TODO: Refactor
                var pos = Mouse.GetPosition(this);
                var hitTest = VisualTreeHelper.HitTest(this, pos);

                DependencyObject? element = ((FrameworkElement)hitTest.VisualHit).Parent;

                do
                {
                    if (element is AssetView)
                        break;
                    else
                        element = VisualTreeHelper.GetParent(element);
                }
                while (element is not MainWindow);

                if (element is AssetView assetView)
                {
                    var menuItem = assetView.OpenInExplorerMenuItem;
                    menuItem.Command.Execute(assetView.DataContext);
                }
            }
        }

        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                IsCtrlHeld = false;
        }

        private void AddAssets_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddAssetsButtons.Visibility = AddAssetsButtons.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var clickedRegion = WpfElementUtils.GetParentOfTypeByName<StackPanel>((FrameworkElement)e.OriginalSource, nameof(AddAssets));

            //if (clickedRegion != AddAssets)
            //    AddAssetsButtons.Visibility = Visibility.Collapsed;
        }

        private void ItemsContainer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Properly raise mouse scroll event without interfering with other controls.
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

        private void EditableTextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            EditableTextBlock editableTB = (EditableTextBlock)sender;
            editableTB.IsEditing = true;
        }

        private void AddButtons_FadeOut_Completed(object sender, EventArgs e)
        {
            // Update button state when AddButtons was hidden by other actions.
            // Without this AddButton needs to be clicked twice to show buttons.
            AddButton.IsChecked = false;
        }

        private void AddButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
