using System.Linq.Expressions;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.AccountData.Interfaces;

public interface IAccountRepository
{
    // <summary>
    /// Retrieves all accounts from the database.
    /// </summary>
    Task<List<Account>> GetAllAccountsAsync();
    Task<Account> FindAccount(Expression<Func<Account, bool>> accountDetails);

    /// <summary>
    /// Retrieves a list of accounts associated with a specified username.
    /// It returns a list of Account representing the accounts for the given user.
    /// </summary>
    Task<List<Account>> GetAccountByUsernameAsync(string username);

    /// <summary>
    /// Retrieves the details of a specific account by its account number.
    /// It returns an Account representing the details of the requested account.
    /// </summary>
    Task<Account> GetAccountByAccountNumberAsync(long accountNumber);

    /// <summary>
    /// Creates a new account using the provided account details.
    /// It returns a boolean indicating whether the account creation was successful.
    /// </summary>
    Task<bool> CreateAccountAsync(Account account);

    /// <summary>
    /// Updates an existing account using the provided account details.
    /// It returns a boolean indicating whether the update was successful.
    /// </summary>
    Task<bool> UpdateAccountAsync(Account account);

    /// <summary>
    /// Retrieves the account type associated with a specified account type name.
    /// It returns an AccountType representing the details of the requested account type.
    /// </summary>
    Task<AccountType> GetAccountTypeByNameAsync(AccountName accountTypeName);
}
