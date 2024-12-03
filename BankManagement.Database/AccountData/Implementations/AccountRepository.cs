using System.Linq.Expressions;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.AccountData.Interfaces;
using BankManagement.Database.DbContexts;
using BankManagement.Utilities.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.AccountData.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly BankDataContext _accountContext;

    public AccountRepository(BankDataContext accountContext)
    {
        _accountContext = accountContext;
    }

    public async Task<List<Account>> GetAllAccountsAsync()
    {
        return await _accountContext.Accounts.ToListAsync();
    }

    public async Task<Account> FindAccount(Expression<Func<Account, bool>> accountDetails)
    {
        return await GetAccountsWithIncludes().FirstOrDefaultAsync(accountDetails);
    }

    public async Task<Account> GetAccountByAccountNumberAsync(long accountNumber)
    {
        return await GetAccountsWithIncludes()
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.IsActive == true);
    }

    public async Task<List<Account>> GetAccountByUsernameAsync(string username)
    {
        return await GetAccountsWithIncludes()
            .Where(a => a.User.UserName == username && a.IsActive == true)
            .ToListAsync();
    }

    public async Task<AccountType> GetAccountTypeByNameAsync(AccountName accountTypeName)
    {
        return await _accountContext.AccountTypes
            .FirstOrDefaultAsync(at => at.AccountTypeName == accountTypeName);
    }

    public async Task<bool> CreateAccountAsync(Account account)
    {
        await _accountContext.Accounts.AddAsync(account);
        return await _accountContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAccountAsync(Account account)
    {
        _accountContext.Accounts.Update(account);
        return await _accountContext.SaveChangesAsync() > 0;
    }

    private IQueryable<Account> GetAccountsWithIncludes()
    {
        return _accountContext.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.User)
            .Include(a => a.Branch);
    }
}
