using System.ComponentModel.DataAnnotations;

namespace BankManagement.Services.UserServices.DTOs;

public class UserDTO
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public ProfileDetailsDTO Profile { get; set; }
}

