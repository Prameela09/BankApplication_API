using System;
using BankManagement.Utilities.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.BranchServices.DTOs;

public class BranchDTO
{
    [SwaggerIgnore]
    public int BranchId { get; set; }
    public string BranchName { get; set; }

    [SwaggerIgnore]
    public string IFSCCode { get; set; }

    [SwaggerIgnore]
    public string BranchCode { get; set; }

    public Location Location { get; set; }
}
