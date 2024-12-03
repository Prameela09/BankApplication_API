using System;
using System.ComponentModel.DataAnnotations;

namespace BankManagement.Services.UserServices.DTOs;

public class ChangePasswordDTO
{
    [Required]
    public string OldPassword { get; set; } 

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$")]
    public string NewPassword { get; set; }
}
