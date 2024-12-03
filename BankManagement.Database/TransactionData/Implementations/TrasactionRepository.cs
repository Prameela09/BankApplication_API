using System.Linq.Expressions;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.AccountData.Interfaces;
using BankManagement.Database.CommonEntities;
using BankManagement.Database.DbContexts;
using BankManagement.Database.TransactionData.Entities;
using BankManagement.Database.TransactionData.Interfaces;
using BankManagement.Utilities.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.TransactionData.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankDataContext _context;
        private readonly IAccountRepository _accountRepository;

        public TransactionRepository(BankDataContext context, IAccountRepository accountRepository)
        {
            _context = context;
            _accountRepository = accountRepository;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction, long accountNumber)
        {
            Account account = await _accountRepository.GetAccountByAccountNumberAsync(accountNumber);
            transaction.Account = account;

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
        
        public async Task<ICollection<Transaction>> FindTransactionsAsync(Expression<Func<Transaction, bool>> transactionDetails)
        {
            return await GetTransactionsWithIncludes()
                .Where(transactionDetails)
                .ToListAsync();
        }

        public async Task<TransactionType> GetTransactionTypeByNameAsync(TransactionName transactionType)
        {
            return await _context.TransactionTypes
                .FirstOrDefaultAsync(tt => tt.Name == transactionType);
        }

        public async Task<SourceType> GetSourceTypeByNameAsync(TransactionSource sourceType)
        {
            return await _context.SourceTypes
                .FirstOrDefaultAsync(st => st.SourceName == sourceType);
        }

        public async Task<StatusType> GetStatusTypeByIdAsync(int statusType)
        {
            return await _context.StatusTypes
                .FirstOrDefaultAsync(st => st.StatusTypeId == statusType);
        }
        
        private IQueryable<Transaction> GetTransactionsWithIncludes()
        {
            return _context.Transactions.Include(t => t.Account)
                                         .Include(t => t.TransactionName)
                                         .Include(t => t.SourceName);
        }
    }
}