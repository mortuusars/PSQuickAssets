using PSQuickAssets.Views.State;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PSQuickAssets.WPF.Converters;

namespace PSQuickAssets.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private const string _MAIN_VIEW_STATE_FILE = "state.json";

        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register("IsShown", typeof(bool), typeof(MainView), new PropertyMetadata(false));

        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        public MainView()
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
                return JsonSerializer.Deserialize<ViewState>(json);
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

        private void CornerResize_MouseEnter(object sender, MouseEventArgs e) => Mouse.OverrideCursor = Cursors.SizeNWSE;

        private void CornerResize_MouseLeave(object sender, MouseEventArgs e) => Mouse.OverrideCursor = Cursors.Arrow;

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
                CloseButton.ActivateAlternativeStyle = true;
        }

        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            CloseButton.ActivateAlternativeStyle = false;
        }

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
    }
}
