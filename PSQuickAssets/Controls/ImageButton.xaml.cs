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

namespace PSQuickAssets.Controls
{
    /// <summary>
    /// Image control with mouseover opacity. ActivateAlternativeStyle can be used to switch between sources.
    /// </summary>
    public partial class ImageButton : UserControl
    {
        public static readonly DependencyProperty ActivateAlternativeStyleProperty =
            DependencyProperty.Register(nameof(ActivateAlternativeStyle), typeof(bool), typeof(ImageButton), new PropertyMetadata(false));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

        public static readonly DependencyProperty AlternativeSourceProperty =
            DependencyProperty.Register(nameof(AlternativeSource), typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool ActivateAlternativeStyle
        {
            get { return (bool)GetValue(ActivateAlternativeStyleProperty); }
            set { SetValue(ActivateAlternativeStyleProperty, value); }
        }

        public ImageSource AlternativeSource
        {
            get { return (ImageSource)GetValue(AlternativeSourceProperty); }
            set { SetValue(AlternativeSourceProperty, value); }
        }

        public ImageButton()
        {
            InitializeComponent();
        }
    }
}
