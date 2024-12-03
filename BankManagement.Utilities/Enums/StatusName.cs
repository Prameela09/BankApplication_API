using System.Text.Json.Serialization;

namespace BankManagement.Utilities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusName
{
    Successful = 1,
    Pending = 2, 
    Fail = 3
}
