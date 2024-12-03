using BankManagement.Database.NotificationData.Entities;

namespace BankManagement.Database.NotificationData.Interfaces;

public interface INotificationRepository
{
    /// <summary>
    /// Adds a new notification using the provided notification details.
    /// It returns a boolean indicating whether the notification was successfully added.
    /// </summary>
    Task<bool> AddNotificationAsync(Notification notification);

    /// <summary>
    /// Retrieves a collection of notifications associated with a specified username.
    /// It returns a collection of Notification representing the notifications for the given user.
    /// </summary>
    Task<ICollection<Notification>> GetNotificationsByUserNameAsync(string username);

    /// <summary>
    /// Retrieves notifications that are older than 24 hours.
    /// It returns a collection of Notification representing the old notifications.
    /// </summary>
    Task<ICollection<Notification>> GetNotificationsOlderThan24HoursAsync();

    /// <summary>
    /// Deletes a notification by its ID.
    /// It returns a boolean indicating whether the deletion was successful.
    /// </summary>
    Task<bool> DeleteNotificationAsync(int notificationId);
}
