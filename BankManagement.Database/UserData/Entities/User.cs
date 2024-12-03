using System;
using System.ComponentModel.DataAnnotations;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.NotificationData.Entities;
using Microsoft.EntityFrameworkCore;
namespace BankManagement.Database.UserData.Entities;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(AadharNumber), IsUnique = true)]
[Index(nameof(PanNumber), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public string Password { get; set; }

    public DateTime LoginAt { get; set; } = DateTime.Now;

    public DateTime LogoutAt { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;

    public virtual List<Role> Roles { get; set; } = new List<Role>(); 

    public virtual UserProfile Profile { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual List<Notification> Notifications { get; set; } = new List<Notification>();

    public string AadharNumber { get; set; }

    public string PanNumber { get; set; }
}