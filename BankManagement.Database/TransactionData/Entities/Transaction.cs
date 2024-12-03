using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.CommonEntities;


namespace BankManagement.Database.TransactionData.Entities;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required]
    public long AccountNumber { get; set; }

    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }

    [Required]
    public int TransactionTypeId { get; set; }

    [ForeignKey("TransactionTypeId")]
    public virtual TransactionType TransactionName { get; set; }

    [Required]
    public int SourceTypeId { get; set; }

    [ForeignKey("SourceTypeId")]
    public virtual SourceType SourceName { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime DateOfTransaction { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public int StatusTypeId { get; set; }

    [ForeignKey("StatusTypeId")]
    public virtual StatusType StatusName { get; set; }
}
