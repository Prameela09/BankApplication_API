using System.ComponentModel.DataAnnotations;
using BankManagement.API.Filters;
using BankManagement.Services.UserServices.DTOs;
using BankManagement.Services.UserServices.Interfaces;
using BankManagement.Utilities.ExceptionHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankManagement.API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
// [GlobalExceptionHandler]
[ServiceFilter(typeof(UserValidationFilterAttribute))]
public class UserManagementController : ControllerBase
{
    private readonly IUserServices _userServices;

    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(IUserServices userServices, ILogger<UserManagementController> logger)
    {
        _userServices = userServices;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetTermsAndConditions()
    {
        var terms = _userServices.GetTermsAndConditions();
        HttpContext.Session.SetString("TermsAccepted", "true");
        return Ok(terms);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(RegistrationDTO registration)
    {

        var termsAccepted = HttpContext.Session.GetString("TermsAccepted");
        if (termsAccepted != "true")
        {
            _logger.LogWarning("User  registration failed: Please accept terms and conditions.");
            return BadRequest(new { message = "You must accept terms and conditions before registration." });
        }

        _logger.LogInformation("Registering user with email: {Email}", registration.Email);
        await _userServices.RegisterUserAsync(registration);

        _logger.LogInformation("User  registered successfully: {Email}", registration.Email);
        return Ok("User Registered successfully!");

    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {

        string token = await _userServices.LoginAsync(login);
        _logger.LogInformation("User  logged in successfully: {UserName}", login.UserName);
        return Ok(new { Token = "Bearer " + token, Message = "Login successfully" });

    }

    [HttpPost]
    public IActionResult Logout()
    {
        
        HttpContext.Session.Remove("TermsAccepted");
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        _userServices.LogoutAsync(token);
        _logger.LogInformation("User  logged out successfully. Token: {Token}", token);
        return Ok(new { message = "Logged out successfully." });

    }

    [HttpPost]
    public async Task<IActionResult> RecoverUserAccount([FromQuery][Required] string username, RegistrationDTO recoveryDetails)
    {

        _logger.LogInformation("Attempting to recover user account for username: {Username}", username);
        await _userServices.RecoverUserAccountAsync(recoveryDetails);

        _logger.LogInformation("User account for username: {Username} recovered successfully.", username);
        return Ok("User account recovered successfully.");

    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDTO>> GetUserById(int userId)
    {

        _logger.LogInformation("Retrieving user by ID: {User Id}", userId);
        UserDTO user = await _userServices.GetUserByIdAsync(userId);

        _logger.LogInformation("User  retrieved successfully: {@User }", user);
        return Ok(user);

    }

    [HttpGet("{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> GetUserByUsername(string username)
    {

        _logger.LogInformation("Retrieving user by username: {Username}", username);
        UserDTO user = await _userServices.GetUserByUsernameAsync(username);

        _logger.LogInformation("User  retrieved successfully: {@User }", user);
        return Ok(user);

    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddUser([FromForm] UserDTO userDto, [FromForm] string password)
    {

        _logger.LogInformation("Adding new user with details: {@User Dto}", userDto);
        await _userServices.AddUserAsync(userDto, password);

        _logger.LogInformation("User added successfully: {Username}", userDto.UserName);
        return Ok("User Added Successfull!");

    }

    [HttpPut("{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> EditProfile(string username, [FromBody] ProfileDetailsDTO updatedProfileDto)
    {

        _logger.LogInformation("Editing profile for user: {UserName}", username);
        await _userServices.EditUserProfileAync(username, updatedProfileDto);

        _logger.LogInformation("Profile edited successfully for user: {UserName}", username);
        return Ok($"Profile updated successfully for user: {username}");

    }

    [HttpDelete]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<ActionResult> BlockYourUserAccount()
    {

        _logger.LogInformation("Blocking Your Account........");
        await _userServices.BlockUserAsync();

        _logger.LogInformation("User Blocked successfully!");
        return Ok("Your Account Blocked Successfully!");

    }

    [HttpGet]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<ActionResult<UserDTO>> GetYourDetails()
    {

        _logger.LogInformation("Retrieving profile details for the current user.");
        UserDTO profile = await _userServices.GetCurrentUserDetails();

        _logger.LogInformation("Profile details retrieved successfully: {@Profile}", profile);
        return Ok(profile);

    }

    [HttpPut]
    [Authorize(Roles = "Admin, Customer")]
    public async Task<ActionResult> EditYourProfile([FromBody] ProfileDetailsDTO profile)
    {

        _logger.LogInformation("Editing profile for the current user.");
        await _userServices.EditCurrentUserProfile(profile);

        _logger.LogInformation("Profile updated successfully for the current user.");
        return Ok("Profile Updated successfully!");

    }

    [Authorize(Roles = "Admin, Customer")]
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDTO newPasswordDetails)
    {
        _logger.LogInformation("Changing password for the current user.");
        bool result = await _userServices.ChangePasswordAsync(newPasswordDetails.OldPassword, newPasswordDetails.NewPassword);

        if (!result)
        {
            _logger.LogWarning("Password change failed for the current user: Invalid old password or user not found.");
            return Unauthorized("Invalid old password or user not found.");
        }

        _logger.LogInformation("Password changed successfully for the current user.");
        return Ok("Password changed successfully.");
    }
}
