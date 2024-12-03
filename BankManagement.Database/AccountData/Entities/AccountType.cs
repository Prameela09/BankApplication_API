using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankManagement.Database.UserData.Entities;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.AccountData.Entities;

public class AccountType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public AccountName AccountTypeName { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal InterestRate { get; set; } 

    [Column(TypeName = "decimal(10, 2)")]
    public decimal WithdrawLimit { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal MinimumBalance { get; set; } 

    [Column(TypeName = "decimal(10, 2)")]
    public decimal OverDraft { get; set; } 

    [Column(TypeName = "decimal(10, 2)")]
    public decimal MonthlyFee { get; set; } 
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>(); 
}