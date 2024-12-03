using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankManagement.Database.CommonEntities;
using BankManagement.Database.UserData.Entities;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.NotificationData.Entities;

public class Notification
{

    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    [StringLength(1000)]
    public string Message { get; set; }

    [Required]
    public int StatusId { get; set; }

    [ForeignKey("StatusId")]
    public virtual StatusType Status { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

