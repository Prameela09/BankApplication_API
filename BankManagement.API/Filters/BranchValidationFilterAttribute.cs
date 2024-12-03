// using BankManagement.Database.BranchData.DTOs;
// using BankManagement.Services.BranchServices.DTOs;
// using BankManagement.Services.BranchServices.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;

// namespace BankManagement.API.Filters;

// public class BranchValidationFilterAttribute : IAsyncActionFilter
// {
//     private readonly IBranchServices _branchServices;
//     private readonly ILogger<BranchValidationFilterAttribute> _logger;

//     public BranchValidationFilterAttribute(IBranchServices branchServices, ILogger<BranchValidationFilterAttribute> logger)
//     {
//         _branchServices = branchServices;
//         _logger = logger;
//     }

//     public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         if (context.ActionDescriptor.RouteValues["action"] == "CreateBranch")
//         {
//             var branchDto = context.ActionArguments["branchDto"] as BranchDTO;
//             if (branchDto == null)
//             {
//                 _logger.LogWarning("Branch creation failed: Branch details cannot be null.");
//                 context.Result = new BadRequestObjectResult("Branch details cannot be null.");
//                 return;
//             }
//         }

//         if (context.ActionDescriptor.RouteValues["action"] == "GetBranchDetailsById")
//         {
//             if (context.ActionArguments.TryGetValue("branchId", out var idObj) && idObj is int branchId)
//             {
//                 var branch = await _branchServices.GetBranchDetailsByIdAsync(branchId);
//                 if (branch == null)
//                 {
//                     _logger.LogWarning("Branch not found: {BranchId}", branchId);
//                     context.Result = new NotFoundObjectResult("Branch not found.");
//                     return;
//                 }
//             }
//         }

//         if (context.ActionDescriptor.RouteValues["action"] == "GetBranchDetailsByCode")
//         {
//             if (context.ActionArguments.TryGetValue("branchCode", out var codeObj) && codeObj is string branchCode)
//             {
//                 var branch = await _branchServices.GetBranchByCodeAsync(branchCode);
//                 if (branch == null)
//                 {
//                     _logger.LogWarning("Branch not found for code: {BranchCode}", branchCode);
//                     context.Result = new NotFoundObjectResult("Branch not found.");
//                     return;
//                 }
//             }
//         }

//         if (context.ActionDescriptor.RouteValues["action"] == "UpdateBranchDetails")
//         {
//             if (context.ActionArguments.TryGetValue("branchId", out var idObj) && idObj is int branchId)
//             {
//                 var branchUpdationDto = context.ActionArguments["branchUpdationDto"] as BranchUpdationDTO;
//                 if (branchUpdationDto == null)
//                 {
//                     _logger.LogWarning("Update attempt failed: Branch details cannot be null.");
//                     context.Result = new BadRequestObjectResult("Branch details cannot be null.");
//                     return;
//                 }

//                 var branch = await _branchServices.GetBranchDetailsByIdAsync(branchId);
//                 if (branch == null)
//                 {
//                     _logger.LogWarning("Update attempt failed: Branch with ID {BranchId} not found.", branchId);
//                     context.Result = new NotFoundObjectResult($"Branch with ID {branchId} not found.");
//                     return;
//                 }
//             }
//         }

//         if (context.ActionDescriptor.RouteValues["action"] == "DeactivateBranch")
//         {
//             if (context.ActionArguments.TryGetValue("branchId", out var idObj) && idObj is int branchId)
//             {
//                 var branch = await _branchServices.GetBranchDetailsByIdAsync(branchId);
//                 if (branch == null)
//                 {
//                     _logger.LogWarning("Deactivation attempt failed: Branch with ID {BranchId} not found.", branchId);
//                     context.Result = new NotFoundObjectResult($"Branch with ID {branchId} not found.");
//                     return;
//                 }
//             }
//         }

//         if (context.ActionDescriptor.RouteValues["action"] == "RecoverBranch")
//         {
//             var branchRecoveryDetails = context.ActionArguments["branchRecoveryDetails"] as BranchDTO;
//             if (branchRecoveryDetails == null)
//             {
//                 _logger.LogWarning("Recovery attempt failed: Branch recovery details cannot be null.");
//                 context.Result = new BadRequestObjectResult("Branch recovery details cannot be null.");
//                 return;
//             }


//             var branch = await _branchServices.GetBranchByCodeAsync(branchRecoveryDetails.BranchCode);
//             if (branch == null)
//             {
//                 _logger.LogWarning("Recovery attempt failed: Branch with ID {BranchId} not found.", branchRecoveryDetails.BranchName);
//                 context.Result = new NotFoundObjectResult($"Branch with ID {branchRecoveryDetails.BranchName} not found.");
//                 return;
//             }
//         }

//         await next();
//     }
// }



using BankManagement.Database.BranchData.DTOs;
using BankManagement.Services.BranchServices.DTOs;
using BankManagement.Services.BranchServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace BankManagement.API.Filters
{
    public class BranchValidationFilterAttribute : IAsyncActionFilter
    {
        private readonly IBranchServices _branchServices;
        private readonly ILogger<BranchValidationFilterAttribute> _logger;

        public BranchValidationFilterAttribute(IBranchServices branchServices, ILogger<BranchValidationFilterAttribute> logger)
        {
            _branchServices = branchServices;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionName = context.ActionDescriptor.RouteValues["action"];

            switch (actionName)
            {
                case "CreateBranch":
                    ValidateCreateBranch(context);
                    break;

                case "GetBranchDetailsById":
                    await ValidateGetBranchDetailsById(context);
                    break;

                case "GetBranchDetailsByCode":
                    await ValidateGetBranchDetailsByCode(context);
                    break;

                case "UpdateBranchDetails":
                    await ValidateUpdateBranchDetails(context);
                    break;

                case "DeactivateBranch":
                    await ValidateDeactivateBranch(context);
                    break;

                case "RecoverBranch":
                    ValidateRecoverBranch(context);
                    break;

                default:
                    break;
            }
            if (context.Result == null)
            {
                await next();
            }
        }

        private void ValidateCreateBranch(ActionExecutingContext context)
        {
            if (context.ActionArguments["branchDto"] is not BranchDTO branchDto || branchDto == null)
            {
                _logger.LogWarning("Branch creation failed: Branch details cannot be null.");
                context.Result = new BadRequestObjectResult("Branch details cannot be null.");
            }
        }

        private async Task ValidateGetBranchDetailsById(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("branchId", out var idObj) && idObj is int branchId)
            {
                var branch = await _branchServices.GetBranchDetailsByIdAsync(branchId);
                if (branch == null)
                {
                    _logger.LogWarning("Branch not found: {BranchId}", branchId);
                    context.Result = new NotFoundObjectResult("Branch not found.");
                }
            }
        }

        private async Task ValidateGetBranchDetailsByCode(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("branchCode", out var codeObj) && codeObj is string branchCode)
            {
                var branch = await _branchServices.GetBranchByCodeAsync(branchCode);
                if (branch == null)
                {
                    _logger.LogWarning("Branch not found for code: {BranchCode}", branchCode);
                    context.Result = new NotFoundObjectResult("Branch not found.");
                }
            }
        }

        private async Task ValidateUpdateBranchDetails(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("branchId", out var idObj) && idObj is int branchId)
            {
                if (context.ActionArguments["branchUpdationDto"] is not BranchUpdationDTO branchUpdationDto || branchUpdationDto == null)
                {
                    _logger.LogWarning("Update attempt failed: Branch details cannot be null.");
                    context.Result = new BadRequestObjectResult("Branch details cannot be null.");
                    return;
                }

                var branch = await _branchServices.GetBranchDetailsByIdAsync(branchId);
                if (branch == null)
                {
                    _logger.LogWarning("Update attempt failed: Branch with ID {BranchId} not found.", branchId);
                    context.Result = new NotFoundObjectResult($"Branch with ID {branchId} not found.");
                }
            }
        }

        private async Task ValidateDeactivateBranch(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("branchId", out var idObj) && idObj is int branchId)
            {
                var branch = await _branchServices.GetBranchDetailsByIdAsync(branchId);
                if (branch == null)
                {
                    _logger.LogWarning("Deactivation attempt failed: Branch with ID {BranchId} not found.", branchId);
                    context.Result = new NotFoundObjectResult($"Branch with ID {branchId} not found.");
                }
            }
        }

        private void ValidateRecoverBranch(ActionExecutingContext context)
        {
            if (context.ActionArguments["branchRecoveryDetails"] is not BranchDTO branchRecoveryDetails || branchRecoveryDetails == null)
            {
                _logger.LogWarning("Recovery attempt failed: Branch recovery details cannot be null.");
                context.Result = new BadRequestObjectResult("Branch recovery details cannot be null.");
                return;
            }
        }
    }
}