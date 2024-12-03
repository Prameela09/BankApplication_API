using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagement.Database.UserData.Entities;

public class UserProfile
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string AccountHolderName { get; set; }

    public long? PhoneNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(1000)]
    public string Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [ForeignKey("User")]
    public int UserId { get; set; }

    public virtual User User { get; set; }
}