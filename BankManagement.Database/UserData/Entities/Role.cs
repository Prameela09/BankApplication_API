using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.UserData.Entities;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    public RoleName Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
