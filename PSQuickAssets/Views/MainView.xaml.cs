using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using PSQuickAssets.Views.State;

namespace PSQuickAssets.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        #region STATE

        public void SaveState(string filepath)
        {
            var state = new ViewState()
            {
                Left = this.Left,
                Top = this.Top,
                Width = this.ActualWidth,
                Height = this.ActualHeight,
            };

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions() { WriteIndented = true });

            try
            {
                File.WriteAllText(filepath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save window state: " + ex.Message);
            }
        }

        public void RestoreState(string filepath)
        {
            var state = ReadStateFromFile(filepath);

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
    }
}
