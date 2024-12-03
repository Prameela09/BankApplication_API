using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankManagement.Database.BranchData.Entities;
using BankManagement.Database.CommonEntities;
using BankManagement.Database.TransactionData.Entities;
using BankManagement.Database.UserData.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.AccountData.Entities;


[Index(nameof(AccountNumber), IsUnique = true)]
public class Account
{
    [Key]
    public int AccountId { get; set; }

    [Required]
    public int AccountTypeId { get; set; }

    [ForeignKey("AccountTypeId")]
    public virtual AccountType AccountType { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [Required]
    public int BranchId { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch Branch { get; set; }

    [Required]
    public int StatusTypeId { get; set; }

    [ForeignKey("StatusTypeId")]
    public virtual StatusType Status { get; set; }

    [Required]
    public long AccountNumber { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Balance { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal CurrentInterestRate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal CurrentMonthlyFee { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<Transaction> Transactions { get; set; }
}