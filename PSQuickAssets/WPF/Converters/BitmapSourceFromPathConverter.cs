﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.WPF.Converters;

[ValueConversion(typeof(string), typeof(BitmapSource))]
internal class BitmapSourceFromPathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string path)
        {
            try
            {
                return CreateThumbnail(path, 999, 300);
            }
            catch (Exception)
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/image_light.png"));
            }
        }

        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public BitmapImage CreateThumbnail(string filePath, int maxWidth, int maxHeight)
    {
        byte[] imgBytes = GetImageBytesInMemory(filePath, maxWidth, maxHeight);
        BitmapImage bitmapImage = BitmapImageFromBytes(imgBytes);

        //if (bitmapImage.DpiX != 96)
        //    bitmapImage = ConvertBitmapTo96DPI(bitmapImage);

        return bitmapImage;
    }

    private byte[] GetImageBytesInMemory(string filePath, int maxWidth, int maxHeight)
    {
        var image = NetVips.Image.Thumbnail(filePath, maxWidth, maxHeight);
        byte[] imgBytes = image.WriteToBuffer(".png");
        image.Dispose();
        return imgBytes;
    }

    private BitmapImage BitmapImageFromBytes(byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            BitmapImage bmp = new();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
    }

    private static BitmapImage ConvertBitmapTo96DPI(BitmapImage bitmapImage)
    {
        double dpi = 96;
        int width = bitmapImage.PixelWidth;
        int height = bitmapImage.PixelHeight;

        int stride = width * bitmapImage.Format.BitsPerPixel;
        byte[] pixelData = new byte[stride * height];
        bitmapImage.CopyPixels(pixelData, stride, 0);

        BitmapSource bitmapSource = BitmapSource.Create(width, height, dpi, dpi, bitmapImage.Format, null, pixelData, stride);

        PngBitmapEncoder encoder = new();
        MemoryStream memoryStream = new();
        BitmapImage image = new();

        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        encoder.Save(memoryStream);

        using (var ms = new MemoryStream())
        {
            image.BeginInit();
            image.StreamSource = memoryStream;
            image.EndInit();
            image.Freeze();
        }

        return image;
    }
}