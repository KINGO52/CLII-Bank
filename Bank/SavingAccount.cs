using System;
using BankApp;

namespace BankApp
{
    public class SavingAccount : Account
    {
        public SavingAccount(Customer customer, int accountNumber, double initialDeposit, double minimumBalance)
            : base(customer, accountNumber, 0, initialDeposit, "Saving", minimumBalance)
        {
        }

        public override void ApplyInterest()
        {
            if (Balance > 0)
            {
                Balance += Balance * (InterestRate / 12);
                Console.WriteLine($"Applied interest to account {AccountNumber}. New balance: {Balance}$");
            }
            else
            {
                Console.WriteLine($"No interest applied to account {AccountNumber} as balance is non-positive (or zero).");
            }
        }
    }
}