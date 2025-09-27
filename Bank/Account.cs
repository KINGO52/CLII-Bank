using System;
using System.Text.Json.Serialization;


namespace BankApp
{
    public abstract class Account
    {
        public int AccountNumber { get; set; }
        public double Balance { get; set; }
        public double InterestRate = 0.05;
        public double Loan { get; set; }
        public readonly Customer customer;
        protected string accountType { get; set; } // changed to protected
        private double MinimumBalance { get; set; } // changed to protected


        // Constructor
        public Account(Customer customer, int accountNumber, double loan, double initialBalance, string accountType, double minimumBalance)
        {
            this.customer = customer;
            AccountNumber = accountNumber;
            Loan = loan;
            Balance = initialBalance;
            this.accountType = accountType;
            this.MinimumBalance = minimumBalance;
            TransactionDatabase.Accounts[accountNumber] = this; // Register account in TransactionDatabase

            if (Balance < 0)
            {
                Loan = -Balance;
                Balance = 0;
            }
        }

        public virtual void Deposit()
        {
            Console.WriteLine($"How much does {customer.Name} wish to deposit? Type 'a' to deposit to another account.");
            string input = Console.ReadLine();

            if (input.ToLower() == "a")
            {
                Console.WriteLine("Enter the account ID to deposit to:");
                string targetAccountID = Console.ReadLine();
                Console.WriteLine("Enter the amount to deposit:");
                string inputAmount = Console.ReadLine();

                if (double.TryParse(inputAmount, out double transferAmount))
                {
                    // Find target account in memory (example: first account of customer)
                    Account targetAccount = null;
                    foreach (var cust in Customer.Manager.GetAllCustomers())
                    {
                        if (cust.accounts != null)
                        {
                            foreach (var acc in cust.accounts)
                            {
                                if (acc.AccountNumber.ToString() == targetAccountID)
                                {
                                    targetAccount = acc;
                                    break;
                                }
                            }
                        }
                        if (targetAccount != null) break;
                    }

                    if (targetAccount != null && transferAmount > 0 && Balance >= transferAmount)
                    {
                        Balance -= transferAmount;
                        targetAccount.Balance += transferAmount;
                        Console.WriteLine($"Deposited {transferAmount}$ by {customer.Name} to account {targetAccountID}.");

                        if (targetAccount.Loan > 0)
                        {
                            targetAccount.Loan -= transferAmount;
                            if (targetAccount.Loan < 0)
                            {
                                targetAccount.Balance += -targetAccount.Loan;
                                targetAccount.Loan = 0;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Target account not found or insufficient funds.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid amount entered.");
                }
            }
            else
            {
                if (double.TryParse(input, out double depositAmount) && depositAmount > 0)
                {
                    Balance += depositAmount;
                    Console.WriteLine($"Deposited {depositAmount}$ to account {AccountNumber}. New balance is {Balance}$.");
                }
                else
                {
                    Console.WriteLine("Invalid amount entered.");
                }
            }
        }

        public static void Transfer(Account fromAccount, Account toAccount, double amount)
        {
            if (fromAccount.accountType == "Saving")
            {
                if (amount > (fromAccount.Balance - fromAccount.MinimumBalance))
                {
                    Console.WriteLine($"Cannot have less than {fromAccount.MinimumBalance}.");
                    return;
                }
            }
            if (fromAccount.Balance >= amount && amount > 0)
            {
                fromAccount.Balance -= amount;
                toAccount.Balance += amount;
                Console.WriteLine($"Transferred {amount} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}. New balance is {fromAccount.Balance}");

                if (toAccount.Loan > 0)
                {
                    toAccount.Loan -= amount;
                    if (toAccount.Loan < 0)
                    {
                        toAccount.Balance += -toAccount.Loan;
                        toAccount.Loan = 0;
                    }
                }
                fromAccount.CheckLoan();
            }
            else
            {
                Console.WriteLine("Insufficient funds or invalid amount for transfer.");
            }
        }

        public virtual void Withdraw(double amount)
        {
            if (amount > 0 && Balance >= amount)
            {
                Balance -= amount;
                Console.WriteLine($"Withdrew {amount}. New balance is {Balance}");
            }
            else
            {
                Console.WriteLine("Insufficient funds or invalid amount for withdrawal.");
            }
        }

        public void CheckLoan()
        {
            if (Balance < 0)
            {
                Loan = -Balance;
                Balance = 0;
            }
        }

        public void AMLA(Account account) //Apply Monthly Loan Adjustment
        {
            if (Balance > 0)
            {
                account.Loan *= (1 + InterestRate / 12);
            }
        }
        public virtual void ApplyInterest()
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
        public virtual void EditminimumBalance() // AcsAccount ==> Accesed Account
        {
            double temp;
            Console.WriteLine("Enter new minimum balance:");
            if (double.TryParse(Console.ReadLine(), out double newMinBalance))
            {
                temp = MinimumBalance;
                MinimumBalance = newMinBalance;
                Console.WriteLine($"Minimum balance updated from {temp} to {MinimumBalance}$ for account {AccountNumber}.");
            }
            else
            {
                Console.WriteLine("Invalid input. Minimum balance not changed.");
            }
        }
        public void NISMB(double newMinBalance) // No Input Add Minimum Balance
        {
            MinimumBalance += newMinBalance;
        }
    }
}