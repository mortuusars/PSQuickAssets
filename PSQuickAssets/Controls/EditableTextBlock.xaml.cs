using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PSQuickAssets.Controls;

public partial class EditableTextBlock : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableTextBlock), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false, OnIsEditingChanged));

    public static readonly DependencyProperty RenameValidationProperty =
        DependencyProperty.Register(nameof(RenameValidation), typeof(Func<string, bool>), typeof(EditableTextBlock),
            new PropertyMetadata(new Func<string, bool>((s) => true)));

    public static readonly DependencyProperty HasErrorProperty =
        DependencyProperty.Register("HasError", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));

    /// <summary>
    /// Text that will be displayed.
    /// </summary>
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    /// <summary>
    /// Indicates whether a control is currently in editing mode and can receive input.
    /// </summary>
    public bool IsEditing
    {
        get { return (bool)GetValue(IsEditingProperty); }
        set { SetValue(IsEditingProperty, value); }
    }

    /// <summary>
    /// Used to validate new name on every input box change.
    /// </summary>
    public Func<string, bool> RenameValidation
    {
        get { return (Func<string, bool>)GetValue(RenameValidationProperty); }
        set { SetValue(RenameValidationProperty, value); }
    }

    /// <summary>
    /// Indicates whether input box is in error state due to invalid name.
    /// </summary>
    public bool HasError
    {
        get { return (bool)GetValue(HasErrorProperty); }
        set { SetValue(HasErrorProperty, value); }
    }

    public EditableTextBlock()
    {
        InitializeComponent();
    }

    private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        EditableTextBlock editableTB = (EditableTextBlock)d;

        if (e.NewValue is true)
        {
            editableTB.EditBox.Text = editableTB.Text;
            editableTB.EditBox.CaretIndex = editableTB.EditBox.Text.Length;
            editableTB.EditBox.SelectAll();
            // Other methods to set focus don't work for some reason. This works:
            editableTB.Dispatcher.BeginInvoke(() => { editableTB.EditBox.Focus(); Keyboard.Focus(editableTB.EditBox); }, DispatcherPriority.Background);
        }
        else
        {
            editableTB.HasError = false;
            editableTB.ReleaseMouseCapture();
        }
    }

    private void EditBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            CancelRename();
        }
        else if (e.Key == Key.Enter)
        {
            if (EditBox.Text.Equals(Text))
                CancelRename();
            else if (RenameValidation(EditBox.Text)) 
                ConfirmRename();

            e.Handled = true;
        }
    }

    private void CancelRename()
    {
        IsEditing = false;
        EditBox.Text = Text;
    }

    private void ConfirmRename()
    {
        IsEditing = false;
        Text = EditBox.Text;
    }

    private void EditBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            Dispatcher.BeginInvoke(() => Mouse.Capture(this, CaptureMode.SubTree), DispatcherPriority.Normal);
            Dispatcher.BeginInvoke(() => { Debug.WriteLine(EditBox.Focus()); /*Keyboard.Focus(EditBox);*/ }, DispatcherPriority.Background);
            Debug.WriteLine(Keyboard.FocusedElement);
        }
    }

    private void root_LostMouseCapture(object sender, MouseEventArgs e)
    {
        if (IsEditing)
            EditBox.CaptureMouse();
    }

    private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsEditing is false)
            return;

        if (IsMouseOutside())
        {
            e.Handled = true;
            ConfirmRename();
        }
        else
            EditBox.CaptureMouse();
    }

    private void EditBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Validate new name on every text change.
        HasError = !RenameValidation(EditBox.Text) && !EditBox.Text.Equals(Text);
    }

    private void EditBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsMouseOutside())
            ConfirmRename();
        else
            Mouse.Capture(EditBox, CaptureMode.SubTree);
    }

    /// <summary>Determines if mouse is currently outside of this control (EditableTextBlock). Used to Exit editing state if clicked outside.</summary>
    private bool IsMouseOutside()
    {
        var pos = Mouse.GetPosition(this);
        var hitTestResult = VisualTreeHelper.HitTest(this, pos);
        return hitTestResult is null;
    }
}