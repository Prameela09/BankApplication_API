using BankManagement.Services.TransactionServices.DTOs;

namespace BankManagement.Services.AccountServices.DTOs;

public class AccountOverviewDTO
{

    public List<AccountResultDTO> Accounts { get; set; }
    
    public Dictionary<long, List<TransactionResultDTO>> AccountTransactions { get; set; }

}