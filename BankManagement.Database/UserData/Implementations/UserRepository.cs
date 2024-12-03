using System.Linq.Expressions;
using BankManagement.Database.DbContexts;
using BankManagement.Database.UserData.Entities;
using BankManagement.Database.UserData.Interfaces;
using BankManagement.Utilities.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.UserData.Implementations;

public class UserRepository : IUserRepository
{
    private readonly BankDataContext _userContext;

    public UserRepository(BankDataContext userContext)
    {
        _userContext = userContext;
    }


    public async Task<Role> GetRoleByNameAsync(RoleName roleName)
    {
        return await _userContext.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    public async Task<User> FindUser(Expression<Func<User, bool>> userDetails)
    {
        return await GetUsersWithIncludes().FirstOrDefaultAsync(userDetails);
    }

    public async Task<bool> AddUserAsync(User user)
    {
        await _userContext.Users.AddAsync(user);
        return await _userContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUser(User user)
    {
        _userContext.Users.Update(user);
        return await _userContext.SaveChangesAsync() > 0;
    }

    public async Task<UserProfile> GetUserProfileAsync(string username)
    {
        User user = await FindUser( u => u.UserName == username);
        if (user == null)
        {
            return null;
        }

        UserProfile userProfile = await _userContext.UserProfiles
            .Include(u => u.User)
            .FirstOrDefaultAsync(up => up.UserId == user.Id);

        if (userProfile == null)
        {
            return null;
        }

        return userProfile;
    }

    private IQueryable<User> GetUsersWithIncludes()
    {
        return _userContext.Users.Include(u => u.Profile).Include(u => u.Roles);
    }
}
