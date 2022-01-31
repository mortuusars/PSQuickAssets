using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace WindowTemplates;

[TemplatePart(Name = _partBackgroundName, Type = typeof(Border))]
[TemplatePart(Name = _partHeaderName, Type = typeof(FrameworkElement))]
public class WindowBase : Window
{
    public Brush HeaderBackground
    {
        get { return (Brush)GetValue(HeaderBackgroundProperty); }
        set { SetValue(HeaderBackgroundProperty, value); }
    }

    public static readonly DependencyProperty HeaderBackgroundProperty =
        DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(WindowBase), new PropertyMetadata(null));


    public object HeaderContent
    {
        get { return (object)GetValue(HeaderContentProperty); }
        set { SetValue(HeaderContentProperty, value); }
    }

    public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(WindowBase), new PropertyMetadata(null));


    private const string _partBackgroundName = "PART_Background";
    private const string _partHeaderName = "PART_Header";

    private Border? _backgroundBorder;
    private FrameworkElement? _headerElement;

    static WindowBase()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase), new FrameworkPropertyMetadata(typeof(WindowBase)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _backgroundBorder = GetTemplateChild(_partBackgroundName) as Border;

        if (_headerElement is not null)
            _headerElement.SizeChanged -= _headerPanel_SizeChanged;

        _headerElement = GetTemplateChild(_partHeaderName) as FrameworkElement;
        if (_headerElement is not null)
            _headerElement.SizeChanged += _headerPanel_SizeChanged;
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        var windowChrome = WindowChrome.GetWindowChrome(this);
        windowChrome.CaptionHeight += windowChrome.ResizeBorderThickness.Top;

        if (_backgroundBorder is not null)
        {
            _backgroundBorder.Margin = WindowState == WindowState.Maximized ? 
                Subtract(windowChrome.ResizeBorderThickness, Add(_backgroundBorder.BorderThickness, 2))
                : new Thickness(0);
        }
    }

    private void _headerPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_headerElement is not null && e.HeightChanged)
        {
            var windowChrome = WindowChrome.GetWindowChrome(this);
            windowChrome.CaptionHeight = e.NewSize.Height - windowChrome.ResizeBorderThickness.Top;
            WindowChrome.SetWindowChrome(this, windowChrome);
        }
    }

    private Thickness Subtract(Thickness source, Thickness sub)
    {
        return new Thickness(source.Left - sub.Left, source.Top - sub.Top, source.Right - sub.Right, source.Bottom - sub.Bottom);
    }

    private Thickness Multiply(Thickness source, double mul)
    {
        return new Thickness(source.Left * mul, source.Top * mul, source.Right * mul, source.Bottom * mul);
    }

    private Thickness Add(Thickness source, double add)
    {
        return new Thickness(source.Left + add, source.Top + add, source.Right + add, source.Bottom + add);
    }
}
