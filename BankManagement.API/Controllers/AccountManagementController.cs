using BankManagement.API.Filters;
using BankManagement.Services.AccountServices.DTOs;
using BankManagement.Services.AccountServices.Interfaces;
using BankManagement.Utilities.ExceptionHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [GlobalExceptionHandler]
    [ServiceFilter(typeof(AccountValidationFilterAttribute))]
    [ServiceFilter(typeof(UserValidationFilterAttribute))]
    public class AccountManagementController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        private readonly ILogger<AccountManagementController> _logger;

        public AccountManagementController(IAccountServices accountServices, ILogger<AccountManagementController> logger)
        {
            _accountServices = accountServices;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult<List<AccountResultDTO>>> MyAccountDetails()
        {
            _logger.LogInformation("Retrieving accounts for the current user.");
            var accounts = await _accountServices.GetAccountsForCurrentUserAsync();
            return Ok(accounts);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult<AccountOverviewDTO>> GetMyAccountOverview()
        {
            _logger.LogInformation("Retrieving account overview for the current user.");
            var accountOverview = await _accountServices.GetMyAccountOverviewAsync();
            return Ok(accountOverview);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult<AccountResultDTO>> CreateAccount([FromQuery] AccountCreationRequestDTO accountDto)
        {
            _logger.LogInformation("Creating account with details: {@AccountDto}", accountDto);
            var createdAccount = await _accountServices.CreateAccountAsync(accountDto);
            return Ok(createdAccount);
        }

        [HttpPatch]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult> BranchTransferForAccount([FromQuery] AccountBranchTransferDTO accountDto)
        {
            _logger.LogInformation("Updating account with ID: {AccountId}", accountDto.AccountNumber);
            await _accountServices.BranchTransferForAccountAsync(accountDto);
            return Ok("Account location updated successfully!");
        }

        [HttpDelete("{accountNumber}")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult> BlockYourAccount(long accountNumber)
        {
            _logger.LogInformation("Deleting account with number: {AccountNumber}", accountNumber);
            await _accountServices.BlockAccountAsync(accountNumber);
            return Ok("Account Blocked Successfully!");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult> RecoverYourAccount([FromQuery] long accountNumber)
        {
            _logger.LogInformation("Recovering account with number: {AccountNumber}", accountNumber);
            await _accountServices.RecoverAccountAsync(accountNumber);
            return Ok("Account recovered successfully!");
        }

        [HttpGet("{accountNumber}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AccountResultDTO>> GetAccountDetailsByAccountNumber(long accountNumber)
        {
            _logger.LogInformation("Retrieving account details for account number: {AccountNumber}", accountNumber);
            var account = await _accountServices.GetAccountByAccountNumberAsync(accountNumber);
            return Ok(account);
        }

        [HttpGet("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AccountResultDTO>> GetAccountDetailsByUserName(string username)
        {
            _logger.LogInformation("Retrieving account details for account number: {UserName}", username);
            var accounts = await _accountServices.GetAccountsByUsernameAsync(username);
            return Ok(accounts);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Dictionary<string, List<AccountResultDTO>>>> GetInactiveAccounts()
        {
            _logger.LogInformation("Fetching inactive accounts.");
            var inactiveAccounts = await _accountServices.GetInactiveAccountsAsync();
            return Ok(inactiveAccounts);
        }
    }
}
