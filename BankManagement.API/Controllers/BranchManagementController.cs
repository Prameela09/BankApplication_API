using BankManagement.API.Filters;
using BankManagement.Database.BranchData.DTOs;
using BankManagement.Services.BranchServices.DTOs;
using BankManagement.Services.BranchServices.Interfaces;
using BankManagement.Utilities.ExceptionHandlers;
using BankManagement.Utilities.HelperClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    // [GlobalExceptionHandler]
    [ServiceFilter(typeof(BranchValidationFilterAttribute))]
    public class BranchManagementController : ControllerBase
    {
        private readonly IBranchServices _branchServices;
        private readonly ILogger<BranchManagementController> _logger;

        public BranchManagementController(IBranchServices branchServices, ILogger<BranchManagementController> logger)
        {
            _branchServices = branchServices;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BranchDTO>> CreateBranch([FromQuery] BranchDTO branchDto)
        {
            _logger.LogInformation("Creating branch with details: {@BranchDto}", branchDto);
            BranchDTO createdBranch = await _branchServices.CreateBranchAsync(branchDto);

            _logger.LogInformation("Branch created successfully: {BranchName}", createdBranch.BranchName);
            return Ok(createdBranch);
        }

        [HttpGet("{branchId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BranchDTO>> GetBranchDetailsById(int branchId)
        {
            _logger.LogInformation("Retrieving details for branch ID: {BranchId}", branchId);
            BranchDTO branch = await _branchServices.GetBranchDetailsByIdAsync(branchId);
            
            _logger.LogInformation("Branch details retrieved successfully: {@Branch}", branch);
            return Ok(branch);
        }

        [HttpPut("{branchId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBranchDetails(int branchId, [FromQuery] BranchUpdationDTO branchUpdationDto)
        {
            _logger.LogInformation("Attempting to update branch with ID: {BranchId} with details: {@BranchDetails}", branchId, branchUpdationDto);
            await _branchServices.UpdateBranchAsync(branchId, branchUpdationDto);

            _logger.LogInformation("Branch with ID {BranchId} updated successfully.", branchId);
            return Ok("Branch updated successfully!");
        }

        [HttpPost("{branchId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateBranch(int branchId)
        {
            _logger.LogInformation("Attempting to deactivate branch with ID: {BranchId}", branchId);
            await _branchServices.DeactivateBranchAsync(branchId);

            _logger.LogInformation("Branch with ID {BranchId} deactivated successfully.", branchId);
            return Ok("Branch deactivated successfully.");

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RecoverBranch([FromBody] BranchDTO branchRecoveryDetails)
        {
            await _branchServices.RecoverBranchAsync(branchRecoveryDetails);

            _logger.LogInformation("Branch with ID {BranchId} has been successfully recovered.", branchRecoveryDetails.BranchName);
            return Ok($"Branch with ID {branchRecoveryDetails.BranchName} has been successfully recovered.");
        }

        [HttpGet("inactive")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<BranchDTO>>> GetAllInactiveBranches([FromQuery] PaginationInput pageDetails)
        {
            _logger.LogInformation("Admin retrieving all inactive branches with pagination: {@PageDetails}", pageDetails);
            List<BranchDTO> branches = await _branchServices.GetAllInactiveBranchesAsync(pageDetails);

            _logger.LogInformation("Retrieved {Count} inactive branches successfully.", branches.Count);
            return Ok(branches);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<BranchDTO>>> GetAllBranchesInformation([FromQuery] PaginationInput pageDetails)
        {
            _logger.LogInformation("Retrieving all branches with pagination: {@PageDetails}", pageDetails);
            List<BranchDTO> branches = await _branchServices.GetAllActiveBranchesAsync(pageDetails);

            _logger.LogInformation("Retrieved {Count} branches successfully.", branches.Count);
            return Ok(branches);
        }

        [HttpGet("{branchCode}")]
        [Authorize]
        public async Task<ActionResult<BranchDTO>> GetBranchDetailsByCode(string branchCode)
        {
            _logger.LogInformation("Retrieving branch with code: {BranchCode}", branchCode);
            BranchDTO branch = await _branchServices.GetBranchByCodeAsync(branchCode);

            _logger.LogInformation("Branch details retrieved successfully for code: {BranchCode}", branchCode);
            return Ok(branch);
        }
    }
}