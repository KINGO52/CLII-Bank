namespace TransactionApp
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime DateOT { get; set; } // Changed from Date to DateOT ==> Date of Transaction
        public double Amount { get; set; }
        public string TypeOfTransaction { get; set; } // "Deposit", "Withdrawal", "Transfer" "Payment"
        public int FromAccount { get; set; } // Account number
        public int ToAccount { get; set; } // Account number, if applicable if N/A then 0

        public Transaction(int transactionId, DateTime date, double amount, string type, int fromAccount, int toAccount = 0)
        {
            TransactionId = transactionId;
            DateOT = date == default ? DateTime.Now : date;
            Amount = amount;
            TypeOfTransaction = type;
            FromAccount = fromAccount;
            ToAccount = toAccount;
        }
    }
}