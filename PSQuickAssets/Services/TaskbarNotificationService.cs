using Hardcodet.Wpf.TaskbarNotification;
using System;

namespace PSQuickAssets.Services
{
    /// <summary>
    /// Provides ability to show toast notifications. Uses Hardcodet.NotifyIcon.
    /// </summary>
    internal class TaskbarNotificationService : INotificationService
    {
        private Action _onNotificationClicked = () => { };

        private readonly TaskbarIcon _taskBarIcon;

        public TaskbarNotificationService(TaskbarIcon taskBarIcon)
        {
            _taskBarIcon = taskBarIcon;
            _taskBarIcon.TrayBalloonTipClicked += NotificationClicked;
        }

        public void Notify(string title, string message, NotificationIcon icon)
        {
            BalloonIcon balloonIcon = icon switch
            {
                NotificationIcon.None => BalloonIcon.None,
                NotificationIcon.Info => BalloonIcon.Info,
                NotificationIcon.Warning => BalloonIcon.Warning,
                NotificationIcon.Error => BalloonIcon.Error,
                _ => BalloonIcon.Info,
            };

            _onNotificationClicked = () => { };
            _taskBarIcon.ShowBalloonTip(title, message, balloonIcon);
        }

        public void Notify(string title, string message, NotificationIcon icon, Action onNotificationClicked)
        {
            Notify(title, message, icon);
            _onNotificationClicked = onNotificationClicked;
        }

        private void NotificationClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            _onNotificationClicked();
        }
    }
}
