using System;
using System.ComponentModel.DataAnnotations;


namespace BankManagement.Services.UserServices.DTOs;

public class ProfileDetailsDTO
{
    [MaxLength(100)]
    public string AccountHolderName { get; set; }
    public long? PhoneNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    [MaxLength(1000)]
    public string Address { get; set; }
}
