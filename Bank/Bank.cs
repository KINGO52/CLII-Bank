using System;
using System.Collections.Generic;
using BankApp;


namespace BankApp // <-- Change from 'Bank' to 'BankApp'
{
    public class Bank
    {
        public List<Customer> Customers => Customer.Manager.GetAllCustomers();

        public List<Account> Accounts
        {
            get
            {
                var accounts = new List<Account>();
                foreach (var customer in Customers)
                {
                    if (customer.accounts != null)
                        accounts.AddRange(customer.accounts);
                }
                return accounts;
            }
        }

        public void AddCustomer()
        {
            string Name, CustomerID, Address, Phone;
            double initialDeposit;

            Console.WriteLine("Enter customer Name");
            Name = Console.ReadLine();
            Console.WriteLine("Enter customer ID");
            CustomerID = Console.ReadLine();
            Console.WriteLine("Enter customer Address");
            Address = Console.ReadLine();
            Console.WriteLine("Enter customer Phone number");
            Phone = Console.ReadLine();
            Console.WriteLine("Enter initial deposit amount");
            initialDeposit = Convert.ToDouble(Console.ReadLine());
            var newCustomer = new Customer(Name, CustomerID, Address, Phone, initialDeposit);
            TransactionDatabase.AddCustomer(newCustomer); // <-- Add customer to TransactionDatabase

            try
            {
                // Removed TransactionDatabase reference
                Console.WriteLine($"Added customer {newCustomer.Name} with ID {newCustomer.CustomerID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding customer: {ex.Message}");
            }
        }

        public void ListCustomers()
        {
            Console.WriteLine("Listing all customers:");
            foreach (var customer in Customers)
            {
                if (customer.accounts == null || customer.accounts.Count == 0) continue;
                Console.WriteLine($"Customer ID: {customer.CustomerID}, Name: {customer.Name}, Accounts:");
                foreach(var acc in customer.accounts)
                {
                    Console.WriteLine($" Account:{acc.AccountNumber} which has {acc.Balance}$");
                }
            }
        }

        public void ListAccounts(Customer customer)
        {
            Console.WriteLine("Listing all accounts:");
            foreach (var account in customer.accounts)
            {
                Console.WriteLine($"Account Number: {account.AccountNumber}, Balance: {account.Balance}$");
            }
        }

        public void RemoveCustomer(string customerID)
        {
            var customer = Customer.Manager.GetCustomer(customerID);
            if (customer != null)
            {
                Customer.Manager.RemoveCustomer(customerID);
                Console.WriteLine($"Removed customer {customer.Name} with ID {customer.CustomerID}");
            }
            else
            {
                Console.WriteLine($"Customer with ID {customerID} not found.");
            }
        }

        public void AddAccount(Customer customer, double initialDeposit)
        {
            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.WriteLine("Select account type to add (1: Checking, 2: Saving):");
            string choice = Console.ReadLine();
            try
            {
                if(choice == "1")
                {
                    Console.WriteLine("Enter overdraft limit for Checking Account:");
                    double overdraftLimit = Convert.ToDouble(Console.ReadLine());
                    var newAccount = new CheckingAccount(customer, GenerateAccountNumber(), initialDeposit, overdraftLimit);
                    customer.accounts.Add(newAccount);
                    // Removed TransactionDatabase reference
                    Console.WriteLine($"Added Checking account {newAccount.AccountNumber} for customer {customer.Name} with initial deposit {initialDeposit}$ and overdraft limit {overdraftLimit}$");
                    return;
                }
                else if(choice == "2")
                {
                    Console.WriteLine("Enter minimum balance for Saving Account:");
                    double minimumBalance = Convert.ToDouble(Console.ReadLine());
                    var newAccount = new SavingAccount(customer, GenerateAccountNumber(), initialDeposit, minimumBalance);
                    customer.accounts.Add(newAccount);
                    // Removed TransactionDatabase reference
                    Console.WriteLine($"Added Saving account {newAccount.AccountNumber} for customer {customer.Name} with initial deposit {initialDeposit}$ and minimum balance {minimumBalance}$");
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Account not added.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding account: {ex.Message}");
            }
        }

        public void AddAccount()
        {
            Console.WriteLine("Enter customer ID:");
            string? customerId = Console.ReadLine();
            if (string.IsNullOrEmpty(customerId))
            {
                Console.WriteLine("Invalid customer ID");
                return;
            }

            var customer = Customer.Manager.GetCustomer(customerId);
            if (customer == null)
            {
                Console.WriteLine("Customer not found");
                return;
            }

            Console.WriteLine("Enter initial deposit amount:");
            if (!double.TryParse(Console.ReadLine(), out double initialDeposit))
            {
                Console.WriteLine("Invalid amount");
                return;
            }

            AddAccount(customer, initialDeposit);
        }

        public void ListAccounts()
        {
            Console.WriteLine("Enter customer ID:");
            string? customerId = Console.ReadLine();
            if (string.IsNullOrEmpty(customerId))
            {
                Console.WriteLine("Invalid customer ID");
                return;
            }

            var customer = Customer.Manager.GetCustomer(customerId);
            if (customer == null)
            {
                Console.WriteLine("Customer not found");
                return;
            }

            ListAccounts(customer);
        }

        public void Deposit()
        {
            Console.WriteLine("Enter account number:");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number");
                return;
            }

            var account = TransactionDatabase.GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found");
                return;
            }

            account.Deposit();
        }

        public void Withdraw()
        {
            Console.WriteLine("Enter account number:");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number");
                return;
            }

            Console.WriteLine("Enter amount to withdraw:");
            if (!double.TryParse(Console.ReadLine(), out double amount))
            {
                Console.WriteLine("Invalid amount");
                return;
            }

            var account = TransactionDatabase.GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found");
                return;
            }

            account.Withdraw(amount);
        }

        public void Transfer()
        {
            Console.WriteLine("Enter source account number:");
            if (!int.TryParse(Console.ReadLine(), out int fromAccountNumber))
            {
                Console.WriteLine("Invalid source account number");
                return;
            }

            Console.WriteLine("Enter target account number:");
            if (!int.TryParse(Console.ReadLine(), out int toAccountNumber))
            {
                Console.WriteLine("Invalid target account number");
                return;
            }

            Console.WriteLine("Enter amount to transfer:");
            if (!double.TryParse(Console.ReadLine(), out double amount))
            {
                Console.WriteLine("Invalid amount");
                return;
            }

            var fromAccount = TransactionDatabase.GetAccount(fromAccountNumber);
            var toAccount = TransactionDatabase.GetAccount(toAccountNumber);
            if (fromAccount == null || toAccount == null)
            {
                Console.WriteLine("One or both accounts not found");
                return;
            }

            Account.Transfer(fromAccount, toAccount, amount);
        }

        public void ApplyInterest()
        {
            foreach (var account in Accounts)
            {
                if (account is CheckingAccount checkingAccount)
                    checkingAccount.ApplyInterest();
                else if (account is SavingAccount savingAccount)
                    savingAccount.ApplyInterest();
            }
        }

        private int GenerateAccountNumber()
        {
            return new Random().Next(100000, 999999);
        }
    }   
}