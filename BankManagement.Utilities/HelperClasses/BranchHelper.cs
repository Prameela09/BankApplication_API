using System;

namespace BankManagement.Utilities.HelperClasses;

public class BranchHelper
{
    private static int _branchCounter = 1;
    public static string GenerateBranchCode()
    {
        string branchCode = $"BRN{_branchCounter:D3}";
        _branchCounter++;
        return branchCode;
    }

    public static string GenerateIFSCCode(string branchCode)
    {
        const string bankCode = "ABCD";

        if (string.IsNullOrEmpty(branchCode) || branchCode.Length != 6)
        {
            throw new ArgumentException("Branch code must be 6 characters long.");
        }

        string ifscCode = $"{bankCode}{branchCode.ToUpper()}";
        return ifscCode;
    }
}
