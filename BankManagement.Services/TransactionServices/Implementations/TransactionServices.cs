// using AutoMapper;
// using BankManagement.Database.AccountData.Entities;
// using BankManagement.Database.AccountData.Interfaces;
// using BankManagement.Database.TransactionData.Entities;
// using BankManagement.Database.TransactionData.Interfaces;
// using BankManagement.Database.UserData.Interfaces;
// using BankManagement.Services.TransactionServices.DTOs;
// using BankManagement.Services.TransactionServices.Interfaces;
// using BankManagement.Services.UserServices.Interfaces;
// using BankManagement.Utilities.Enums;
// using BankManagement.Utilities.HelperClasses;

// namespace BankManagement.Services.TransactionServices.Implementations
// {
//     public class TransactionServices : ITransactionServices
//     {
//         private readonly ITransactionRepository _transactionRepository;
//         private readonly IAccountRepository _accountRepository;
//         private readonly IUserRepository _userRepository;
//         private readonly IUserServices _userServices;
//         private readonly IMapper _mapper;

//         public TransactionServices(ITransactionRepository transactionRepository,
//                                    IAccountRepository accountRepository,
//                                    IUserRepository userRepository,
//                                    IUserServices userServices,
//                                    IMapper mapper)
//         {
//             _transactionRepository = transactionRepository;
//             _accountRepository = accountRepository;
//             _userRepository = userRepository;
//             _userServices = userServices;
//             _mapper = mapper;
//         }

//         public async Task<List<TransactionDTO>> GetTransactionHistoryForCurrentUserAsync(PaginationInput pageDetails)
//         {
//             var currentUserName = _userServices.GetCurrentUserName();
//             var accounts = await _accountRepository.GetAccountByUsernameAsync(currentUserName);

//             if (accounts == null || !accounts.Any())
//             {
//                 return new List<TransactionDTO>();
//             }

//             var allTransactions = new List<Transaction>();
//             foreach (var account in accounts)
//             {
//                 var transactions = await _transactionRepository.FindTransactionsAsync( t => t.Account.AccountId == account.AccountId);
//                 allTransactions.AddRange(transactions);
//             }

//             var transactionDtos = _mapper.Map<List<TransactionDTO>>(allTransactions);
//             return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
//         }

//         public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
//         {
//             var transaction = await _transactionRepository.FindTransactionsAsync(t => t.Id == transactionId);
//             return _mapper.Map<TransactionDTO>(transaction);
//         }

//         public async Task<ICollection<TransactionDTO>> GetTransactionsByAccountNumberAsync(long accountNumber, PaginationInput pageDetails)
//         {
//             var transactions = await _transactionRepository.FindTransactionsAsync(t => t.Account.AccountNumber == accountNumber);
//             var transactionDtos = transactions.Select(t => _mapper.Map<TransactionDTO>(t)).ToList();
//             return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
//         }

//         public async Task<ICollection<TransactionDTO>> GetTransactionsByTypeAsync(TransactionName transactionType, PaginationInput pageDetails)
//         {
//             var transactions = await _transactionRepository.FindTransactionsAsync(t => t.TransactionName.Name == transactionType);
//             var transactionDtos = transactions.Select(t => _mapper.Map<TransactionDTO>(t)).ToList();
//             return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
//         }

//         public async Task<ICollection<TransactionDTO>> GetTransactionsBySourceAsync(TransactionSource source, PaginationInput pageDetails)
//         {
//             var transactions = await _transactionRepository.FindTransactionsAsync(t => t.SourceName.SourceName == source);
//             var transactionDtos = transactions.Select(t => _mapper.Map<TransactionDTO>(t)).ToList();
//             return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
//         }

//         public async Task<ICollection<TransactionResultDTO>> GetTransactionsByStatusAsync(StatusName status)
//         {
//             var transactions = await _transactionRepository.FindTransactionsAsync(t => t.StatusName.Status == status);
//             return _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
//         }

//         public async Task<TransactionDTO> PerformTransactionAsync(TransactionDTO transaction)
//         {
//             var account = await _accountRepository.GetAccountByAccountNumberAsync(transaction.AccountNumber);
//             if (account == null)
//             {
//                 throw new ArgumentException("Account not found.");
//             }

//             var transactionType = await _transactionRepository.GetTransactionTypeByNameAsync(transaction.TransactionType);
//             if (transactionType == null)
//             {
//                 throw new ArgumentException("Transaction type not found.", nameof(transaction.TransactionType));
//             }

//             var sourceType = await _transactionRepository.GetSourceTypeByNameAsync(transaction.SourceName);
//             if (sourceType == null)
//             {
//                 throw new ArgumentException("Source type not found.", nameof(transaction.SourceName));
//             }

//             var newTransaction = new Transaction
//             {
//                 AccountId = account.AccountId,
//                 AccountNumber = account.AccountNumber,
//                 TransactionTypeId = transactionType.Id,
//                 SourceTypeId = sourceType.SourceTypeId,
//                 StatusTypeId = 1,
//                 Amount = transaction.Amount,
//                 DateOfTransaction = DateTime.UtcNow,
//                 Description = transaction.Description,
//             };
//             var createdTransaction = await _transactionRepository.CreateTransactionAsync(newTransaction, account.AccountNumber);
//             return _mapper.Map<TransactionDTO>(createdTransaction);
//         }

//         public async Task<string> TransferFundsAsync(FundTransferRequestDTO fundTransferRequest)
//         {
//             var sourceAccount = await _accountRepository.GetAccountByAccountNumberAsync(fundTransferRequest.SourceAccountNumber);
//             var targetAccount = await _accountRepository.GetAccountByAccountNumberAsync(fundTransferRequest.TargetAccountNumber);

//             if (sourceAccount == null || targetAccount == null)
//             {
//                 throw new ArgumentException("Source or target account not found.");
//             }

//             if (fundTransferRequest.Amount <= 0)
//             {
//                 throw new ArgumentException("Transfer amount must be greater than zero.");
//             }

//             if (IsDebitTransaction(fundTransferRequest.TransactionType))
//             {
//                 if (sourceAccount.Balance < fundTransferRequest.Amount)
//                 {
//                     throw new InvalidOperationException("Insufficient balance in source account.");
//                 }

//                 sourceAccount.Balance -= fundTransferRequest.Amount;
//                 targetAccount.Balance += fundTransferRequest.Amount;

//                 await _accountRepository.UpdateAccountAsync(sourceAccount);
//                 await _accountRepository.UpdateAccountAsync(targetAccount);
//             }

//             await PerformTransactionAsync(new TransactionDTO
//             {
//                 AccountNumber = fundTransferRequest.SourceAccountNumber,
//                 Amount = fundTransferRequest.Amount, 
//                 TransactionType = fundTransferRequest.TransactionType,
//                 SourceName = fundTransferRequest.Source,
//                 Description = $"Transferred {fundTransferRequest.Amount} to account {fundTransferRequest.TargetAccountNumber}."
//             });

//             await PerformTransactionAsync(new TransactionDTO
//             {
//                 AccountNumber = fundTransferRequest.TargetAccountNumber,
//                 Amount = fundTransferRequest.Amount, 
//                 TransactionType = fundTransferRequest.TransactionType,
//                 SourceName = fundTransferRequest.Source,
//                 Description = $"Received {fundTransferRequest.Amount} from account {fundTransferRequest.SourceAccountNumber}."
//             });

//             return $"Amount {fundTransferRequest.Amount} transferred successfully through source: {fundTransferRequest.Source}.";
//         }


//         public async Task<List<TransactionDTO>> GetTransactionHistoryByUsernameAsync(string username, PaginationInput pageDetails)
//         {
//             var user = await _userRepository.FindUser(u => u.UserName == username);
//             if (user == null)
//             {
//                 throw new InvalidOperationException("User not found.");
//             }

//             var accountNumbers = user.Accounts.Select(a => a.AccountNumber).ToList();
//             var transactions = await _transactionRepository.FindTransactionsAsync(t => accountNumbers.Contains(t.AccountNumber));
//             var transactionDtos = _mapper.Map<List<TransactionDTO>>(transactions);
//             return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
//         }

//         private async Task<TransactionDTO> HandleAccountTransaction(Account account, TransactionDTO transaction)
//         {
//             if (transaction.TransactionType == TransactionName.Deposit)
//             {
//                 return await ProcessDeposit(account, transaction);
//             }

//             if (IsDebitTransaction(transaction.TransactionType))
//             {
//                 ValidateTransaction(account, transaction);
//                 account.Balance -= transaction.Amount;
//                 await _accountRepository.UpdateAccountAsync(account);
//             }

//             await UpdateAccountInterestAndFees(account);
//             return await CreateTransaction(transaction);
//         }

//         private async Task<TransactionDTO> ProcessDeposit(Account account, TransactionDTO transaction)
//         {
//             account.Balance += transaction.Amount;
//             await UpdateAccountInterestAndFees(account);
//             await _accountRepository.UpdateAccountAsync(account);
//             return await CreateTransaction(transaction);
//         }

//         private async Task<TransactionDTO> CreateTransaction(TransactionDTO transaction)
//         {
//             var newTransaction = _mapper.Map<Transaction>(transaction);
//             newTransaction.DateOfTransaction = DateTime.UtcNow;
//             var createdTransaction = await _transactionRepository.CreateTransactionAsync(newTransaction, transaction.AccountNumber);
//             return _mapper.Map<TransactionDTO>(createdTransaction);
//         }

//         private async Task<TransactionDTO> CreateTransactionAsync(long accountNumber, decimal amount, TransactionName transactionType, TransactionSource source)
//         {
//             var transactionDto = new TransactionDTO
//             {
//                 Amount = amount,
//                 TransactionType = transactionType,
//                 SourceName = source,
//                 AccountNumber = accountNumber,
//                 StatusTypeId = 1,
//                 DateOfTransaction = DateTime.UtcNow
//             };

//             return await CreateTransaction(transactionDto);
//         }

//         private void ValidateTransaction(Account account, TransactionDTO transaction)
//         {
//             if (account.AccountType.AccountTypeName == AccountName.Savings)
//             {
//                 ValidateSavingsTransaction(account, transaction.Amount);
//             }
//             else if (account.AccountType.AccountTypeName == AccountName.Current)
//             {
//                 ValidateCurrentTransaction(account, transaction.Amount);
//             }
//             else
//             {
//                 throw new InvalidOperationException("Unsupported account type.");
//             }
//         }

//         private void ValidateSavingsTransaction(Account account, decimal amount)
//         {
//             decimal minimumBalance = AccountHelper.GetMinimumBalance(account.AccountType.AccountTypeName);
//             if (account.Balance - amount < minimumBalance)
//             {
//                 throw new InvalidOperationException("Insufficient balance. Minimum balance requirement not met.");
//             }
//         }

//         private void ValidateCurrentTransaction(Account account, decimal amount)
//         {
//             if (account.Balance - amount < -AccountHelper.GetOverDraft())
//             {
//                 throw new InvalidOperationException("Insufficient balance. Overdraft limit exceeded.");
//             }
//         }

//         private bool IsDebitTransaction(TransactionName transactionType)
//         {
//             return transactionType == TransactionName.WithDraw ||
//                    transactionType == TransactionName.OnlinePurchase ||
//                    transactionType == TransactionName.Transfer ||
//                    transactionType == TransactionName.Subscription;
//         }

//         private async Task UpdateAccountInterestAndFees(Account account)
//         {
//             if (account.AccountType.AccountTypeName == AccountName.Savings)
//             {
//                 account.CurrentInterestRate = AccountHelper.CalculateInterestRate(account.Balance);
//             }
//             if (account.AccountType.AccountTypeName == AccountName.Current)
//             {
//                 account.CurrentMonthlyFee = AccountHelper.CalculateMonthlyFee(account.Balance);
//             }
//             await _accountRepository.UpdateAccountAsync(account);
//         }

//         private List<TransactionDTO> RetrieveTransactionsPerPage(PaginationInput pageDetails, List<TransactionDTO> transactions)
//         {
//             return transactions
//                    .Skip((pageDetails.PageNumber - 1) * pageDetails.NumOfEntities)
//                    .Take(pageDetails.NumOfEntities)
//                    .ToList();
//         }
//     }
// }




using AutoMapper;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.TransactionData.Entities;
using BankManagement.Database.TransactionData.Interfaces;
using BankManagement.Database.UserData.Entities;
using BankManagement.Services.AccountServices.DTOs;
using BankManagement.Services.AccountServices.Interfaces;
using BankManagement.Services.TransactionServices.DTOs;
using BankManagement.Services.TransactionServices.Interfaces;
using BankManagement.Services.UserServices.Interfaces;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;

namespace BankManagement.Services.TransactionServices.Implementations
{
    public class TransactionServices : ITransactionServices
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountServices _accountServices;
        private readonly IUserServices _userServices;
        private readonly IMapper _mapper;

        public TransactionServices(ITransactionRepository transactionRepository,
                                   IAccountServices accountServices,
                                   IUserServices userServices,
                                   IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _userServices = userServices;
            _accountServices = accountServices;
            _mapper = mapper;
        }

        public async Task<List<TransactionDTO>> GetTransactionHistoryForCurrentUserAsync(PaginationInput pageDetails)
        {
            var currentUserName = _userServices.GetCurrentUserName();
            var accounts = await _accountServices.GetAccountsByUsernameAsync(currentUserName);

            if (accounts == null || !accounts.Any())
            {
                return new List<TransactionDTO>();
            }

            var allTransactions = new List<Transaction>();
            foreach (var account in accounts)
            {
                var transactions = await _transactionRepository.FindTransactionsAsync(t => t.Account.AccountNumber == account.AccountNumber);
                allTransactions.AddRange(transactions);
            }

            var transactionDtos = _mapper.Map<List<TransactionDTO>>(allTransactions);
            return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
        }

        public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _transactionRepository.FindTransactionsAsync(t => t.Id == transactionId);
            return _mapper.Map<TransactionDTO>(transaction);
        }

        public async Task<ICollection<TransactionDTO>> GetTransactionsByAccountNumberAsync(long accountNumber, PaginationInput pageDetails)
        {
            var transactions = await _transactionRepository.FindTransactionsAsync(t => t.Account.AccountNumber == accountNumber);
            var transactionDtos = transactions.Select(t => _mapper.Map<TransactionDTO>(t)).ToList();
            return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
        }

        public async Task<ICollection<TransactionDTO>> GetTransactionsByTypeAsync(TransactionName transactionType, PaginationInput pageDetails)
        {
            var transactions = await _transactionRepository.FindTransactionsAsync(t => t.TransactionName.Name == transactionType);
            var transactionDtos = transactions.Select(t => _mapper.Map<TransactionDTO>(t)).ToList();
            return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
        }

        public async Task<ICollection<TransactionDTO>> GetTransactionsByTypeForCurrentUserAsync(TransactionName transactionType, PaginationInput pageDetails)
        {
            string currentUserName = _userServices.GetCurrentUserName();
            var allTransactionDtos = await GetTransactionsByTypeAsync(transactionType, pageDetails);
            var allTransactions = _mapper.Map<List<Transaction>>(allTransactionDtos);
            var userTransactions = allTransactions.Where(t => t.Account.User.UserName == currentUserName).ToList();

            return _mapper.Map<ICollection<TransactionDTO>>(userTransactions);
        }

        public async Task<ICollection<TransactionDTO>> GetTransactionsBySourceAsync(TransactionSource source, PaginationInput pageDetails)
        {
            var transactions = await _transactionRepository.FindTransactionsAsync(t => t.SourceName.SourceName == source);
            var transactionDtos = transactions.Select(t => _mapper.Map<TransactionDTO>(t)).ToList();
            return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
        }

        public async Task<ICollection<TransactionDTO>> GetTransactionsBySourceForCurrentUserAsync(TransactionSource source, PaginationInput pageDetails)
        {
            string currentUserName = _userServices.GetCurrentUserName();
            var allTransactionDtos = await GetTransactionsBySourceAsync(source, pageDetails);
            var allTransactions = _mapper.Map<List<Transaction>>(allTransactionDtos);
            var userTransactions = allTransactions.Where(t => t.Account.User.UserName == currentUserName).ToList();

            return _mapper.Map<ICollection<TransactionDTO>>(userTransactions);
        }

        public async Task<ICollection<TransactionResultDTO>> GetTransactionsByStatusAsync(StatusName status)
        {
            var transactions = await _transactionRepository.FindTransactionsAsync(t => t.StatusName.Status == status);
            return _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
        }

        public async Task<TransactionDTO> PerformTransactionAsync(TransactionDTO transaction, long? targetAccountNumber = null)
        {
            var account = await _accountServices.GetAccountByAccountNumberAsync(transaction.AccountNumber);
            if (account == null)
            {
                throw new ArgumentException("Account not found.");
            }

            var transactionType = await _transactionRepository.GetTransactionTypeByNameAsync(transaction.TransactionType);
            if (transactionType == null)
            {
                throw new ArgumentException("Transaction type not found.", nameof(transaction.TransactionType));
            }

            var sourceType = await _transactionRepository.GetSourceTypeByNameAsync(transaction.SourceName);
            if (sourceType == null)
            {
                throw new ArgumentException("Source type not found.", nameof(transaction.SourceName));
            }

            if (!IsValidTransactionCombination(transaction.TransactionType, transaction.SourceName))
            {
                throw new ArgumentException("Invalid combination of transaction type and source type.");
            }

            if (transaction.TransactionType == TransactionName.Transfer)
            {
                if (!targetAccountNumber.HasValue)
                {
                    throw new ArgumentException("Target account number must be provided for transfer transactions.");
                }

                var targetAccount = await _accountServices.GetAccountByAccountNumberAsync(targetAccountNumber.Value);
                if (targetAccount == null)
                {
                    throw new ArgumentException("Target account not found.");
                }

                if (account.Balance < transaction.Amount)
                {
                    throw new InvalidOperationException("Insufficient balance in source account.");
                }

                account.Balance -= transaction.Amount;
                targetAccount.Balance += transaction.Amount;

                await _accountServices.UpdateAccountAsync(account);
                await _accountServices.UpdateAccountAsync(targetAccount);

                await CreateTransactionAsync(transaction.AccountNumber, transaction.Amount, transaction.TransactionType, transaction.SourceName, $"Transferred {transaction.Amount} to account {targetAccountNumber.Value}.");
                await CreateTransactionAsync(targetAccountNumber.Value, transaction.Amount, transaction.TransactionType, transaction.SourceName, $"Received {transaction.Amount} from account {transaction.AccountNumber}.");
            }
            else
            {
                await HandleAccountTransaction(_mapper.Map<Account>(account), transaction);
            }

            return await CreateTransaction(transaction);
        }

        public async Task<List<TransactionDTO>> GetTransactionHistoryByUsernameAsync(string username, PaginationInput pageDetails)
        {
            var userDto = await _userServices.GetUserByUsernameAsync(username);
            if (userDto == null)
            {
                throw new InvalidOperationException("User  not found.");
            }
            
            User user = _mapper.Map<User>(userDto);
            var accountNumbers = user.Accounts.Select(a => a.AccountNumber).ToList();
            var transactions = await _transactionRepository.FindTransactionsAsync(t => accountNumbers.Contains(t.AccountNumber));
            var transactionDtos = _mapper.Map<List<TransactionDTO>>(transactions);
            return RetrieveTransactionsPerPage(pageDetails, transactionDtos);
        }

        public async Task<ICollection<long>> GetAccountNumbersByUserNameAsync(string userName)
        {
            var accountResults = await _accountServices.GetAccountsByUsernameAsync(userName);

            var accountNumbers = accountResults.Select(account => account.AccountNumber).ToList();

            return accountNumbers;
        }
        private async Task<TransactionDTO> HandleAccountTransaction(Account account, TransactionDTO transaction)
        {
            if (transaction.TransactionType == TransactionName.Deposit)
            {
                return await ProcessDeposit(account, transaction);
            }

            if (IsDebitTransaction(transaction.TransactionType))
            {
                ValidateTransaction(account, transaction);
                account.Balance -= transaction.Amount;
                await _accountServices.UpdateAccountAsync(_mapper.Map<AccountResultDTO>(account));
            }

            await UpdateAccountInterestAndFees(account);
            return await CreateTransaction(transaction);
        }

        private async Task<TransactionDTO> ProcessDeposit(Account account, TransactionDTO transaction)
        {
            account.Balance += transaction.Amount;
            await UpdateAccountInterestAndFees(account);
            await _accountServices.UpdateAccountAsync(_mapper.Map<AccountResultDTO>(account));
            return await CreateTransaction(transaction);
        }

        private async Task<TransactionDTO> CreateTransaction(TransactionDTO transaction)
        {
            var newTransaction = _mapper.Map<Transaction>(transaction);
            newTransaction.DateOfTransaction = DateTime.UtcNow;
            var createdTransaction = await _transactionRepository.CreateTransactionAsync(newTransaction, transaction.AccountNumber);
            return _mapper.Map<TransactionDTO>(createdTransaction);
        }

        private async Task<TransactionDTO> CreateTransactionAsync(long accountNumber, decimal amount, TransactionName transactionType, TransactionSource source, string description)
        {
            var transactionDto = new TransactionDTO
            {
                Amount = amount,
                TransactionType = transactionType,
                SourceName = source,
                AccountNumber = accountNumber,
                StatusTypeId = 1,
                DateOfTransaction = DateTime.UtcNow,
                Description = description
            };

            return await CreateTransaction(transactionDto);
        }

        private bool IsValidTransactionCombination(TransactionName transactionType, TransactionSource sourceType)
        {
            switch (transactionType)
            {
                case TransactionName.Deposit:

                    // Deposits can be made using Cash, Check, or Other
                    return sourceType == TransactionSource.Cash ||
                           sourceType == TransactionSource.Check ||
                           sourceType == TransactionSource.Other;

                case TransactionName.WithDraw:

                    // Withdrawals can be made using Debit Card, Cash, or Check
                    return sourceType == TransactionSource.DebitCard ||
                           sourceType == TransactionSource.Cash ||
                           sourceType == TransactionSource.Check;

                case TransactionName.OnlinePurchase:

                    // Online purchases can be made using Credit Card, Debit Card, PhonePe, Google Pay, or Paytm
                    return sourceType == TransactionSource.CreditCard ||
                           sourceType == TransactionSource.DebitCard ||
                           sourceType == TransactionSource.PhonePe ||
                           sourceType == TransactionSource.GooglePay ||
                           sourceType == TransactionSource.Paytm;

                case TransactionName.Transfer:

                    // Transfers can be made using Debit Card, PhonePe, Google Pay, Paytm, or Other
                    return sourceType == TransactionSource.DebitCard ||
                           sourceType == TransactionSource.PhonePe ||
                           sourceType == TransactionSource.GooglePay ||
                           sourceType == TransactionSource.Paytm ||
                           sourceType == TransactionSource.Other;

                case TransactionName.Subscription:

                    // Subscriptions can be made using Credit Card, Debit Card, PhonePe, Google Pay, or Paytm
                    return sourceType == TransactionSource.CreditCard ||
                           sourceType == TransactionSource.DebitCard ||
                           sourceType == TransactionSource.PhonePe ||
                           sourceType == TransactionSource.GooglePay ||
                           sourceType == TransactionSource.Paytm;

                default:
                    return false;
            }
        }
        private void ValidateTransaction(Account account, TransactionDTO transaction)
        {
            if (account.AccountType.AccountTypeName == AccountName.Savings)
            {
                ValidateSavingsTransaction(account, transaction.Amount);
            }
            else if (account.AccountType.AccountTypeName == AccountName.Current)
            {
                ValidateCurrentTransaction(account, transaction.Amount);
            }
            else
            {
                throw new InvalidOperationException("Unsupported account type.");
            }
        }

        private void ValidateSavingsTransaction(Account account, decimal amount)
        {
            decimal minimumBalance = AccountHelper.GetMinimumBalance(account.AccountType.AccountTypeName);
            if (account.Balance - amount < minimumBalance)
            {
                throw new InvalidOperationException("Insufficient balance. Minimum balance requirement not met.");
            }
        }

        private void ValidateCurrentTransaction(Account account, decimal amount)
        {
            if (account.Balance - amount < -AccountHelper.GetOverDraft())
            {
                throw new InvalidOperationException("Insufficient balance. Overdraft limit exceeded.");
            }
        }

        private bool IsDebitTransaction(TransactionName transactionType)
        {
            return transactionType == TransactionName.WithDraw ||
                   transactionType == TransactionName.OnlinePurchase ||
                   transactionType == TransactionName.Transfer ||
                   transactionType == TransactionName.Subscription;
        }

        private async Task UpdateAccountInterestAndFees(Account account)
        {
            if (account.AccountType.AccountTypeName == AccountName.Savings)
            {
                account.CurrentInterestRate = AccountHelper.CalculateInterestRate(account.Balance);
            }
            if (account.AccountType.AccountTypeName == AccountName.Current)
            {
                account.CurrentMonthlyFee = AccountHelper.CalculateMonthlyFee(account.Balance);
            }
            await _accountServices.UpdateAccountAsync(_mapper.Map<AccountResultDTO>(account));
        }

        private List<TransactionDTO> RetrieveTransactionsPerPage(PaginationInput pageDetails, List<TransactionDTO> transactions)
        {
            return transactions
                   .Skip((pageDetails.PageNumber - 1) * pageDetails.NumOfEntities)
                   .Take(pageDetails.NumOfEntities)
                   .ToList();
        }
    }
}