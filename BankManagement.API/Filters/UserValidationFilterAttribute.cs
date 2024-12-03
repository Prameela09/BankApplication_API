// using BankManagement.Services.UserServices.DTOs;
// using BankManagement.Services.UserServices.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;

// namespace BankManagement.API.Filters;

// public class UserValidationFilterAttribute : IAsyncActionFilter
// {
//     private readonly IUserServices _userServices;
//     private readonly ILogger<UserValidationFilterAttribute> _logger;

//     public UserValidationFilterAttribute(IUserServices userServices, ILogger<UserValidationFilterAttribute> logger)
//     {
//         _userServices = userServices;
//         _logger = logger;
//     }

//     public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         if (context.ActionDescriptor.RouteValues["action"] == "RegisterUser")
//         {
//             var registerDto = context.ActionArguments["registration"] as RegistrationDTO;

//             if (registerDto != null)
//             {
//                 var existingUser = await _userServices.GetUserByUsernameAsync(registerDto.UserName);
//                 if (existingUser != null && registerDto.IsActive)
//                 {
//                     _logger.LogWarning("User registration failed: {User Name} already exists.", registerDto.UserName);
//                     context.Result = new BadRequestObjectResult(new { message = "User already exists." });
//                     return;
//                 }
//                 else if (existingUser != null && !registerDto.IsActive)
//                 {
//                     _logger.LogWarning("User registration failed. First recover your account!");
//                     context.Result = new BadRequestObjectResult(new { message = "You alreay have account. Just recover your account!" });
//                     return;
//                 }
//             }
//         }

//         else if (context.ActionDescriptor.RouteValues["action"] == "Login")
//         {
//             var loginDto = context.ActionArguments["login"] as LoginDTO;

//             if (loginDto != null)
//             {
//                 var user = await _userServices.LoginAsync(loginDto);
//                 if (user == null)
//                 {
//                     _logger.LogWarning("Login failed for user: {User Name}. Invalid credentials.", loginDto.UserName);
//                     context.Result = new UnauthorizedObjectResult(new { message = "Invalid credentials." });
//                     return;
//                 }
//             }
//         }

//         else if (context.ActionDescriptor.RouteValues["action"] == "RecoverUserAccount")
//         {
//             var recoveryDetails = context.ActionArguments["recoveryDetails"] as RegistrationDTO;

//             if (recoveryDetails != null)
//             {
//                 var IsUserExists = await _userServices.RecoverUserAccountAsync(recoveryDetails);
//                 if (!IsUserExists)
//                 {
//                     _logger.LogWarning("User Login failed. Invalid Credentials!");
//                     context.Result = new UnauthorizedObjectResult(new { message = "Invalid Credentials!" });
//                     return;
//                 }
//             }
//         }

//         else if (context.ActionArguments.TryGetValue("userId", out var idObj) && idObj is int id)
//         {
//             if (id < 0)
//             {
//                 _logger.LogWarning("Invalid user ID: {UserId}. ID cannot be negative.", id);
//                 context.Result = new BadRequestObjectResult(new { error = "User ID cannot be negative." });
//                 return;
//             }

//             var user = await _userServices.GetUserByIdAsync(id);
//             if (user == null)
//             {
//                 _logger.LogWarning("User not found: {User  Id}", id);
//                 context.Result = new NotFoundObjectResult(new { error = "User not found." });
//                 return;
//             }
//         }

//         else if (context.ActionArguments.TryGetValue("username", out var usernameObj) && usernameObj is string username)
//         {
//             if (string.IsNullOrWhiteSpace(username))
//             {
//                 _logger.LogWarning("Invalid username provided: {Username}.", username);
//                 context.Result = new BadRequestObjectResult(new { error = "Invalid username." });
//                 return;
//             }

//             var user = await _userServices.GetUserByUsernameAsync(username);
//             if (user == null)
//             {
//                 _logger.LogWarning("User  not found: {Username}", username);
//                 context.Result = new NotFoundObjectResult(new { error = "Invalid username." });
//                 return;
//             }
//         }
//         await next();
//     }
// }


using BankManagement.Services.UserServices.DTOs;
using BankManagement.Services.UserServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankManagement.API.Filters;
public class UserValidationFilterAttribute : IAsyncActionFilter
{
    private readonly IUserServices _userServices;
    private readonly ILogger<UserValidationFilterAttribute> _logger;

    public UserValidationFilterAttribute(IUserServices userServices, ILogger<UserValidationFilterAttribute> logger)
    {
        _userServices = userServices;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor.RouteValues["action"] == "RegisterUser ")
        {
            var registerDto = context.ActionArguments["registration"] as RegistrationDTO;
            if (registerDto != null)
            {
                var existingUser  = await _userServices.GetUserByUsernameAsync(registerDto.UserName);
                if (existingUser  != null)
                {
                    var message = registerDto.IsActive 
                        ? "User  already exists." 
                        : "You already have an account. Just recover your account!";
                    _logger.LogWarning("User  registration failed: {Message}", message);
                    context.Result = new BadRequestObjectResult(new { message });
                    return;
                }
            }
        }

        if (context.ActionDescriptor.RouteValues["action"] == "Login")
        {
            var loginDto = context.ActionArguments["login"] as LoginDTO;
            if (loginDto != null && await _userServices.LoginAsync(loginDto) == null)
            {
                _logger.LogWarning("Login failed for user: {User Name}. Invalid credentials.", loginDto.UserName);
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid credentials." });
                return;
            }
        }

        if (context.ActionDescriptor.RouteValues["action"] == "RecoverUser Account")
        {
            var recoveryDetails = context.ActionArguments["recoveryDetails"] as RegistrationDTO;
            if (recoveryDetails != null && !await _userServices.RecoverUserAccountAsync(recoveryDetails))
            {
                _logger.LogWarning("User  recovery failed. Invalid credentials!");
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid credentials!" });
                return;
            }
        }

        if (context.ActionArguments.TryGetValue("userId", out var idObj) && idObj is int id)
        {
            if (id < 0)
            {
                _logger.LogWarning("Invalid user ID: {User Id}. ID cannot be negative.", id);
                context.Result = new BadRequestObjectResult(new { error = "User  ID cannot be negative." });
                return;
            }

            var user = await _userServices.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User  not found: {User Id}", id);
                context.Result = new NotFoundObjectResult(new { error = "User  not found." });
                return;
            }
        }

        if (context.ActionArguments.TryGetValue("username", out var usernameObj) && usernameObj is string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("Invalid username provided: {Username}.", username);
                context.Result = new BadRequestObjectResult(new { error = "Invalid username." });
                return;
            }

            var user = await _userServices.GetUserByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User  not found: {Username}", username);
                context.Result = new NotFoundObjectResult(new { error = "User  not found." });
                return;
            }
        }

        await next();
    }
}