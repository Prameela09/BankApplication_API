using System;
using System.Linq.Expressions;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.BranchData.Entities;
using BankManagement.Database.BranchData.Interfaces;
using BankManagement.Database.DbContexts;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.BranchData.Implementations;

public class BranchRepository : IBranchRepository
{
    private readonly BankDataContext _branchContext;

    public BranchRepository(BankDataContext branchContext)
    {
        _branchContext = branchContext;
    }

    public async Task<Branch> FindBranchAsync(Expression<Func<Branch, bool>> branchDetails)
    {
        return await GetBranchesWithIncludes().FirstOrDefaultAsync(branchDetails);
    }

    public async Task<bool> CreateBranchAsync(Branch branch)
    {
        _branchContext.Branches.Add(branch);
        return await _branchContext.SaveChangesAsync() > 0;
    }

    public async Task<List<Branch>> GetAllActiveBranchesAsync()
    {
        return await _branchContext.Branches.Where(b => b.IsActive == true).ToListAsync();
    }

    public async Task<List<Branch>> GetAllInactiveBranchesAsync()
    {
        return await _branchContext.Branches.Where(b => b.IsActive == false).ToListAsync();
    }
    
    public async Task<bool> UpdateBranchAsync(Branch branch)
    {
        _branchContext.Branches.Update(branch);
        return await _branchContext.SaveChangesAsync() > 0;
    }

    public async Task<List<Branch>> FindBranchesByLocationAsync(Location location)
    {
        return await _branchContext.Branches
            .Where(b => b.Location == location)
            .ToListAsync();
    }
    
    private IQueryable<Branch> GetBranchesWithIncludes()
    {
        return _branchContext.Branches.Include(b => b.Accounts);
    }
}

