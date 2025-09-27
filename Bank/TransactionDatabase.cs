using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BankApp;

namespace BankApp
{
    public static class TransactionDatabase
    {
        // Stores all customers by ID
        public static Dictionary<string, Customer> Customers { get; }  = new Dictionary<string, Customer>();

        // Stores all accounts by account number
        public static Dictionary<int, Account> Accounts { get; } = new Dictionary<int, Account>();

        // Stores all transactions by transaction ID
        public static Dictionary<int, Transaction> Transactions { get; } = new Dictionary<int, Transaction>();

        private static readonly string CustomersFile = "customers.json";
        private static readonly string AccountsFile = "accounts.json";

        // Add a customer
        public static void AddCustomer(Customer customer)
        {   
            Console.WriteLine("Adding customer to database: " + customer.CustomerID);
            if (!Customers.ContainsKey(customer.CustomerID))
            {
                Console.WriteLine("Customer added to database: " + customer.CustomerID);
                Customers[customer.CustomerID] = customer;
                SaveCustomers();
            }
        }

        // Add an account
        public static void AddAccount(Account account)
        {
            if (!Accounts.ContainsKey(account.AccountNumber))
            {
                Accounts[account.AccountNumber] = account;
                SaveAccounts(Accounts.Values.ToList());
            }
        }

        // Add a transaction
        public static void AddTransaction(Transaction transaction)
        {
            if (!Transactions.ContainsKey(transaction.TransactionId))
                Transactions[transaction.TransactionId] = transaction;
        }

        // Get customer by ID
        public static Customer GetCustomer(string customerID)
        {   if(customerID == null) return null;
            return Customers.TryGetValue(customerID, out var customer) ? customer : null;
        }

        // Get account by account number
        public static Account GetAccount(int accountNumber)
        {
            return Accounts.TryGetValue(accountNumber, out var account) ? account : null;
        }

        // Get transaction by transaction ID
        public static Transaction GetTransaction(int transactionId)
        {
            return Transactions.TryGetValue(transactionId, out var transaction) ? transaction : null;
        }

        // Get all customers
        public static List<Customer> GetAllCustomers()
        {
            return new List<Customer>(Customers.Values);
        }

        // Get all accounts
        public static List<Account> GetAllAccounts()
        {
            return new List<Account>(Accounts.Values);
        }

        // Get all transactions
        public static List<Transaction> GetAllTransactions()
        {
            return new List<Transaction>(Transactions.Values);
        }

        // Save all customers to JSON file
        // Save all customers to JSON file
        public static void SaveCustomers()
        {
            var customersList = new List<Customer>(Customers.Values);
            var options = new JsonSerializerOptions { WriteIndented = true };

            // Check if a customer with the same ID already exists
            foreach (var customer in Customers.Values)
            {
                if (Customers.ContainsKey(customer.CustomerID))
                {
                    
                    continue;
                }

                // Customer does not exist, add it to the dictionary
                Customers[customer.CustomerID] = customer;
            }

            // Serialize and save the updated customers list to the JSON file
            File.WriteAllText(CustomersFile, JsonSerializer.Serialize(customersList, options));
        }

        // Load all customers from JSON file
        public static void LoadCustomers()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true // If you're using fields instead of properties
            };

            if (File.Exists(CustomersFile))
            {
                var json = File.ReadAllText(CustomersFile);
                var customersList = JsonSerializer.Deserialize<List<Customer>>(json, options);
                Customers.Clear();
                if (customersList != null)
                {
                    foreach (var customer in customersList)
                    {
                        if(customer == null || customer.CustomerID == null) continue;
                        Customers[customer.CustomerID] = customer;
                        Customer.Manager.AddCustomer(customer);
                    }
                }
            }
        }

        public static List<Account> LoadAccounts()
        {
            try
            {
                if (File.Exists("accounts.json")) // Replace with your actual file path
                {
                    string json = File.ReadAllText("accounts.json");
                    
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new List<Account>();
                    }

                    var options = new JsonSerializerOptions
                    {
                        Converters = { new AccountConverter() },
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };

                    return JsonSerializer.Deserialize<List<Account>>(json, options) ?? new List<Account>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading accounts: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return new List<Account>();
        }

        // Save all accounts to JSON file
        // Save all accounts to JSON file
        public static void SaveAccounts(List<Account> accounts)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new AccountConverter() },
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(accounts, options);
                File.WriteAllText("accounts.json", json); // Replace with your actual file path
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving accounts: {ex.Message}");
            }
        }
        static TransactionDatabase()
        {
            AppDomain.CurrentDomain.ProcessExit += SaveCustomersAndAccounts;
        }
        private static void SaveCustomersAndAccounts(object sender, EventArgs e)
        {
            SaveCustomers();
            SaveAccounts(GetAllAccounts());
        }
        public static List<Account> GetAllAccountsList()
        {
            return TransactionDatabase.Accounts.Values.ToList();
        }
    }
}


