using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PSQuickAssets.Services;

internal interface IStatusService : INotifyPropertyChanged
{
    /// <summary>
    /// Indicates that there is currently one or more tasks in progress.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Represetns a task that is considered in progress when created, and finished when disposed.
    /// </summary>
    IDisposable LoadingStatus();
}

internal class StatusService : ObservableObject, IStatusService
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }

    private readonly ObservableCollection<object> _loadingTasks = new();

    public StatusService()
    {
        _loadingTasks.CollectionChanged += (s, e) =>
        {
            IsLoading = _loadingTasks.Any();
        };
    }

    public IDisposable LoadingStatus()
    {
        var status = new LoadingTask(this);
        _loadingTasks.Add(status);
        return status;
    }

    private void LoadingTaskFinished(LoadingTask task)
    {
        _loadingTasks.Remove(task);
    }

    private class LoadingTask : IDisposable
    {
        private readonly StatusService _statusService;
        public LoadingTask(StatusService statusService) => _statusService = statusService;

        public void Dispose()
        {
            _statusService.LoadingTaskFinished(this);
        }
    }
}