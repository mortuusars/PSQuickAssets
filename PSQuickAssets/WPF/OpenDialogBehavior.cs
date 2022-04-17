using PSQuickAssets.Utils.SystemDialogs;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PSQuickAssets.WPF;

public enum DialogType
{
    None,
    SelectFile,
    SelectFiles,
    SelectFolder,
    SelectFolders
}

public class OpenDialogBehavior : DependencyObject
{
    public static readonly DependencyProperty DialogProperty =
    DependencyProperty.RegisterAttached("Dialog", typeof(DialogType),
        typeof(OpenDialogBehavior), new PropertyMetadata(DialogType.None, OnDialogChanged));
    public static DialogType GetDialog(DependencyObject obj) => (DialogType)obj.GetValue(DialogProperty);
    public static void SetDialog(DependencyObject obj, DialogType value) => obj.SetValue(DialogProperty, value);

    public static readonly DependencyProperty CommandProperty =
    DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(OpenDialogBehavior), new PropertyMetadata(null));
    public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);
    public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

    private static void OnDialogChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UIElement element = (UIElement)d;
        element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
        element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
    }

    private static void Element_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        UIElement element = (UIElement)sender;

        DialogType dialog = GetDialog(element);

        if (dialog == DialogType.None)
            return;

        ICommand? command = null;

        if (GetCommand(element) is ICommand cmd)
            command = cmd;
        else if (element is ButtonBase btn)
            command = btn.Command;

        if (command is null)
            return;

        string[] dialogResult = ShowDialog(dialog);
        command.Execute(dialogResult);
    }

    private static string[] ShowDialog(DialogType dialog)
    {
        return dialog switch
        {
            DialogType.None => Array.Empty<string>(),
            DialogType.SelectFile => SystemDialogs.SelectFiles(
                Resources.Localization.Instance["SelectAssets"], FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Single),
            DialogType.SelectFiles => SystemDialogs.SelectFiles(
                Resources.Localization.Instance["SelectAssets"], FileFilters.Images + "|" + FileFilters.AllFiles, SelectionMode.Multiple),
            DialogType.SelectFolder => SystemDialogs.SelectFolder(
                Resources.Localization.Instance["SelectFolder"], SelectionMode.Single),
            DialogType.SelectFolders => SystemDialogs.SelectFolder(
                Resources.Localization.Instance["SelectFolder"], SelectionMode.Multiple),
            _ => throw new NotSupportedException()
        };
    }
}
