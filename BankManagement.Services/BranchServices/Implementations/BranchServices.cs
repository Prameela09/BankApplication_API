using System.Linq.Expressions;
using AutoMapper;
using BankManagement.Database.BranchData.DTOs;
using BankManagement.Database.BranchData.Entities;
using BankManagement.Database.BranchData.Interfaces;
using BankManagement.Services.BranchServices.DTOs;
using BankManagement.Services.BranchServices.Interfaces;
using BankManagement.Utilities.Enums;
using BankManagement.Utilities.HelperClasses;

namespace BankManagement.Services.BranchServices.Implementations
{
    public class BranchServices : IBranchServices
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;

        public BranchServices(IBranchRepository branchRepository, IMapper mapper)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
        }

        public async Task<List<BranchDTO>> GetAllActiveBranchesAsync(PaginationInput pageDetails)
        {
            var branches = await _branchRepository.GetAllActiveBranchesAsync();
            return RetrieveBranchesPerPage(pageDetails, _mapper.Map<List<BranchDTO>>(branches));
        }

        public async Task<BranchDTO> GetBranchDetailsByIdAsync(int branchId)
        {
            return await GetBranchAsync(b => b.BranchId == branchId && b.IsActive == true);
        }

        public async Task<BranchDTO> GetBranchByCodeAsync(string branchCode)
        {
            return await GetBranchAsync(b => b.BranchCode == branchCode && b.IsActive == true);
        }

        public async Task<ICollection<BranchDTO>> GetBranchByLocationAsync(Location location)
        {
            var sameLocationBranches = await _branchRepository.FindBranchesByLocationAsync(location);
            return _mapper.Map<List<BranchDTO>>(sameLocationBranches);
        }

        public async Task<List<BranchDTO>> GetAllInactiveBranchesAsync(PaginationInput pageDetails)
        {
            var branches = await _branchRepository.GetAllInactiveBranchesAsync();
            return RetrieveBranchesPerPage(pageDetails, _mapper.Map<List<BranchDTO>>(branches));
        }

        public async Task<BranchDTO> CreateBranchAsync(BranchDTO branchDto)
        {
            var branch = _mapper.Map<Branch>(branchDto);
            SetBranchDefaults(branch);
            await _branchRepository.CreateBranchAsync(branch);
            return _mapper.Map<BranchDTO>(branch);
        }

        public async Task<bool> UpdateBranchAsync(int branchId, BranchUpdationDTO branchUpdationDto)
        {
            var existingBranch = await _branchRepository.FindBranchAsync(b => b.BranchId == branchId);
            if (existingBranch == null) return false;

            existingBranch.BranchName = branchUpdationDto.BranchName ?? existingBranch.BranchName;
            existingBranch.Location = branchUpdationDto.Location ?? existingBranch.Location;
            
            return await _branchRepository.UpdateBranchAsync(existingBranch);
        }

        public async Task<bool> DeactivateBranchAsync(int branchId)
        {
            var branchToDeactivate = await _branchRepository.FindBranchAsync(b => b.BranchId == branchId);
            if (branchToDeactivate == null) return false;

            branchToDeactivate.IsActive = false;
            var mainBranch = await _branchRepository.FindBranchAsync(b => b.BranchId == 2);
            var accounts = branchToDeactivate.Accounts;

            foreach (var account in accounts)
            {
                account.Branch = mainBranch;
            }

            await _branchRepository.UpdateBranchAsync(branchToDeactivate);
            return true;
        }

        public async Task<bool> RecoverBranchAsync(BranchDTO branchRecoveryDetails)
        {
            var branchToRecover = await _branchRepository.FindBranchAsync(b =>
                b.BranchName == branchRecoveryDetails.BranchName &&
                b.BranchCode == branchRecoveryDetails.BranchCode &&
                b.IFSCCode == branchRecoveryDetails.IFSCCode &&
                b.Location == branchRecoveryDetails.Location);

            if (branchToRecover == null) return false;

            branchToRecover.IsActive = true;
            await _branchRepository.UpdateBranchAsync(branchToRecover);
            return true;
        }

        private async Task<BranchDTO> GetBranchAsync(Expression<Func<Branch, bool>> branchDetails)
        {
            var branch = await _branchRepository.FindBranchAsync(branchDetails);
            return _mapper.Map<BranchDTO>(branch);
        }

        private void SetBranchDefaults(Branch branch)
        {
            branch.CreatedAt = DateTime.UtcNow;
            branch.ModifiedAt = DateTime.UtcNow;
            branch.IsActive = true;
            branch.BranchCode = BranchHelper.GenerateBranchCode();
            branch.IFSCCode = BranchHelper.GenerateIFSCCode(branch.BranchCode);
        }

        private List<BranchDTO> RetrieveBranchesPerPage(PaginationInput pageDetails, List<BranchDTO> branches)
        {
            return branches
                .Skip((pageDetails.PageNumber - 1) * pageDetails.NumOfEntities)
                .Take(pageDetails.NumOfEntities)
                .ToList();
        }
    }
}