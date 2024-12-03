using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.TransactionServices.DTOs;

public class TransactionResultDTO
{
    [Required]
    public long AccountNumber { get; set; }

    [Required]
    public TransactionName TransactionType { get; set; }

    [Required]
    public TransactionSource SourceName { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [SwaggerIgnore]
    public DateTime DateOfTransaction { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string Description { get; set; }
}
