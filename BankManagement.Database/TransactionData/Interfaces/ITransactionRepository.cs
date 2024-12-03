using System.Linq.Expressions;
using BankManagement.Database.CommonEntities;
using BankManagement.Database.TransactionData.Entities;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.TransactionData.Interfaces;

public interface ITransactionRepository
{
    /// <summary>
    /// Creates a new transaction using the provided transaction details and associated account number.
    /// It returns the created Transaction representing the new transaction.
    /// </summary>
    Task<Transaction> CreateTransactionAsync(Transaction transaction, long accountNumber);

    /// <summary>
    /// Retrieves the details of a specific transactions based on respective transaction Details.
    /// It returns a collection of Transaction representing the details of the requested transaction.
    /// </summary>
    Task<ICollection<Transaction>> FindTransactionsAsync(Expression<Func<Transaction, bool>> transactionDetails);

    /// <summary>
    /// Retrieves the transaction type associated with a specified transaction type name.
    /// It returns a TransactionType representing the details of the requested transaction type.
    /// </summary>
    Task<TransactionType> GetTransactionTypeByNameAsync(TransactionName transactionTypeId);

    /// <summary>
    /// Retrieves the source type associated with a specified transaction source.
    /// It returns a SourceType representing the details of the requested source type.
    /// </summary>
    Task<SourceType> GetSourceTypeByNameAsync(TransactionSource sourceType);

    /// <summary>
    /// Retrieves the status type associated with a specified status type ID.
    /// It returns a StatusType representing the details of the requested status type.
    /// </summary>
    Task<StatusType> GetStatusTypeByIdAsync(int statusType);
}
