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
