using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSQA.Core;

namespace PSQuickAssets.ViewModels;

internal partial class PostActionsViewModel : ObservableObject
{
    public ObservableCollection<PhotoshopAction> Actions { get; } = new();

    [ObservableProperty]
    private bool _addingMode;

    [ObservableProperty]
    [AlsoNotifyCanExecuteFor(nameof(AddActionCommand))]
    private string _action = string.Empty;

    [ObservableProperty]
    [AlsoNotifyCanExecuteFor(nameof(AddActionCommand))]
    private string _set = string.Empty;

    public bool CanAddAction { get => !string.IsNullOrWhiteSpace(Action) && !string.IsNullOrWhiteSpace(Set); }

    private readonly IConfig _config;

    public PostActionsViewModel(IConfig config)
    {
        _config = config;
        Actions = new ObservableCollection<PhotoshopAction>(_config.ActionsAfterAdding);
        Actions.CollectionChanged += UpdateConfig;
    }

    private void UpdateConfig(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        _config.ActionsAfterAdding = Actions.ToArray();
        // Config saves automatically after a change.
    }

    [ICommand]
    private void EnterAddingMode()
    {
        ClearInputFields();
        AddingMode = true;
    }

    [ICommand]
    private void ExitAddingMode() => AddingMode = false;

    [ICommand(CanExecute = nameof(CanAddAction))]
    private void AddAction()
    {
        if (!CanAddAction) return;

        Actions.Add(new PhotoshopAction(Action, Set));
        ExitAddingMode();
    }

    [ICommand]
    private void RemoveAction(PhotoshopAction? action)
    {
        Actions.Remove(action!);
    }

    private void ClearInputFields()
    {
        Action = string.Empty;
        Set = string.Empty;
    }
}