// using BankManagement.Services.TransactionServices.DTOs;
// using BankManagement.Services.TransactionServices.Interfaces;
// using BankManagement.Utilities.Enums;
// using BankManagement.Utilities.HelperClasses;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using AutoMapper;
// using BankManagement.Utilities.ExceptionHandlers;

// namespace BankManagement.API.Controllers;

// [ApiController]
// [Route("api/[controller]/[action]")]
// [GlobalExceptionHandler]

// public class TransactionManagementController : ControllerBase
// {
//     private readonly ITransactionServices _transactionServices;
//     private readonly ILogger<TransactionManagementController> _logger;
//     private readonly IMapper _mapper;

//     public TransactionManagementController(ITransactionServices transactionServices, ILogger<TransactionManagementController> logger, IMapper mapper)
//     {
//         _transactionServices = transactionServices;
//         _logger = logger;
//         _mapper = mapper;
//     }

//     [HttpPost]
//     [Authorize]
//     public async Task<ActionResult<TransactionResultDTO>> PerformTransaction([FromQuery] TransactionDTO transactionDto)
//     {
//         _logger.LogInformation("Performing transaction with details: {@TransactionDto}", transactionDto);
//         TransactionDTO result = await _transactionServices.PerformTransactionAsync(transactionDto);

//         _logger.LogInformation("Transaction performed successfully: {TransactionId}", result.TransactionId);
//         TransactionResultDTO transactionResult = _mapper.Map<TransactionResultDTO>(result);
//         return Ok(transactionResult);
//     }

//     [HttpGet("{transactionId}")]
//     [Authorize(Roles = "Admin")]
//     public async Task<ActionResult<TransactionResultDTO>> GetTransactionDeatailsById(int transactionId)
//     {
//         _logger.LogInformation("Retrieving transaction by ID: {TransactionId}", transactionId);
//         TransactionDTO transaction = await _transactionServices.GetTransactionByIdAsync(transactionId);
//         if (transaction == null)
//         {
//             _logger.LogWarning("Transaction not found: {TransactionId}", transactionId);
//             return NotFound("Transaction not found.");
//         }
//         _logger.LogInformation("Transaction retrieved successfully: {@Transaction}", transaction);
//         TransactionResultDTO transactionResult = _mapper.Map<TransactionResultDTO>(transaction);
//         return Ok(transactionResult);
//     }

//     [HttpGet("{accountNumber}")]
//     [Authorize]
//     public async Task<ActionResult<ICollection<TransactionResultDTO>>> GetTransactionsHistoryByAccountNumber(long accountNumber, [FromQuery] PaginationInput pageDetails)
//     {
//         _logger.LogInformation("Retrieving transactions for account number: {AccountNumber}", accountNumber);
//         ICollection<TransactionDTO> transactions = await _transactionServices.GetTransactionsByAccountNumberAsync(accountNumber, pageDetails);

//         ICollection<TransactionResultDTO> transactionResults = _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
//         _logger.LogInformation("Retrieved {Count} transactions for account number: {AccountNumber}", transactionResults.Count, accountNumber);
//         return Ok(transactionResults);
//     }

//     [HttpGet("{transactionType}")]
//     [Authorize]
//     public async Task<ActionResult<ICollection<TransactionResultDTO>>> GetTransactionsByTransactionType(TransactionName transactionType, [FromQuery] PaginationInput pageDetails)
//     {
//         _logger.LogInformation("Retrieving transactions of type: {TransactionType}", transactionType);
//         ICollection<TransactionDTO> transactions = await _transactionServices.GetTransactionsByTypeAsync(transactionType, pageDetails);

//         ICollection<TransactionResultDTO> transactionResults = _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
//         _logger.LogInformation("Retrieved {Count} transactions of type: {TransactionType}", transactionResults.Count, transactionType);
//         return Ok(transactionResults);
//     }

//     [HttpGet("{source}")]
//     [Authorize]
//     public async Task<ActionResult<ICollection<TransactionResultDTO>>> GetTransactionsBySource(TransactionSource source, [FromQuery] PaginationInput pageDetails)
//     {
//         _logger.LogInformation("Retrieving transactions from source: {Source}", source);
//         ICollection<TransactionDTO> transactions = await _transactionServices.GetTransactionsBySourceAsync(source, pageDetails);

//         ICollection<TransactionResultDTO> transactionResults = _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
//         _logger.LogInformation("Retrieved {Count} transactions from source: {Source}", transactionResults.Count, source);
//         return Ok(transactionResults);
//     }

//     [HttpGet("{status}")]
//     [Authorize]
//     public async Task<ActionResult<ICollection<TransactionDTO>>> GetTransactionsByStatus(StatusName status)
//     {
//         _logger.LogInformation("Getting transactions for status: {Status}", status);
//         ICollection<TransactionResultDTO> transactions = await _transactionServices.GetTransactionsByStatusAsync(status);
//         if (transactions == null || transactions.Count == 0)
//         {
//             _logger.LogWarning("No transactions found for status: {Status}", status);
//             return NotFound($"No transactions found for status '{status}'.");
//         }
//         _logger.LogInformation("Found {Count} transactions for status: {Status}", transactions.Count, status);
//         return Ok(transactions);
//     }
//     [HttpPost]
//     [Authorize]
//     public async Task<ActionResult<TransactionResultDTO>> TransferFunds([FromQuery] FundTransferRequestDTO fundTransferRequest)
//     {
//         _logger.LogInformation("Transferring funds with details: {@FundTransferRequest}", fundTransferRequest);
//         string result = await _transactionServices.TransferFundsAsync(fundTransferRequest);

//         _logger.LogInformation("Funds transferred successfully!");
//         return Ok(result);
//     }

//     [HttpGet("{username}")]
//     [Authorize(Roles = "Admin")]
//     public async Task<ActionResult<List<TransactionResultDTO>>> GetTransactionHistoryByUsername(string username, [FromQuery] PaginationInput pageDetails)
//     {
//         _logger.LogInformation("Retrieving transaction history for username: {Username}", username);
//         List<TransactionDTO> transactions = await _transactionServices.GetTransactionHistoryByUsernameAsync(username, pageDetails);

//         List<TransactionResultDTO> transactionResults = _mapper.Map<List<TransactionResultDTO>>(transactions);
//         _logger.LogInformation("Retrieved {Count} transactions for username: {Username}", transactionResults.Count, username);
//         return Ok(transactionResults);
//     }

//     [HttpGet]
//     [Authorize]
//     public async Task<ActionResult<List<TransactionResultDTO>>> GetMyTransactionHistory([FromQuery] PaginationInput pageDetails)
//     {
//         _logger.LogInformation("Retrieving transaction history for the current user.");
//         List<TransactionDTO> transactions = await _transactionServices.GetTransactionHistoryForCurrentUserAsync(pageDetails);

//         List<TransactionResultDTO> transactionResults = _mapper.Map<List<TransactionResultDTO>>(transactions);
//         _logger.LogInformation("Retrieved {Count} transactions for the current user.", transactionResults.Count);
//         return Ok(transactionResults);
//     }
// }


using BankManagement.Services.TransactionServices.DTOs;
using BankManagement.Services.TransactionServices.Interfaces;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BankManagement.Utilities.ExceptionHandlers;
using BankManagement.API.Filters;

namespace BankManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [GlobalExceptionHandler]
    [ServiceFilter(typeof(AccountValidationFilterAttribute))]
    [ServiceFilter(typeof(UserValidationFilterAttribute))]
    [ServiceFilter(typeof(TransactionValidationFilterAttribute))]
    public class TransactionManagementController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;
        private readonly ILogger<TransactionManagementController> _logger;
        private readonly IMapper _mapper;

        public TransactionManagementController(ITransactionServices transactionServices, ILogger<TransactionManagementController> logger, IMapper mapper)
        {
            _transactionServices = transactionServices;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<TransactionResultDTO>> PerformTransaction([FromBody] TransactionDTO transactionDto)
        {
            _logger.LogInformation("Performing transaction with details: {@TransactionDto}", transactionDto);
            TransactionDTO result = await _transactionServices.PerformTransactionAsync(transactionDto);

            _logger.LogInformation("Transaction performed successfully: {TransactionId}", result.TransactionId);
            TransactionResultDTO transactionResult = _mapper.Map<TransactionResultDTO>(result);
            return Ok(transactionResult);
        }

        [HttpGet("{accountNumber}")]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<ICollection<TransactionResultDTO>>> GetTransactionByAccountNumber(long accountNumber, [FromQuery] PaginationInput pageDetails)
        {
            ICollection<TransactionDTO> transactions = await _transactionServices.GetTransactionsByAccountNumberAsync(accountNumber, pageDetails);
            var transactionResults = _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
            return Ok(transactionResults);
        }


        [HttpGet("transactionId")]
        [Authorize(Roles = "Admin,Customer")]
        public ActionResult<TransactionResultDTO> GetTransactionById(int transactionId)
        {
            var transaction = HttpContext.Items["Transaction"] as TransactionDTO;
            TransactionResultDTO transactionResult = _mapper.Map<TransactionResultDTO>(transaction);
            return Ok(transactionResult);
        }

        [HttpGet("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<TransactionResultDTO>>> GetTransactionHistoryByUsername(string username, [FromQuery] PaginationInput pageDetails)
        {
            _logger.LogInformation("Retrieving transaction history for username: {Username}", username);
            List<TransactionDTO> transactions = await _transactionServices.GetTransactionHistoryByUsernameAsync(username, pageDetails);

            List<TransactionResultDTO> transactionResults = _mapper.Map<List<TransactionResultDTO>>(transactions);
            _logger.LogInformation("Retrieved {Count} transactions for username: {Username}", transactionResults.Count, username);
            return Ok(transactionResults);
        }

        [HttpGet("{source}")]
        [Authorize(Roles = "Customer, Admin")]
        public ActionResult<ICollection<TransactionResultDTO>> GetTransactionHistoryBySource(TransactionSource source, [FromQuery] PaginationInput pageDetails)
        {
            var transactions = HttpContext.Items["Transactions"] as ICollection<TransactionDTO>;
            var transactionResults = _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
            return Ok(transactionResults);
        }

        [HttpGet("{transactionType}")]
        [Authorize(Roles = "Customer, Admin")]
        public ActionResult<ICollection<TransactionResultDTO>> GetTransactionHistoryByTransactionType(TransactionName transactionType, [FromQuery] PaginationInput pageDetails)
        {
            var transactions = HttpContext.Items["Transactions"] as ICollection<TransactionDTO>;
            var transactionResults = _mapper.Map<ICollection<TransactionResultDTO>>(transactions);
            return Ok(transactionResults);
        }

        [HttpGet]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<List<TransactionResultDTO>>> GetMyTransactionHistory([FromQuery] PaginationInput pageDetails)
        {
            _logger.LogInformation("Retrieving transaction history for the current user.");
            List<TransactionDTO> transactions = await _transactionServices.GetTransactionHistoryForCurrentUserAsync(pageDetails);

            List<TransactionResultDTO> transactionResults = _mapper.Map<List<TransactionResultDTO>>(transactions);
            _logger.LogInformation("Retrieved {Count} transactions for the current user.", transactionResults.Count);
            return Ok(transactionResults);
        }
    }
}