using System;

namespace PSQuickAssets.Services
{
    internal interface INotificationService
    {
        /// <summary>
        /// Displays notification to the user and allows executing action when user click on displayed notification.
        /// </summary>
        /// <param name="title">Title of the notification.</param>
        /// <param name="message">Message.</param>
        /// <param name="icon">Icon that would be displayed on notification.</param>
        /// <param name="onNotificationClicked">Executed when user clicks on notification.</param>
        void Notify(string title, string message, NotificationIcon icon, Action? onNotificationClicked = null);
        /// <summary>
        /// Displays notification to the user and allows executing action when user click on displayed notification.<br>Uses AppName title.</br>
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="icon">Icon that would be displayed on notification.</param>
        /// <param name="onNotificationClicked">Executed when user clicks on notification.</param>
        void Notify(string message, NotificationIcon icon, Action? onNotificationClicked = null);
    }

    internal enum NotificationIcon
    {
        None, Info, Warning, Error
    }
}
