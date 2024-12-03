using BankManagement.Services.TransactionServices.DTOs;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;

namespace BankManagement.Services.TransactionServices.Interfaces
{
    public interface ITransactionServices
    {
        /// <summary>
        /// Retrieves the transaction history for the current user.
        /// It fetches all accounts associated with the user and retrieves their transactions.
        /// The results are paginated based on the provided page details.
        /// </summary>
        Task<List<TransactionDTO>> GetTransactionHistoryForCurrentUserAsync(PaginationInput pageDetails);

        /// <summary>
        /// Retrieves a specific transaction by its ID.
        /// It returns the transaction details mapped to a TransactionDTO.
        /// </summary>
        Task<TransactionDTO> GetTransactionByIdAsync(int transactionId);

        /// <summary>
        /// Retrieves transactions associated with a specific account number.
        /// The results are paginated based on the provided page details.
        /// </summary>
        Task<ICollection<TransactionDTO>> GetTransactionsByAccountNumberAsync(long accountNumber, PaginationInput pageDetails);

        /// <summary>
        /// Retrieves transactions filtered by their type.
        /// The results are paginated based on the provided page details.
        /// </summary>
        Task<ICollection<TransactionDTO>> GetTransactionsByTypeAsync(TransactionName transactionType, PaginationInput pageDetails);

        /// <summary>
        /// Retrieves transactions filtered by their source.
        /// The results are paginated based on the provided page details.
        /// </summary>
        Task<ICollection<TransactionDTO>> GetTransactionsBySourceAsync(TransactionSource source, PaginationInput pageDetails);

        /// <summary>
        /// Retrieves transactions filtered by their status.
        /// It returns a collection of TransactionResultDTO representing the transactions with the specified status.
        /// </summary>
        Task<ICollection<TransactionResultDTO>> GetTransactionsByStatusAsync(StatusName status);

        /// <summary>
        /// Performs a transaction for the current user.
        /// It validates the transaction details and updates the account balances accordingly.
        /// </summary>
        Task<TransactionDTO> PerformTransactionAsync(TransactionDTO transaction, long? targetAccountNumber = null);

        /// <summary>
        /// Retrieves the transaction history for a specified user by their username.
        /// It returns a list of TransactionDTO representing the user's transaction history.
        /// </summary>
        Task<List<TransactionDTO>> GetTransactionHistoryByUsernameAsync(string username, PaginationInput pageDetails);

        Task<ICollection<long>> GetAccountNumbersByUserNameAsync(string userName);
        Task<ICollection<TransactionDTO>> GetTransactionsBySourceForCurrentUserAsync(TransactionSource source, PaginationInput pageDetails);
        Task<ICollection<TransactionDTO>> GetTransactionsByTypeForCurrentUserAsync(TransactionName transactionType, PaginationInput pageDetails);
    }
}