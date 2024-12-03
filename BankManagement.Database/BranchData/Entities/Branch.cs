using System.ComponentModel.DataAnnotations;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Utilities.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.BranchData.Entities;

[Index(nameof(BranchCode), IsUnique = true)]
public class Branch
{
    [Key]
    public int BranchId { get; set; }

    [Required]
    [StringLength(100)]
    public string BranchName { get; set; }

    [Required]
    public Location Location { get; set; } 

    [Required]
    [StringLength(50)]
    public string BranchCode { get; set; }

    [Required]
    public string IFSCCode { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}