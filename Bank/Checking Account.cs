using System;
using BankApp;

namespace BankApp
{
    public class CheckingAccount : Account
    {
        public double OverdraftLimit;

        public CheckingAccount(Customer customer, int accountNumber, double initialDeposit, double overdraftLimit)
            : base(customer, accountNumber, 0, initialDeposit, "Checking", -overdraftLimit)
        {
            OverdraftLimit = overdraftLimit;
            InterestRate = 0.01;
        }

        public override void ApplyInterest()
        {
            if (Balance > 0)
            {
                Balance += Balance * ((InterestRate / 5) / 12);
                Console.WriteLine($"Applied interest to account {AccountNumber}. New balance: {Balance}$");
            }
            else
            {
                Console.WriteLine($"No interest applied to account {AccountNumber} as balance is non-positive (or zero).");
            }
        }

        public void Pay(double payAmount)
        {
            if (payAmount > 0 && (Balance + OverdraftLimit) >= payAmount)
            {
                Balance -= payAmount;
                Console.WriteLine($"{customer.Name} paid out {payAmount}$ from account {AccountNumber}. New balance: {Balance}$");
            }
            else
            {
                Console.WriteLine($"Payment of {payAmount}$ exceeds available balance and overdraft limit in account {AccountNumber}.");
            }
        }

        public void Ctransfer(Account targetAccount, double amount)
        {
            if (amount > 0 && (Balance + OverdraftLimit) >= amount)
            {
                Balance -= amount;
                targetAccount.Balance += amount;
                CheckLoan();
                Console.WriteLine($"Transferred {amount}$ from account {AccountNumber} to account {targetAccount.AccountNumber}. New balance is {Balance}$.");
            }
            else
            {
                Console.WriteLine("Insufficient funds or overdraft limit for transfer.");
            }
        }
    }
}