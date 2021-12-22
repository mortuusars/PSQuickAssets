using PSQuickAssets.Windows.State;
using PSQuickAssets.WPF;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace PSQuickAssets.Windows
{
    public partial class MainWindow : Window
    {
        private const string _MAIN_VIEW_STATE_FILE = "state.json";

        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register("IsShown", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            IsVisibleChanged += OnVisibilityChanged;
        }

        public new void Show()
        {
            base.Show();
            this.Visibility = Visibility.Collapsed;
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                SaveState();
        }

        #region STATE

        public void SaveState()
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
                File.WriteAllText(_MAIN_VIEW_STATE_FILE, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save window state: " + ex.Message);
            }
        }

        public void RestoreState()
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
            var hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
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

        #region ANIMATIONS

        public void FadeIn()
        {
            if (!IsShown)
            {
                IsShown = true;
                this.Visibility = Visibility.Visible;
            }
        }

        public void FadeOut()
        {
            if (IsShown)
                IsShown = false;
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            if (!IsShown)
                this.Visibility = Visibility.Collapsed;
        }

        #endregion

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                CloseButton.ActivateAlternativeStyle = true;
        }

        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                CloseButton.ActivateAlternativeStyle = false;
        }

        private void AddAssets_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddAssetsButtons.Visibility = AddAssetsButtons.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedRegion = WpfElementUtils.GetParentOfTypeByName<StackPanel>((FrameworkElement)e.OriginalSource, nameof(AddAssets));

            if (clickedRegion != AddAssets)
                AddAssetsButtons.Visibility = Visibility.Collapsed;
        }

        private void ItemsContainer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (sender is ItemsControl && !e.Handled && Keyboard.Modifiers != ModifierKeys.None)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }

            //App.Logger.Info(sender.GetType().ToString());
            //App.Logger.Info(((UIElement)((Control)sender).Parent).ToString());

            //string wheel = e.Delta > 0 ? "Up" : "Down";
            //string msg = $"{Keyboard.Modifiers} + {wheel}";
            //App.Logger.Info(msg);
        }
    }
}
