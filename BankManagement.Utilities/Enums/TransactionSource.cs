using System.Text.Json.Serialization;

namespace BankManagement.Utilities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionSource
{
    CreditCard = 1,
    DebitCard = 2,
    Cash = 3,
    PhonePe = 4,
    GooglePay = 5,
    Paytm = 6,
    Check = 7,
    Other = 8 
}
