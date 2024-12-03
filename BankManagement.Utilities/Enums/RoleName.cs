using System.Text.Json.Serialization;

namespace BankManagement.Utilities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleName
{
    Admin = 1,
    Customer = 2
    
}
