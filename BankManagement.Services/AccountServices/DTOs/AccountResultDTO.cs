using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankManagement.Utilities.Enums;

namespace BankManagement.Services.AccountServices.DTOs;
public class AccountResultDTO
{
    public string UserName { get; set; }
    public AccountName AccountType { get; set; }
    public long AccountNumber { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Balance { get; set; }
    public string BranchCode { get; set; }
    public string IFSCCode { get; set; }
    public string AadharNumber { get; set; }
    public string PanNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
