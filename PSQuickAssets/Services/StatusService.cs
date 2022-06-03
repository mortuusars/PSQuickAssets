using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PSQuickAssets.Services;

internal interface IStatusService : INotifyPropertyChanged
{
    /// <summary>
    /// Indicates that there is currently one or more tasks in progress.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Collection of currently active loading tasks.
    /// </summary>
    ObservableCollection<ITask> LoadingTasks { get; }

    /// <summary>
    /// Represetns a task that is considered in progress when created, and finished when disposed.
    /// </summary>
    IDisposable Loading(string taskName);
}

internal interface ITask
{
    event EventHandler? Finished;
    string Name { get; }
    DateTime StartTime { get; }
}

internal partial class StatusService : ObservableObject, IStatusService
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<ITask> _loadingTasks = new();

    public StatusService()
    {
        _loadingTasks.CollectionChanged += (s, e) => IsLoading = _loadingTasks.Any();
    }

    public IDisposable Loading(string taskName)
    {
        var loadingTask = new LoadingTask(taskName);
        loadingTask.Finished += LoadingTask_Finished;
        _loadingTasks.Add(loadingTask);
        return loadingTask;
    }

    private void LoadingTask_Finished(object? sender, EventArgs e)
    {
        if (sender is ITask task)
        {
            task.Finished -= LoadingTask_Finished;
            _loadingTasks.Remove(task);
        }
    }

    private class LoadingTask : ITask, IDisposable
    {
        public event EventHandler? Finished;
        public string Name { get; init; }
        public DateTime StartTime { get; init; }

        public LoadingTask(string name)
        {
            Name = name;
            StartTime = DateTime.Now;
        }

        public void Dispose()
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
}