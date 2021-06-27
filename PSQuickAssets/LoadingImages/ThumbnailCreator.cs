using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PSQuickAssets
{
    public static class ThumbnailCreator
    {
        /// <summary>
        /// Loads image from file and creates small thumbnail from it.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="width"></param>
        /// <returns>BitmapSourse to be used in WPF.</returns>
        public static BitmapSource CreateThumbnail(string filepath, int width = 60)
        {
            Image image = Image.FromFile(filepath);
            int newHeight = Convert.ToInt32(image.Size.Height / (image.Size.Width / width));
            Image thumbnail = image.GetThumbnailImage(width, newHeight, null, IntPtr.Zero);
            return ImageToBitmapSource(thumbnail);
        }

        public static BitmapSource ImageToBitmapSource(Image image)
        {
            var bitmap = new Bitmap(image);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bmpPt,IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);
    }

}
