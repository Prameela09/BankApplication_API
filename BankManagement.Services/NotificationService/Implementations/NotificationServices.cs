using BankManagement.Database.NotificationData.Entities;
using BankManagement.Database.NotificationData.Interfaces;
using BankManagement.Services.NotificationService.DTOs;
using BankManagement.Services.NotificationServices.Interfaces;
using AutoMapper;
using BankManagement.Utilities.HelperClasses;

namespace BankManagement.Services.NotificationServices.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public async Task<ICollection<NotificationDTO>> GetNotificationsByUserNameAsync(string username, PaginationInput pageDetails)
        {
            ICollection<Notification> notifications = await _notificationRepository.GetNotificationsByUserNameAsync(username);
            ICollection<NotificationDTO> notificationDtos = _mapper.Map<ICollection<NotificationDTO>>(notifications);

            return RetrieveNotificationsPerPage(pageDetails, notificationDtos);
        }

        public async Task<bool> SendNotificationAsync(int userId, string title, string message, int statusId)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                StatusId = statusId,
                CreatedAt = DateTime.Now,
                SentAt = DateTime.Now
            };

            return await _notificationRepository.AddNotificationAsync(notification);
        }

        public async Task<bool> DeleteOldNotificationsAsync()
        {
            var oldNotifications = await _notificationRepository.GetNotificationsOlderThan24HoursAsync();
            bool result = true;

            foreach (var notification in oldNotifications)
            {
                result &= await _notificationRepository.DeleteNotificationAsync(notification.Id);
            }

            return result;
        }

        public async Task<ICollection<Notification>> GetOldNotificationsAsync()
        {
            return await _notificationRepository.GetNotificationsOlderThan24HoursAsync();
        }

        private List<NotificationDTO> RetrieveNotificationsPerPage(PaginationInput pageDetails, ICollection<NotificationDTO> notifications)
        {
            return notifications
                   .Skip((pageDetails.PageNumber - 1) * pageDetails.NumOfEntities)
                   .Take(pageDetails.NumOfEntities)
                   .ToList();
        }
    }
}