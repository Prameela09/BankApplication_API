using System.Linq.Expressions;
using BankManagement.Database.UserData.Entities;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.UserData.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a role by its name asynchronously.
        /// Returns the Role object if found, otherwise null.
        /// </summary>
        Task<Role> GetRoleByNameAsync(RoleName roleName);

        /// <summary>
        /// Finds a user based on the provided user details expression.
        /// Returns the User object if found, otherwise null.
        /// </summary>
        Task<User> FindUser(Expression<Func<User, bool>> userDetails);

        /// <summary>
        /// Adds a new user asynchronously.
        /// Returns true if the user was added successfully, otherwise false.
        /// </summary>
        Task<bool> AddUserAsync(User user);

        /// <summary>
        /// Updates an existing user asynchronously.
        /// Returns true if the user was updated successfully, otherwise false.
        /// </summary>
        Task<bool> UpdateUser(User user);

        /// <summary>
        /// Retrieves the user profile for a given username asynchronously.
        /// Returns the UserProfile object if found, otherwise null.
        /// </summary>
        Task<UserProfile> GetUserProfileAsync(string username);
    }
}