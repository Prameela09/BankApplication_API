using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.UserServices.DTOs;

public class UserRecoveryDTO
{
    [SwaggerIgnore]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; }
    
    public string AadharNumber { get; set; }

    public string PanNumber { get; set; }
}
