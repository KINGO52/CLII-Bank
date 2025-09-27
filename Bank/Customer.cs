/*
TODO:
- Ensure Account and Customer classes are not duplicated across namespaces.
- Refactor Account logic to avoid confusion with AccountApp.Account.
- Add validation for customer creation.
- Add exception handling for dictionary access.
- Add unit tests for customer management.
*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BankApp;

namespace BankApp
{
    // Represents a customer
    public class Customer
    {
        public string Name { get; set; }
        public string CustomerID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public double IR { get; set; }
        public double Loan { get; set; } // Add Loan property
        public double InitialDeposit { get; set; }
        public List<Account> accounts; // List of accounts

        // Static manager for all customers
        public static CustomerManager Manager = new CustomerManager();

        // Constructor
        [JsonConstructor]
        public Customer(string name, string customerID, string address, string phone, double initialDeposit)
        {
            this.Name = name;
            this.CustomerID = customerID;
            this.Address = address;
            this.Phone = phone;
            IR = 0.05;
            this.InitialDeposit = initialDeposit;

            accounts = new List<Account>();
            var account = new SavingAccount(this, GenerateAccountNumber(), initialDeposit, 0); // Default minimumBalance
            accounts.Add(account);

            // Initialize Loan if initialDeposit is negative
            Loan = initialDeposit < 0 ? -initialDeposit : 0;

            // Add this customer to the manager
            if (Manager.GetCustomer(customerID) != null)
            {
                Console.WriteLine($"Customer with ID {customerID} already exists.");
                
            } else Manager.AddCustomer(this);

        }

        // Dummy method for account number generation
        private int GenerateAccountNumber()
        {
            return new System.Random().Next(100000, 999999);
        }
    }

    // Manages all customers
    public class CustomerManager
    {
        private Dictionary<string, Customer> customers = new Dictionary<string, Customer>();

        public void AddCustomer(Customer c)
        {
            if (c != null && c.CustomerID != null && !customers.ContainsKey(c.CustomerID))
            {
                customers.Add(c.CustomerID, c);
            }

        }

        public Customer GetCustomer(string id)
        {
            if (id == null) return null;
            return customers.ContainsKey(id) ? customers[id] : null;
        }

        public List<Customer> GetAllCustomers()
        {
            return new List<Customer>(customers.Values);
        }

        // Optional: Remove customer
        public void RemoveCustomer(string customerID)
        {
            customers.Remove(customerID);
        }
    }
}

