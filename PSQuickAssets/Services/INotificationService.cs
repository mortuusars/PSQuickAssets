using System;

namespace PSQuickAssets.Services
{
    internal interface INotificationService
    {
        /// <summary>
        /// Displays notification to the user.
        /// </summary>
        /// <param name="title">Title of the notification.</param>
        /// <param name="message">Message.</param>
        /// <param name="icon">Icon that would be displayed on notification.</param>
        /// <param name="onNotificationClicked">Executed when user clicks on notification.</param>
        void Notify(string title, string message, NotificationIcon icon, Action onNotificationClicked);
        /// <summary>
        /// Displays notification to the user.
        /// </summary>
        /// <param name="title">Title of the notification.</param>
        /// <param name="message">Message.</param>
        /// <param name="icon">Icon that would be displayed on notification.</param>
        void Notify(string title, string message, NotificationIcon icon);
    }

    internal enum NotificationIcon
    {
        None, Info, Warning, Error
    }
}
