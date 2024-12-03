using System.Text.Json.Serialization;

namespace BankManagement.Utilities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionName
{
    Deposit = 1,
    WithDraw = 2,
    OnlinePurchase = 3,     
    Transfer = 4, 
    Subscription = 5,

}
