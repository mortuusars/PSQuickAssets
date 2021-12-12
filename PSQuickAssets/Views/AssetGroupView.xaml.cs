using PSQuickAssets.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PSQuickAssets.Views
{
    public partial class AssetGroupView : UserControl
    {
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(AssetGroupView), new PropertyMetadata(true, OnExpandedChanged));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public AssetGroupView()
        {
            InitializeComponent();
        }

        private void Fold_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        private void AnimateExpander()
        {
            int toAngle = IsExpanded ? 360 : 270;
            DoubleAnimation rotateAnim = new DoubleAnimation(toAngle, new Duration(TimeSpan.FromMilliseconds(140)));
            var storyboard = new Storyboard();
            storyboard.Children.Add(rotateAnim);
            Storyboard.SetTarget(rotateAnim, expandButton);
            Storyboard.SetTargetProperty(rotateAnim, new PropertyPath("RenderTransform.Angle"));
            expandButton.BeginStoryboard(storyboard);
        }

        private static void OnExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var assetGroupView = (AssetGroupView)d;
            assetGroupView.AnimateExpander();
        }
    }
}
