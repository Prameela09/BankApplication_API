using System.Threading.Tasks;
using BankManagement.Services.UserServices.DTOs;

namespace BankManagement.Services.UserServices.Interfaces
{
    public interface IUserServices
    {
        /// <summary>
        /// Retrieves the terms and conditions for the bank.
        /// </summary>
        string GetTermsAndConditions();

        /// <summary>
        /// Registers a new user asynchronously.
        /// Returns 'true' if registration is successful, otherwise 'false'.
        /// </summary>
        Task<bool> RegisterUserAsync(RegistrationDTO userRegistration);

        /// <summary>
        /// Logs in a user asynchronously.
        /// Returns a JWT token if login is successful, otherwise null.
        /// </summary>
        Task<string> LoginAsync(LoginDTO userLogin);

        /// <summary>
        /// Logs out the current user.
        /// Returns 'true' if logout is successful, otherwise 'false'.
        /// </summary>
        bool LogoutAsync(string token);

        /// <summary>
        /// Retrieves user details by user ID asynchronously.
        /// Returns a UserDTO object containing user details.
        /// </summary>
        Task<UserDTO> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves user details by username asynchronously.
        /// Returns a UserDTO object containing user details.
        /// </summary>
        Task<UserDTO> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Adds a new user asynchronously.
        /// Returns 'true' if the user is added successfully, otherwise 'false'.
        /// </summary>
        Task<bool> AddUserAsync(UserDTO userDto, string password);

        /// <summary>
        /// Blocks a user account asynchronously.
        /// Returns 'true' if the user is blocked successfully, otherwise 'false'.
        /// </summary>
        Task<bool> BlockUserAsync();

        /// <summary>
        /// Recovers a user account asynchronously.
        /// Returns 'true' if the account is recovered successfully, otherwise 'false'.
        /// </summary>
        Task<bool> RecoverUserAccountAsync(RegistrationDTO userRecovery);

        /// <summary>
        /// Edits a user profile asynchronously.
        /// Returns 'true' if the profile is updated successfully, otherwise 'false'.
        /// </summary>
        Task<bool> EditUserProfileAync(string username, ProfileDetailsDTO updatedProfileDto);

        /// <summary>
        /// Retrieves the current user's details asynchronously.
        /// Returns a UserDTO object containing the current user's details.
        /// </summary>
        Task<UserDTO> GetCurrentUserDetails();

        /// <summary>
        /// Edits the current user's profile asynchronously.
        /// Returns 'true' if the profile is updated successfully, otherwise 'false'.
        /// </summary>
        Task<bool> EditCurrentUserProfile(ProfileDetailsDTO profile);

        /// <summary>
        /// Changes the user's password after validating the old password.
        /// Returns 'true' if the password was changed successfully, otherwise 'false'.
        /// </summary>
        Task<bool> ChangePasswordAsync(string oldPassword, string newPassword);

        /// <summary>
        /// Retrieves the UserName of User who is currently login 
        /// </summary>
        string GetCurrentUserName();
    }
}