using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace PSQuickAssets.AssetControl;

public class AssetControl : Control
{
    static AssetControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AssetControl), new FrameworkPropertyMetadata(typeof(AssetControl)));
    }

    public BitmapSource Thumbnail
    {
        get { return (BitmapSource)GetValue(ThumbnailProperty); }
        set { SetValue(ThumbnailProperty, value); }
    }

    public static readonly DependencyProperty ThumbnailProperty =
        DependencyProperty.Register(nameof(Thumbnail), typeof(BitmapSource), typeof(AssetControl), new PropertyMetadata(null, RecalculateThumbnailWidth));

    public double ThumbnailSize
    {
        get { return (double)GetValue(ThumbnailSizeProperty); }
        set { SetValue(ThumbnailSizeProperty, value); }
    }

    public static readonly DependencyProperty ThumbnailSizeProperty =
        DependencyProperty.Register(nameof(ThumbnailSize), typeof(double), typeof(AssetControl), new PropertyMetadata(60.0, RecalculateThumbnailWidth));

    public double MaximumWidthToHeightRatio
    {
        get { return (double)GetValue(MaximumWidthToHeightRatioProperty); }
        set { SetValue(MaximumWidthToHeightRatioProperty, value); }
    }

    public static readonly DependencyProperty MaximumWidthToHeightRatioProperty =
        DependencyProperty.Register(nameof(MaximumWidthToHeightRatio), typeof(double), typeof(AssetControl), new PropertyMetadata(2.5, RecalculateThumbnailWidth));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(AssetControl), new PropertyMetadata(string.Empty));

    internal double ImageWidth
    {
        get { return (double)GetValue(ImageWidthProperty); }
        set { SetValue(ImageWidthProperty, value); }
    }

    internal static readonly DependencyProperty ImageWidthProperty =
        DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(AssetControl), new PropertyMetadata(50.0));

    public Effect AssetEffect
    {
        get { return (Effect)GetValue(AssetEffectProperty); }
        set { SetValue(AssetEffectProperty, value); }
    }

    public static readonly DependencyProperty AssetEffectProperty =
        DependencyProperty.Register(nameof(AssetEffect), typeof(Effect), typeof(AssetControl), new PropertyMetadata(null));


    public Stretch ThumbnailStretch
    {
        get { return (Stretch)GetValue(ThumbnailStretchProperty); }
        set { SetValue(ThumbnailStretchProperty, value); }
    }

    public static readonly DependencyProperty ThumbnailStretchProperty =
        DependencyProperty.Register(nameof(ThumbnailStretch), typeof(Stretch), typeof(AssetControl), new FrameworkPropertyMetadata(Stretch.UniformToFill, FrameworkPropertyMetadataOptions.AffectsRender));


    private static void RecalculateThumbnailWidth(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        AssetControl assetControl = (AssetControl)d;

        if (assetControl.Thumbnail is null)
            return;

        double proportion = assetControl.Thumbnail.Width / assetControl.Thumbnail.Height;

        if (proportion > 4 || proportion < 0.6)
            assetControl.ThumbnailStretch = Stretch.Uniform;

        proportion = Math.Min(assetControl.MaximumWidthToHeightRatio, proportion);

        var newWidth = assetControl.ThumbnailSize * proportion;

        if (assetControl.ImageWidth != newWidth)
            assetControl.ImageWidth = newWidth;
    }
}
