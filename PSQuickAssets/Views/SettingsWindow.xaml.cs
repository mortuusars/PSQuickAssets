﻿using PSQuickAssets.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSQuickAssets.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //DwmDropShadow.DropShadowToWindow(this);
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        private void BG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.DragMove();
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(WpfElementUtils.GetParentOfTypeByName<Grid>(e.OriginalSource as FrameworkElement, nameof(HotkeyColumn)) != HotkeyColumn)
                FocusManager.SetFocusedElement(this, this);
        }
    }

    //public static class DwmDropShadow
    //{
    //    [DllImport("dwmapi.dll", PreserveSig = true)]
    //    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    //    [DllImport("dwmapi.dll")]
    //    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

    //    /// <summary>
    //    /// Drops a standard shadow to a WPF Window, even if the window is borderless. Only works with DWM (Windows Vista or newer).
    //    /// This method is much more efficient than setting AllowsTransparency to true and using the DropShadow effect,
    //    /// as AllowsTransparency involves a huge performance issue (hardware acceleration is turned off for all the window).
    //    /// </summary>
    //    /// <param name="window">Window to which the shadow will be applied</param>
    //    public static void DropShadowToWindow(Window window)
    //    {
    //        if (!DropShadow(window))
    //        {
    //            window.SourceInitialized += new EventHandler(window_SourceInitialized);
    //        }
    //    }

    //    private static void window_SourceInitialized(object sender, EventArgs e)
    //    {
    //        Window window = (Window)sender;

    //        DropShadow(window);

    //        window.SourceInitialized -= new EventHandler(window_SourceInitialized);
    //    }

    //    /// <summary>
    //    /// The actual method that makes API calls to drop the shadow to the window
    //    /// </summary>
    //    /// <param name="window">Window to which the shadow will be applied</param>
    //    /// <returns>True if the method succeeded, false if not</returns>
    //    private static bool DropShadow(Window window)
    //    {
    //        try
    //        {
    //            WindowInteropHelper helper = new WindowInteropHelper(window);
    //            int val = 2;
    //            int ret1 = DwmSetWindowAttribute(helper.Handle, 2, ref val, 4);

    //            if (ret1 == 0)
    //            {
    //                Margins m = new Margins { Bottom = 0, Left = 50, Right = 0, Top = 0 };
    //                int ret2 = DwmExtendFrameIntoClientArea(helper.Handle, ref m);
    //                return ret2 == 0;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // Probably dwmapi.dll not found (incompatible OS)
    //            return false;
    //        }
    //    }

    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct Margins
    //{
    //    public int Left;
    //    public int Right;
    //    public int Top;
    //    public int Bottom;
    //}
}
