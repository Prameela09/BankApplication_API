using BankManagement.Services.NotificationService.DTOs;
using BankManagement.Services.NotificationServices.Interfaces;
using BankManagement.Services.UserServices.Interfaces;
using BankManagement.Utilities.HelperClasses;
using BankManagement.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[GlobalExceptionHandler]
public class NotificationManagementController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IUserServices _userServices;
    private readonly ILogger<NotificationManagementController> _logger;

    public NotificationManagementController(INotificationService notificationService, ILogger<NotificationManagementController> logger, IUserServices userServices)
    {
        _notificationService = notificationService;
        _userServices = userServices;
        _logger = logger;
    }

    [HttpGet("history")]
    [Authorize]
    public async Task<ActionResult<ICollection<NotificationDTO>>> GetNotificationHistory([FromQuery] PaginationInput pageDetails)
    {
        _logger.LogInformation("Retrieving notification history for the current user.");

        string username = _userServices.GetCurrentUserName();
        var notifications = await _notificationService.GetNotificationsByUserNameAsync(username, pageDetails);

        _logger.LogInformation("Notification history retrieved successfully for user {Username}: {@Notifications}", username, notifications);

        return Ok(notifications);
    }
}

