using System;
using System.ComponentModel.DataAnnotations;

namespace BankManagement.Services.UserServices.DTOs;

public class LoginDTO
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$")]
    public string Password { get; set; }
}
