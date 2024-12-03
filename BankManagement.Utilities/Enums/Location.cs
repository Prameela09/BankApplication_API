using System.Text.Json.Serialization;

namespace BankManagement.Utilities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Location
{
    Visakhapatnam = 1,
    Vizianagaram = 2,
    Hyderabad = 3,
    Rajamandri = 4
}
