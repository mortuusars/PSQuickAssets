using Hardcodet.Wpf.TaskbarNotification;

namespace PSQuickAssets.Services
{
    /// <summary>
    /// Provides ability to show toast notifications. Uses Hardcodet.NotifyIcon.
    /// </summary>
    internal class TaskbarNotificationService : INotificationService
    {
        private Action? _onNotificationClickedAction = () => { };

        private TaskbarIcon? TaskbarIcon { get => _taskbarIcon ??= TryGetTaskbarIcon(); }
        private TaskbarIcon? _taskbarIcon;
        private readonly WindowManager _windowManager;

        public TaskbarNotificationService(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void Notify(string title, string message, NotificationIcon icon, Action? onNotificationClicked = null)
        {
            var balloonIcon = icon switch
            {
                NotificationIcon.None => BalloonIcon.None,
                NotificationIcon.Info => BalloonIcon.Info,
                NotificationIcon.Warning => BalloonIcon.Warning,
                NotificationIcon.Error => BalloonIcon.Error,
                _ => BalloonIcon.Info,
            };

            _onNotificationClickedAction = onNotificationClicked;
            TaskbarIcon?.ShowBalloonTip(title, message, balloonIcon);
        }

        public void Notify(string message, NotificationIcon icon, Action? onNotificationClicked = null)
        {
            Notify(App.AppName, message, icon, onNotificationClicked);
        }

        private void NotificationClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_onNotificationClickedAction is not null)
                _onNotificationClickedAction();
        }

        private TaskbarIcon? TryGetTaskbarIcon()
        {
            TaskbarIcon? taskbarIcon = _windowManager.TaskbarIcon;
            if (taskbarIcon is not null)
                taskbarIcon.TrayBalloonTipClicked += NotificationClicked;
            return taskbarIcon;
        }
    }
}
