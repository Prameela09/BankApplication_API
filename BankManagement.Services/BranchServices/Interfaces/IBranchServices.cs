using BankManagement.Database.BranchData.DTOs;
using BankManagement.Services.BranchServices.DTOs;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;

namespace BankManagement.Services.BranchServices.Interfaces
{
    public interface IBranchServices
    {
        /// <summary>
        /// Adds a new branch using the provided branch details.
        /// Returns the created branch details.
        /// </summary>
        Task<BranchDTO> CreateBranchAsync(BranchDTO branchDto);

        /// <summary>
        /// Updates the details of an existing branch identified by its ID.
        /// Returns a boolean indicating whether the update was successful.
        /// </summary>
        Task<bool> UpdateBranchAsync(int branchId, BranchUpdationDTO branchUpdationDto);

        /// <summary>
        /// Deactivates a branch identified by its ID.
        /// Returns a boolean indicating whether the deactivation was successful.
        /// </summary>
        Task<bool> DeactivateBranchAsync(int branchId);

        /// <summary>
        /// Recovers a previously deactivated branch using the provided branch details.
        /// Returns a boolean indicating whether the recovery was successful.
        /// </summary>
        Task<bool> RecoverBranchAsync(BranchDTO branchRecoveryDetails);

        /// <summary>
        /// Retrieves the details of a branch identified by its ID.
        /// Returns the branch details or null if not found.
        /// </summary>
        Task<BranchDTO> GetBranchDetailsByIdAsync(int branchId);

        /// <summary>
        /// Retrieves a branch using its unique code.
        /// Returns the branch details or null if not found.
        /// </summary>
        Task<BranchDTO> GetBranchByCodeAsync(string branchCode);

        /// <summary>
        /// Retrieves all active branches in the bank.
        /// </summary>
        Task<List<BranchDTO>> GetAllActiveBranchesAsync(PaginationInput pageDetails);

        /// <summary>
        /// Retrieves all de-activated branches in the bank.
        /// </summary>
        Task<List<BranchDTO>> GetAllInactiveBranchesAsync(PaginationInput pageDetails);
        
        /// <summary>
        /// Retrieves branches located at the specified location.
        /// Returns a list of branch details located at the specified location.
        /// </summary>
        Task<ICollection<BranchDTO>> GetBranchByLocationAsync(Location location);
    }
}