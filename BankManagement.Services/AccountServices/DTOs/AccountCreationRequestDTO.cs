using System;
using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.AccountServices.DTOs;

public class AccountCreationRequestDTO
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public AccountName AccountTypeName { get; set; }

    [Required]
    public string AadharNumber { get; set; }

    [Required]
    public decimal InitialAmount { get; set; }

    [Required]
    public string PanNumber { get; set; }

    [Required]
    public Location Branch { get; set; }

    [SwaggerIgnore]
    public int StatusTypeId { get; set; }

    [SwaggerIgnore]
    public int AccountTypeId { get; set; }

}
