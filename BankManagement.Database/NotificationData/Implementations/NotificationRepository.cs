using BankManagement.Database.DbContexts;
using BankManagement.Database.NotificationData.Entities;
using BankManagement.Database.NotificationData.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.NotificationData.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly BankDataContext _context;

        public NotificationRepository(BankDataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<Notification>> GetNotificationsByUserNameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }
            ICollection<Notification> notifications = await _context.Notifications
                // .Include(n => n.User) 
                .Where(n => n.User.UserName == username)
                .ToListAsync();

            return notifications;
        }
        public async Task<ICollection<Notification>> GetNotificationsOlderThan24HoursAsync()
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-1440);
            return await _context.Notifications
                .Where(n => n.CreatedAt < cutoffTime)
                .ToListAsync();
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}