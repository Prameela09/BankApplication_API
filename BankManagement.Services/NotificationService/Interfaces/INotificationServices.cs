using BankManagement.Database.NotificationData.Entities;
using BankManagement.Services.NotificationService.DTOs;
using BankManagement.Utilities.HelperClasses;

namespace BankManagement.Services.NotificationServices.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Retrieves a collection of notifications associated with a specified user.
    /// </summary>
    Task<ICollection<NotificationDTO>> GetNotificationsByUserNameAsync(string username, PaginationInput pageDetails);

    /// <summary>
    /// Sends a notification to a specified user, including the notification's title, message, and status.
    /// </summary>
    Task<bool> SendNotificationAsync(int userId, string title, string message, int statusId);

    /// <summary>
    /// Deletes notifications that are older than 24 hours.
    /// </summary>
    Task<bool> DeleteOldNotificationsAsync();

    /// <summary>
    /// Retrieves notifications that are older than 24 hours.
    /// </summary>
    Task<ICollection<Notification>> GetOldNotificationsAsync();
}