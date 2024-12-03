using System.ComponentModel.DataAnnotations;
using BankManagement.Utilities.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace BankManagement.Services.TransactionServices.DTOs
{
    public class TransactionDTO : IValidatableObject
    {
        [SwaggerIgnore]
        public int TransactionId { get; set; }

        [Required]
        public long AccountNumber { get; set; }

        public long? TargetAccountNumber { get; set; }

        [Required]
        public TransactionName TransactionType { get; set; }

        [Required]
        public TransactionSource SourceName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [SwaggerIgnore]
        public DateTime DateOfTransaction { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string Description { get; set; }

        [SwaggerIgnore]
        public int StatusTypeId { get; set; }

        [SwaggerIgnore]
        public int SourceTypeId { get; set; }

        [SwaggerIgnore]
        public int TransactionTypeId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TransactionType == TransactionName.Transfer)
            {
                if (AccountNumber <= 0)
                {
                    yield return new ValidationResult("AccountNumber (source account) is required for fund transfers.", new[] { nameof(AccountNumber) });
                }
                if (TargetAccountNumber <= 0)
                {
                    yield return new ValidationResult("TargetAccountNumber is required for fund transfers.", new[] { nameof(TargetAccountNumber) });
                }
            }
            else
            {
                if (TargetAccountNumber > 0)
                {
                    yield return new ValidationResult("TargetAccountNumber should not be provided for non-fund transfer transactions.", new[] { nameof(TargetAccountNumber) });
                }
            }
        }
    }
}