using System;
using System.ComponentModel;

namespace BankManagement.Utilities.HelperClasses;

public class PaginationInput
{
    [DefaultValue(1)]
    public int PageNumber { get; set; }

    [DefaultValue(5)]
    public int NumOfEntities { get; set; }
}
