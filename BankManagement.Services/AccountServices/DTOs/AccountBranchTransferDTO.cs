using System;
using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.AccountServices.DTOs;

public class AccountBranchTransferDTO
{
    [Required]
    public long AccountNumber { get; set; }
    
    public Location Location { get; set; }
    
    [SwaggerIgnore]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
