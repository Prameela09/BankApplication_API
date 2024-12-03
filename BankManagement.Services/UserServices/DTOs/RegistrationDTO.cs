using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.UserServices.DTOs;

public class RegistrationDTO
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [MinLength(8)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$")]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [SwaggerIgnore]
    public bool IsActive { get; set; }

}
