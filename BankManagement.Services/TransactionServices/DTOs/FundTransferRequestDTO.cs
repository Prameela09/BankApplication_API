using System;
using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.TransactionServices.DTOs;

public class FundTransferRequestDTO
{
    public long SourceAccountNumber { get; set; }
    public string TargetUsername { get; set; }
    public long TargetAccountNumber { get; set; }
    public decimal Amount { get; set; }
    public TransactionName TransactionType { get; set; }
    public TransactionSource Source { get; set; }

    [SwaggerIgnore]
    public DateTime TransactionDate { get; set; }
    
    [SwaggerIgnore]
    public int StatusTypeId { get; set; }

    [SwaggerIgnore]
    public int TransactionTypeId { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }
}
