using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Database.BranchData.DTOs
{
    public class BranchUpdationDTO
    {
        public string BranchName { get; set; }

        public Location? Location { get; set; }

    }
}