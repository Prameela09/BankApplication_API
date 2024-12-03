using System;
using System.Text.RegularExpressions;
using BankManagement.Utilities.Enums;

namespace BankManagement.Utilities.HelperClasses;

public static class AccountHelper
{
    public static long GenerateAccountNumber()
    {
        return new Random().Next(100000000, 999999999);
    }

    public static decimal GetMinimumBalance(AccountName accountType)
    {
        return accountType == AccountName.Savings ? 1000m : 500m;
    }

    public static decimal GetWithdrawLimit()
    {
        return 2000m;
    }

    public static decimal GetOverDraft()
    {
        return 1000m;
    }
    public static decimal CalculateInterestRate(decimal balance)
    {
        decimal baseInterestRate = 0.01m; 
        int increments = (int)(balance / 10000);
        return baseInterestRate + (increments * 0.01m); 
    }

    public static decimal CalculateMonthlyFee(decimal balance)
    {
        decimal baseMonthlyFee = 0; 
        int increments = (int)(balance / 10000);
        return baseMonthlyFee + (increments * 50); 
    }

    public static bool VerifyAadharAndPan(string aadharNumber, string panNumber)
    {
        if (string.IsNullOrWhiteSpace(aadharNumber) || aadharNumber.Length != 12 || !aadharNumber.All(char.IsDigit))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(panNumber) || panNumber.Length != 10 ||
            !Regex.IsMatch(panNumber, @"^[A-Z]{5}[0-9]{4}[A-Z]$"))
        {
            return false;
        }
        return true;
    }
}



