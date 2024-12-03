using System;
using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.TransactionData.Entities;

public class TransactionType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public TransactionName Name { get; set; }
}
