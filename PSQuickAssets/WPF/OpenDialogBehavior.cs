using PSQuickAssets.Utils.SystemDialogs;
using System.Reflection;
using System.Windows;
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
        element.PreviewMouseLeftButtonDown -= Element_MouseLeftButtonDown;
        element.PreviewMouseLeftButtonDown += Element_MouseLeftButtonDown;
    }

    private static void Element_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        UIElement element = (UIElement)sender;

        DialogType dialog = GetDialog(element);

        if (dialog == DialogType.None)
            return;

        ICommand? command = GetCommand(element) ?? GetNativeCommand(element);

        if (command is null)
            return;

        e.Handled = true;

        string[] dialogResult = ShowDialog(dialog);
        command.Execute(dialogResult);
    }

    private static ICommand? GetNativeCommand(object element)
    {
        var type = element.GetType();
        PropertyInfo? cmdProp = type.GetProperty("Command");
        
        if (cmdProp?.GetValue(element) is ICommand command)
            return command;

        return null;
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
