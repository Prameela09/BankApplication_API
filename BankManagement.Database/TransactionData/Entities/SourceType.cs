using System;
using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;

namespace BankManagement.Database.TransactionData.Entities;

public class SourceType
    {
        [Key]
        public int SourceTypeId { get; set; }

        [Required]
        public TransactionSource SourceName { get; set; }
    }
