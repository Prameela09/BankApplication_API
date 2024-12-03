using BankManagement.Services.AccountServices.DTOs;

namespace BankManagement.Services.AccountServices.Interfaces
{
    public interface IAccountServices
    {
        /// <summary>
        /// Retrieves a list of accounts associated with the current user.
        /// It returns a list of AccountResultDTO representing the user's accounts.
        /// </summary>
        Task<List<AccountResultDTO>> GetAccountsForCurrentUserAsync();

        /// <summary>
        /// Retrieves an overview of all accounts associated with the current user,
        /// including account details and recent transactions for each account.
        /// This method returns an AccountOverviewDTO containing a list of AccountResultDTO
        /// representing the user's accounts and a dictionary mapping each account number
        /// to a list of TransactionResultDTO representing the recent transactions for that account.
        /// </summary>
        // Task<AccountOverviewDTO> GetMyAccountOverviewAsync();

        /// <summary>
        /// Retrieves the details of a specific account by its account number.
        /// It returns an AccountResultDTO representing the details of the requested account.
        /// </summary>
        Task<AccountResultDTO> GetAccountByAccountNumberAsync(long accountNumber);

        /// <summary>
        /// Retrieves a list of accounts associated with a specified username.
        /// It returns a list of AccountResultDTO representing the accounts for the given user.
        /// </summary>
        Task<List<AccountResultDTO>> GetAccountsByUsernameAsync(string username);

        /// <summary>
        /// Creates a new account using the provided account creation request details.
        /// It returns an AccountResultDTO representing the details of the newly created account.
        /// </summary>
        Task<AccountResultDTO> CreateAccountAsync(AccountCreationRequestDTO request);

        /// <summary>
        /// Updates an existing account using the provided account updation details.
        /// It returns a boolean indicating whether the update was successful.
        /// </summary>
        Task<bool> BranchTransferForAccountAsync(AccountBranchTransferDTO accountUpdateDto);

        Task UpdateAccountAsync(AccountResultDTO accountDto);

        /// <summary>
        /// De-Activates an account based on the provided account number by making IsActive property false.
        /// It returns a boolean indicating whether the deletion was successful.
        /// </summary>
        Task<bool> BlockAccountAsync(long accountNumber);

        Task<bool> RecoverAccountAsync(long accountNumber);

        Task<Dictionary<string, List<AccountResultDTO>>> GetInactiveAccountsAsync();
    }
}