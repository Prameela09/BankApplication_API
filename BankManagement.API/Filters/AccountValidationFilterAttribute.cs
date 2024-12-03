using BankManagement.Services.AccountServices.Interfaces;
using BankManagement.Services.AccountServices.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankManagement.API.Filters
{
    public class AccountValidationFilterAttribute : IAsyncActionFilter
    {
        private readonly IAccountServices _accountServices;
        private readonly ILogger<AccountValidationFilterAttribute> _logger;

        public AccountValidationFilterAttribute(IAccountServices accountServices, ILogger<AccountValidationFilterAttribute> logger)
        {
            _accountServices = accountServices;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("accountNumber", out var accountNumberObj) && accountNumberObj is long accountNumber)
            {
                if (accountNumber <= 0)
                {
                    _logger.LogWarning("Invalid account number: {AccountNumber}. Account number must be positive.", accountNumber);
                    context.Result = new BadRequestObjectResult(new { error = "Account number must be positive." });
                    return;
                }

                var account = await _accountServices.GetAccountByAccountNumberAsync(accountNumber);
                if (account == null)
                {
                    _logger.LogWarning("Account not found: {AccountNumber}", accountNumber);
                    context.Result = new NotFoundObjectResult(new { error = "Account not found." });
                    return;
                }
            }

            if (context.ActionArguments.TryGetValue("accountDto", out var accountDtoObj) && accountDtoObj is AccountBranchTransferDTO accountDto)
            {
                if (string.IsNullOrWhiteSpace(accountDto.Location.ToString()))
                {
                    _logger.LogWarning("Invalid location provided for account transfer: {Location}.", accountDto.Location);
                    context.Result = new BadRequestObjectResult(new { error = "Location cannot be empty." });
                    return;
                }

                var account = await _accountServices.GetAccountByAccountNumberAsync(accountDto.AccountNumber);
                if (account == null)
                {
                    _logger.LogWarning("Account not found for transfer: {AccountNumber}", accountDto.AccountNumber);
                    context.Result = new NotFoundObjectResult(new { error = "Account not found." });
                    return;
                }
            }
            await next();
        }
    }
}