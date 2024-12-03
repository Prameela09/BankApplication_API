using System.Text.Json.Serialization;

namespace BankManagement.Utilities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountName 
{
    Savings = 1,
    Current = 2
}
