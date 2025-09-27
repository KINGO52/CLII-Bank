using System;

namespace BankApp
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int FromAccountNumber { get; set; }
        public int ToAccountNumber { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }

        public Transaction(int transactionId, int fromAccount, int toAccount, double amount, string description = "")
        {
            TransactionId = transactionId;
            FromAccountNumber = fromAccount;
            ToAccountNumber = toAccount;
            Amount = amount;
            TransactionDate = DateTime.Now;
            Description = description;
        }
    }
}
