using System.Linq.Expressions;
using BankManagement.Database.BranchData.Entities;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.BranchData.Interfaces
{
    public interface IBranchRepository
    {
        /// <summary>
        /// Finds a branch based on the provided branch details.
        /// Returns the first matching branch or null if no match is found.
        /// </summary>
        Task<Branch> FindBranchAsync(Expression<Func<Branch, bool>> branchDetails);

        /// <summary>
        /// Creates a new branch using the provided branch details.
        /// Returns a boolean indicating whether the branch creation was successful.
        /// </summary>
        Task<bool> CreateBranchAsync(Branch branch);

        /// <summary>
        /// Retrieves all active branches from the database.
        /// </summary>
        Task<List<Branch>> GetAllActiveBranchesAsync();

        /// <summary>
        /// Retrieves all de-activated branches from the database.
        /// </summary>
        Task<List<Branch>> GetAllInactiveBranchesAsync();
        
        /// <summary>
        /// Updates the details of an existing branch.
        /// Returns a boolean indicating whether the update was successful.
        /// </summary>
        Task<bool> UpdateBranchAsync(Branch branch);

        /// <summary>
        /// Finds all branches that match the specified location.
        /// Returns a list of branches located at the specified location.
        /// </summary>
        Task<List<Branch>> FindBranchesByLocationAsync(Location location);
    }
}