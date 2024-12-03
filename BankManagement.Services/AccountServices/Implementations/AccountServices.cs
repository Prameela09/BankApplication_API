using AutoMapper;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.AccountData.Interfaces;
using BankManagement.Database.BranchData.Entities;
using BankManagement.Database.UserData.Entities;
using BankManagement.Services.AccountServices.DTOs;
using BankManagement.Services.AccountServices.Interfaces;
using BankManagement.Services.BranchServices.Interfaces;
using BankManagement.Services.NotificationServices.Interfaces;
using BankManagement.Services.TransactionServices.DTOs;
using BankManagement.Services.TransactionServices.Interfaces;
using BankManagement.Services.UserServices.Interfaces;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;

namespace BankManagement.Services.AccountServices.Implementations;

public class AccountServices : IAccountServices
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBranchServices _branchServices;
    private readonly IUserServices _userServices;
    // private readonly ITransactionServices _transactionServices;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public AccountServices(IAccountRepository accountRepository,IBranchServices branchServices,
        IUserServices userServices, INotificationService notificationService,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _branchServices = branchServices;
        _userServices = userServices;
        // _transactionServices = transactionServices;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    // public async Task<AccountOverviewDTO> GetMyAccountOverviewAsync()
    // {
    //     var accounts = await GetAccountsForCurrentUserAsync();
    //     var accountTransactions = await GetAccountTransactionsAsync(accounts);

    //     return new AccountOverviewDTO
    //     {
    //         Accounts = accounts,
    //         AccountTransactions = accountTransactions
    //     };
    // }

    public async Task<List<AccountResultDTO>> GetAccountsForCurrentUserAsync()
    {
        string currentUserName = _userServices.GetCurrentUserName();
        List<Account> accounts = await _accountRepository.GetAccountByUsernameAsync(currentUserName);
        return _mapper.Map<List<AccountResultDTO>>(accounts);
    }

    public async Task<AccountResultDTO> GetAccountByAccountNumberAsync(long accountNumber)
    {
        Account account = await _accountRepository.GetAccountByAccountNumberAsync(accountNumber);
        return _mapper.Map<AccountResultDTO>(account);
    }

    public async Task<List<AccountResultDTO>> GetAccountsByUsernameAsync(string username)
    {
        var user = await GetUserAsync(username);
        var accounts = await _accountRepository.GetAccountByUsernameAsync(username);
        return _mapper.Map<List<AccountResultDTO>>(accounts);
    }

    public async Task<AccountResultDTO> CreateAccountAsync(AccountCreationRequestDTO request)
    {
        var user = await GetUserAsync(request.UserName);
        ValidateAadharAndPan(request.AadharNumber, request.PanNumber, user);

        var selectedBranchCode = await GetRandomBranchCodeAsync(request.Branch);
        var branch = await GetBranchByCodeAsync(selectedBranchCode);

        var accountType = await GetAccountTypeAsync(request.AccountTypeName);
        var initialBalance = ValidateInitialAmount(request.InitialAmount, accountType);

        var account = CreateAccountEntity(user, branch, accountType, initialBalance);
        await CreateAccountInRepositoryAsync(account, user.Id);

        return MapToAccountResultDTO(account, branch, user);
    }

    public async Task<bool> BranchTransferForAccountAsync(AccountBranchTransferDTO branchTransferDetails)
    {
        var account = await _accountRepository.GetAccountByAccountNumberAsync(branchTransferDetails.AccountNumber);
        if (account == null) return await NotifyAccountUpdateFailureAsync(account.User.Id);
        
        var selectedBranchCode = await GetRandomBranchCodeAsync(branchTransferDetails.Location);
        var branch = await GetBranchByCodeAsync(selectedBranchCode);
        account.Branch = branch;
        _mapper.Map(branchTransferDetails, account);
        account.UpdatedAt = DateTime.UtcNow;

        await _notificationService.SendNotificationAsync(account.User.Id, "Account Update Successful!", "Your account details have been updated.", 1);
        return await _accountRepository.UpdateAccountAsync(account);
    }

    public async Task<bool> BlockAccountAsync(long accountNumber)
    {
        Account account = await _accountRepository.GetAccountByAccountNumberAsync(accountNumber);
        if (account == null) return false;

        account.IsActive = false;
        return await _accountRepository.UpdateAccountAsync(account);
    }

    public async Task<bool> RecoverAccountAsync(long accountNumber)
    {
        Account account = await _accountRepository.FindAccount
        (
            a => a.AccountNumber == accountNumber
        );

        if (account == null)
        {
            return false;
        }
        account.IsActive = true;

        await _accountRepository.UpdateAccountAsync(account);
        _mapper.Map<AccountResultDTO>(account);
        return true;
    }
    
    public async Task UpdateAccountAsync(AccountResultDTO accountDto)
    {
        var account = _mapper.Map<Account>(accountDto);
        await _accountRepository.UpdateAccountAsync(account);
    }

    public async Task<Dictionary<string, List<AccountResultDTO>>> GetInactiveAccountsAsync()
    {
        var allAccounts = await _accountRepository.GetAllAccountsAsync();
        var inactiveAccounts = allAccounts.Where(a => !a.IsActive);

        var groupedInactiveAccounts = inactiveAccounts
            .GroupBy(a => a.User.UserName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(account => _mapper.Map<AccountResultDTO>(account)).ToList()
            );

        return groupedInactiveAccounts;
    }

    private async Task<User> GetUserAsync(string username)
    {
        var userDto = await _userServices.GetUserByUsernameAsync(username);
        var user = _mapper.Map<User>(userDto);
        if (user == null) throw new ArgumentException("User not found.", nameof(username));
        return user;
    }

    private void ValidateAadharAndPan(string aadharNumber, string panNumber, User user)
    {
        if (!AccountHelper.VerifyAadharAndPan(aadharNumber, panNumber))
        {
            throw new ArgumentException("Invalid Aadhaar or PAN number.");
        }
        user.AadharNumber = aadharNumber;
        user.PanNumber = panNumber;
    }

    private async Task<string> GetRandomBranchCodeAsync(Location location)
    {
        var branches = await _branchServices.GetBranchByLocationAsync(location);
        var branchCodes = branches.Select(b => b.BranchCode).ToList();

        if (!branchCodes.Any()) throw new ArgumentException("No branches found for the specified location.", nameof(location));

        Random random = new();
        return branchCodes[random.Next(branchCodes.Count)];
    }

    private async Task<Branch> GetBranchByCodeAsync(string branchCode)
    {
        var branchDto = await _branchServices.GetBranchByCodeAsync(branchCode);
        var branch = _mapper.Map<Branch>(branchDto);
        if (branch == null) throw new ArgumentException("Branch not found for the specified code.", nameof(branchCode));
        return branch;
    }

    private async Task<AccountType> GetAccountTypeAsync(AccountName accountTypeName)
    {
        var accountType = await _accountRepository.GetAccountTypeByNameAsync(accountTypeName);
        if (accountType == null) throw new ArgumentException("Account type not found.", nameof(accountTypeName));
        return accountType;
    }

    private decimal ValidateInitialAmount(decimal initialAmount, AccountType accountType)
    {
        if (accountType.AccountTypeName == AccountName.Savings && initialAmount < accountType.MinimumBalance)
        {
            throw new ArgumentException($"Savings account must have a minimum balance of {accountType.MinimumBalance}.", nameof(initialAmount));
        }
        return initialAmount;
    }

    private Account CreateAccountEntity(User user, Branch branch, AccountType accountType, decimal initialBalance)
    {
        return new Account
        {
            UserId = user.Id,
            BranchId = branch.BranchId,
            AccountTypeId = accountType.Id,
            StatusTypeId = 1,
            AccountNumber = AccountHelper.GenerateAccountNumber(),
            Balance = initialBalance,
            CurrentInterestRate = accountType.AccountTypeName == AccountName.Savings ? AccountHelper.CalculateInterestRate(initialBalance) : 0,
            CurrentMonthlyFee = accountType.AccountTypeName == AccountName.Current ? AccountHelper.CalculateMonthlyFee(initialBalance) : 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        };
    }

    private async Task CreateAccountInRepositoryAsync(Account account, int userId)
    {
        bool isCreated = await _accountRepository.CreateAccountAsync(account);
        if (!isCreated)
        {
            await _notificationService.SendNotificationAsync(userId, "Account Creation Failed!", "Your account creation has failed.", 3);
            throw new Exception("Failed to create account.");
        }
        await _notificationService.SendNotificationAsync(userId, "Account Creation Successful!", "Your account has been created successfully.", 1);
    }

    private AccountResultDTO MapToAccountResultDTO(Account account, Branch branch, User user)
    {
        return new AccountResultDTO
        {
            UserName = user.UserName,
            AccountType = account.AccountType.AccountTypeName,
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            BranchCode = branch.BranchCode,
            IFSCCode = branch.IFSCCode,
            AadharNumber = user.AadharNumber,
            PanNumber = user.PanNumber,
            CreatedAt = account.CreatedAt
        };
    }

    // private async Task<Dictionary<long, List<TransactionResultDTO>>> GetAccountTransactionsAsync(List<AccountResultDTO> accounts)
    // {
    //     var accountTransactions = new Dictionary<long, List<TransactionResultDTO>>();

    //     foreach (var account in accounts)
    //     {
    //         var transactions = await _transactionServices.GetTransactionsByAccountNumberAsync(account.AccountNumber, new PaginationInput
    //         {
    //             PageNumber = 1,
    //             NumOfEntities = 5
    //         });

    //         accountTransactions[account.AccountNumber] = _mapper.Map<List<TransactionResultDTO>>(transactions.ToList());
    //     }

    //     return accountTransactions;
    // }

    private async Task<bool> NotifyAccountUpdateFailureAsync(int userId)
    {
        await _notificationService.SendNotificationAsync(userId, "Account Update Failed!", "Update of your account failed.", 3);
        return false;
    }
}
