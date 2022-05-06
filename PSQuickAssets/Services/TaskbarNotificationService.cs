using Hardcodet.Wpf.TaskbarNotification;
using System;

namespace PSQuickAssets.Services
{
    /// <summary>
    /// Provides ability to show toast notifications. Uses Hardcodet.NotifyIcon.
    /// </summary>
    internal class TaskbarNotificationService : INotificationService
    {
        private Action? _onNotificationClickedAction = () => { };

        private TaskbarIcon? _taskbarIcon;

        public void Notify(string title, string message, NotificationIcon icon, Action? onNotificationClicked = null)
        {
            if (_taskbarIcon is null)
                TryGetTaskbarIcon();

            var balloonIcon = icon switch
            {
                NotificationIcon.None => BalloonIcon.None,
                NotificationIcon.Info => BalloonIcon.Info,
                NotificationIcon.Warning => BalloonIcon.Warning,
                NotificationIcon.Error => BalloonIcon.Error,
                _ => BalloonIcon.Info,
            };

            _onNotificationClickedAction = onNotificationClicked;
            _taskbarIcon?.ShowBalloonTip(title, message, balloonIcon);
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
            //TODO: Move this to proper notification service.
            return null;
            try
            {
                var taskbarIcon = (TaskbarIcon)App.Current.FindResource("TaskBarIcon");
                taskbarIcon.TrayBalloonTipClicked += NotificationClicked;
                _taskbarIcon = taskbarIcon;
                return taskbarIcon;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
