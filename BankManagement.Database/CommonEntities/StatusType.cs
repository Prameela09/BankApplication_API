using System;
using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.CommonEntities;

public class StatusType
    {
        [Key]
        public int StatusTypeId { get; set; }

        [Required]
        public StatusName Status { get; set; }
    }
