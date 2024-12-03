using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.UserData.Entities;
using BankManagement.Database.UserData.Interfaces;
using BankManagement.Services.NotificationServices.Interfaces;
using BankManagement.Services.UserServices.DTOs;
using BankManagement.Services.UserServices.Interfaces;
using BankManagement.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BankManagement.Services.UserServices.Implementations;

public class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationServices;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserServices(IUserRepository userRepository, INotificationService notificationServices, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IMapper mapper)
    {
        _userRepository = userRepository;
        _notificationServices = notificationServices;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _mapper = mapper;
    }

    public string GetTermsAndConditions()
    {
        string terms = @"

        By registering with ABC Bank, you agree to the following terms and conditions:
        1. Acceptance of Terms: By accessing or using the services provided by ABC Bank, you agree to comply with these terms and conditions. If you do not agree, please do not use our services.
        2. Account Eligibility: To open an account with ABC Bank, you must be at least 18 years old and a resident of the country where the bank operates. 
        3. Account Security: You are responsible for maintaining the confidentiality of your account information, including your username and password. 
        4. Fees and Charges: ABC Bank reserves the right to charge fees for account maintenance, transactions, and other services. 
        5. Deposits and Withdrawals: All deposits made to your account are subject to verification and may be held for a period of time before being made available for withdrawal.
        6. Interest Rates: Interest rates on accounts may vary and are subject to change at the bank's discretion. The current interest rates will be provided to you.
        7. Privacy Policy: ABC Bank is committed to protecting your privacy. We will not disclose your personal information to third parties without your consent, except as required by law. 
        8. Limitation of Liability: ABC Bank will not be liable for any direct, indirect, incidental, or consequential damages arising from your use of our services.
        9. Governing Law: These terms and conditions shall be governed by and construed in accordance with the laws of the jurisdiction in which ABC Bank operates.
        10. Amendments: ABC Bank reserves the right to amend these terms and conditions at any time. You will be notified of any significant changes.

        ABC Bank Customer Service  

        ";
        return terms;
    }

    public async Task<bool> RegisterUserAsync(RegistrationDTO userRegistration)
    {
        User existingUser = await _userRepository.FindUser(u =>
                            u.UserName == userRegistration.UserName && u.Email == userRegistration.Email);

        if (existingUser != null)
        {
            if (!existingUser.IsActive)
            {
                await _notificationServices.SendNotificationAsync(existingUser.Id, "Registration Failed!", "Your account is inactive. Please recover your account first.", 3);
                return false;
            }

            await _notificationServices.SendNotificationAsync(existingUser.Id, "Registration Failed!", "User  with these details already exists!", 3);
            return false;
        }

        User user = _mapper.Map<User>(userRegistration);
        user.Profile ??= new UserProfile();
        user.LoginAt = DateTime.Now;
        user.IsActive = true;
        user.UserName = userRegistration.UserName;
        user.Email = userRegistration.Email;

        Role customerRole = await _userRepository.GetRoleByNameAsync(RoleName.Customer);
        if (customerRole != null)
        {
            user.Roles.Add(customerRole);
        }

        user.Profile.CreatedAt = DateTime.UtcNow;
        await _userRepository.AddUserAsync(user);
        await _notificationServices.SendNotificationAsync(user.Id, "Registration Successful!", "Start your Journey with our Bank!", 1);
        return true;

    }

    public async Task<string> LoginAsync(LoginDTO userLogin)
    {
        User user = await _userRepository.FindUser(u =>
                    u.UserName == userLogin.UserName && u.Password == userLogin.Password); ;

        if (user == null)
        {
            return null;
        }

        if (!user.IsActive)
        {
            return null;
        }

        bool hasRoles = user.Roles != null && user.Roles.Any();
        bool isAdmin = user.Roles.Any(role => role.Name == RoleName.Admin);

        if (!hasRoles && !isAdmin)
        {
            await _notificationServices.SendNotificationAsync(user.Id, "Login Failed!", "Check the credentials and login again!", 3);
            return null;
        }

        await _notificationServices.SendNotificationAsync(user.Id, "Login Successful!", "You are logged into your account successfully. Complete the profile!", 1);
        return GenerateToken(user);
    }

    public bool LogoutAsync(string token)
    {
        return true;
    }

    public async Task<UserDTO> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.FindUser(u => u.Id == id);
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.FindUser(u => u.UserName == username);
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<bool> AddUserAsync(UserDTO userDto, string password)
    {
        User user = _mapper.Map<User>(userDto);
        Role adminRole = await _userRepository.GetRoleByNameAsync(RoleName.Admin);
        user.Roles.Add(adminRole);

        user.LoginAt = DateTime.Now;
        user.IsActive = true;
        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
        user.Password = password;
        user.Profile.AccountHolderName = userDto.Profile.AccountHolderName;
        user.Profile.Address = userDto.Profile.Address;
        user.Profile.CreatedAt = DateTime.Now;
        user.Profile.DateOfBirth = userDto.Profile.DateOfBirth;
        userDto.Profile.PhoneNumber = userDto.Profile.PhoneNumber;

        return await _userRepository.AddUserAsync(user);
    }

    public async Task<bool> BlockUserAsync()
    {
        string username = GetCurrentUserName();
        User user = await _userRepository.FindUser(u =>
                    u.UserName == username);

        if (user != null)
        {
            user.IsActive = false;
            user.Profile.UpdatedAt = DateTime.UtcNow;
            ICollection<Account> accounts = user.Accounts;
            foreach (Account account in accounts)
            {
                account.IsActive = false;
            }
            
            await _notificationServices.SendNotificationAsync(user.Id, "Profile Blocked!", "Your Profile has been blocked! make a request if you want to recover it.", 1);
            await _userRepository.UpdateUser(user);
        }
        return false;
    }

    public async Task<bool> RecoverUserAccountAsync(RegistrationDTO userRecovery)
    {
        User user = await _userRepository.FindUser
        (
            u => u.UserName == userRecovery.UserName &&
            u.Email == userRecovery.Email &&
            u.Password == userRecovery.Password
        );

        if (user == null)
        {
            return false;
        }

        user.IsActive = true;
        ICollection<Account> accounts = user.Accounts;
        foreach (Account account in accounts)
        {
            account.IsActive = true;
        }

        await _userRepository.UpdateUser(user);
        _mapper.Map<UserRecoveryDTO>(user);
        return true;

    }

    public async Task<bool> EditUserProfileAync(string username, ProfileDetailsDTO updatedProfileDto)
    {
        var updatedProfile = _mapper.Map<UserProfile>(updatedProfileDto);
        UserProfile profile = await _userRepository.GetUserProfileAsync(username);
        profile.AccountHolderName = updatedProfile.AccountHolderName ?? profile.AccountHolderName;
        profile.Address = updatedProfile.Address ?? profile.Address;
        profile.PhoneNumber = updatedProfile.PhoneNumber ?? profile.PhoneNumber;
        profile.DateOfBirth = updatedProfile.DateOfBirth ?? profile.DateOfBirth;
        profile.UpdatedAt = DateTime.Now;

        return await _userRepository.UpdateUser(profile.User);
    }

    public async Task<UserDTO> GetCurrentUserDetails()
    {
        var username = GetCurrentUserName();
        var user = await _userRepository.FindUser(u => u.UserName == username);
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<bool> EditCurrentUserProfile(ProfileDetailsDTO profile)
    {
        var username = GetCurrentUserName();
        return await EditUserProfileAync(username, profile);
    }

    public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
    {
        var username = GetCurrentUserName();
        var user = await _userRepository.FindUser(u => u.UserName == username);
        if (user == null)
        {
            await _notificationServices.SendNotificationAsync(user.Id, "Password reset Failed!", "user not found with associated credentials!", 3);
            return false;
        }

        if (!(oldPassword == user.Password))
        {
            await _notificationServices.SendNotificationAsync(user.Id, "Password Reset Failed!", "Check the old password and try again!", 3);
            return false;
        }

        user.Password = newPassword;
        user.Profile.UpdatedAt = DateTime.Now;
        await _userRepository.UpdateUser(user);
        return true;
    }

    public string GetCurrentUserName()
    {
        string userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        return userName;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
