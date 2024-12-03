using System.Security.Claims;
using BankManagement.Services.TransactionServices.DTOs;
using BankManagement.Services.TransactionServices.Interfaces;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankManagement.API.Filters
{
    public class TransactionValidationFilterAttribute : IAsyncActionFilter
    {
        private readonly ITransactionServices _transactionServices;
        private readonly ILogger<TransactionValidationFilterAttribute> _logger;

        public TransactionValidationFilterAttribute(ITransactionServices transactionServices, ILogger<TransactionValidationFilterAttribute> logger)
        {
            _transactionServices = transactionServices;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("transactionId", out var transactionIdObj) && transactionIdObj is int transactionId)
            {
                _logger.LogInformation("Retrieving transaction by ID: {TransactionId}", transactionId);
                var transaction = await _transactionServices.GetTransactionByIdAsync(transactionId);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction not found: {TransactionId}", transactionId);
                    context.Result = new NotFoundObjectResult("Transaction not found.");
                    return;
                }

                context.HttpContext.Items["Transaction"] = transaction;

                _logger.LogInformation("Transaction retrieved successfully: {@Transaction}", transaction);
            }
            else
            {
                _logger.LogWarning("Invalid transaction ID provided.");
                context.Result = new BadRequestObjectResult(new { error = "Invalid transaction ID." });
                return;
            }

            if (context.ActionArguments.TryGetValue("accountNumber", out var accountNumberObj) && accountNumberObj is long accountNumber)
            {
                var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == "Admin")
                {
                    await next();
                    return;
                }
                else if (userRole == "Customer")
                {
                    var currentUserAccountNumbers = await _transactionServices.GetAccountNumbersByUserNameAsync(context.HttpContext.User.Identity.Name);
                    if (!currentUserAccountNumbers.Contains(accountNumber))
                    {
                        _logger.LogWarning("Customer attempted to access unauthorized account number: {AccountNumber}", accountNumber);
                        context.Result = new ForbidResult("You do not have access to this account number.");
                        return;
                    }
                }
            }

            else
            {
                _logger.LogWarning("Invalid account number provided.");
                context.Result = new BadRequestObjectResult(new { error = "Invalid account number." });
                return;
            }

            if (context.ActionArguments.TryGetValue("source", out var sourceObj) && sourceObj is TransactionSource source &&
                        context.ActionArguments.TryGetValue("pageDetails", out var pageDetailsObj) && pageDetailsObj is PaginationInput pageDetails)
            {
                var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                _logger.LogInformation("User  role: {User Role}", userRole);
                ICollection<TransactionDTO> transactions;

                if (userRole == "Admin")
                {
                    transactions = await _transactionServices.GetTransactionsBySourceAsync(source, pageDetails);
                }
                else
                {
                    transactions = await _transactionServices.GetTransactionsBySourceForCurrentUserAsync(source, pageDetails);
                }

                context.HttpContext.Items["Transactions"] = transactions;
                _logger.LogInformation("Transactions retrieved successfully for role: {User Role}", userRole);
            }

            else
            {
                _logger.LogWarning("Invalid source or page details provided.");
                context.Result = new BadRequestObjectResult(new { error = "Invalid source or pagination details." });
                return;
            }

            if (context.ActionArguments.TryGetValue("transactionType", out var transactionTypeObj) && transactionTypeObj is TransactionName transactionType &&
            context.ActionArguments.TryGetValue("pageDetails", out var pageDetailsObjofTransactionType) && pageDetailsObjofTransactionType is PaginationInput pageDetailsOfTransactionType)
            {
                var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                _logger.LogInformation("User  role: {User  Role}", userRole);
                ICollection<TransactionDTO> transactions;

                if (userRole == "Admin")
                {
                    transactions = await _transactionServices.GetTransactionsByTypeAsync(transactionType, pageDetailsOfTransactionType);
                }
                else
                {
                    transactions = await _transactionServices.GetTransactionsByTypeForCurrentUserAsync(transactionType, pageDetails);
                }

                context.HttpContext.Items["TransactionsByType"] = transactions;
                _logger.LogInformation("Transactions retrieved successfully for role: {User  Role}", userRole);
            }

            await next();
        }
    }
}