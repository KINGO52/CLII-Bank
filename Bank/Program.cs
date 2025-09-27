// See https://aka.ms/new-console-template for more information
using System;
using System.Runtime.CompilerServices;
using BankApp;
using TransactionApp;
using static BankApp.TransactionDatabase;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadCustomers();
            LoadAccounts();
            int item = 10;
            Bank bank = new Bank();
            bool exit = false;
            bool adminmode = false;
            bool passwordcorrect = false;

            while (!exit)
            {
                while (adminmode)
                {
                    Console.WriteLine("Enter the Admin Password.");
                    if (passwordcorrect || Console.ReadLine() == "admin123")

                    {
                        passwordcorrect = true;
                        Console.WriteLine("Admin mode activated.");
                        Console.WriteLine("1. View All Transactions");
                        Console.WriteLine("2. View All Loans");
                        Console.WriteLine("3. Edit Interest Rates");
                        Console.WriteLine("4. Edit Overdraft Limits");
                        Console.WriteLine("5. View System Logs");
                        Console.WriteLine("6. Edit Minimum Balances");
                        Console.WriteLine("7. Apply Monthly Loan Adjustments");
                        Console.WriteLine("8. Apply Interest to All Accounts");
                        Console.WriteLine("9. Exit Admin Mode");
                        string choice1 = Console.ReadLine();
                        switch (choice1)
                        {
                            case "1":
                                Console.WriteLine("All Transactions:");
                                foreach (var transaction in TransactionDatabase.Transactions.Values)
                                {
                                    Console.WriteLine($"Transaction ID: {transaction.TransactionId}, Account: {transaction.FromAccountNumber}, Amount: {transaction.Amount}, Date: {transaction.TransactionDate.ToString()}");
                                }
                                break;
                            case "2":
                                Console.WriteLine("All Loans:");
                                foreach (var account in bank.Accounts)
                                {
                                    if (account.Loan > 0)
                                        Console.WriteLine($"Account Number: {account.AccountNumber}, Customer: {account.customer.Name}, Loan Amount: {account.Loan}$");
                                }
                                break;
                            case "3":
                                Console.WriteLine("Enter new interest rate (as a decimal, e.g., 0.05 for 5%):");
                                if (double.TryParse(Console.ReadLine(), out double newRate))
                                {
                                    foreach (var account in bank.Accounts)
                                    {
                                        account.InterestRate = newRate;
                                    }
                                    Console.WriteLine($"Interest rates updated to {newRate * 100}% for all accounts.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Interest rates not changed.");
                                }
                                break;
                            case "4":
                                Console.WriteLine("Enter account number to edit overdraft limit:");
                                if (int.TryParse(Console.ReadLine(), out int accNum))
                                {
                                    var account = TransactionDatabase.GetAccount(accNum) as CheckingAccount;
                                    if (account != null)
                                    {
                                        Console.WriteLine($"Current overdraft limit for account {accNum} is {account.OverdraftLimit}$. Enter new overdraft limit:");
                                        if (double.TryParse(Console.ReadLine(), out double newLimit))
                                        {
                                            account.OverdraftLimit = newLimit;
                                            account.NISMB(-newLimit);
                                            Console.WriteLine($"Overdraft limit updated to {newLimit}$ for account {accNum}.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid input. Overdraft limit not changed.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Account not found or is not a checking account.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid account number.");
                                }
                                break;
                            case "5":
                                Console.WriteLine("System Logs feature not implemented yet.");
                                break;
                            case "6":
                                Console.WriteLine("Enter account number to edit minimum balance:");
                                if (int.TryParse(Console.ReadLine(), out int accNumMin))
                                {
                                    foreach (var Acc in bank.Accounts)
                                    {
                                        if (Acc.AccountNumber == accNumMin)
                                        {
                                            Acc.EditminimumBalance();
                                            break;
                                        }
                                    }
                                }
                                break;
                            case "7":
                                foreach (var account in bank.Accounts)
                                {
                                    account.AMLA(account);
                                }
                                Console.WriteLine("Applied monthly loan adjustments to all accounts with loans.");
                                break;
                            case "8":
                                foreach (var account in bank.Accounts)
                                {
                                    account.ApplyInterest();
                                }
                                Console.WriteLine("Applied interest to all accounts.");
                                break;
                            case "9":
                                Console.WriteLine("Exiting admin mode.");
                                adminmode = false;
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }
                    }

                    else
                    {
                        Console.WriteLine("Incorrect password. Canceling Choice.");
                        item = 10; // Reset item to exit admin mode
                        adminmode = false;
                    }
                }

                Console.WriteLine("\nBanking System Menu:");
                Console.WriteLine("1. Add Customer");
                Console.WriteLine("2. List Customers");
                Console.WriteLine("3. Add Account");
                Console.WriteLine("4. List Accounts");
                Console.WriteLine("5. Deposit");
                Console.WriteLine("6. Withdraw");
                Console.WriteLine("7. Transfer");
                Console.WriteLine("8. Apply Interest");
                Console.WriteLine("9. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        bank.AddCustomer();
                        SaveCustomers();
                        TransactionDatabase.SaveAccounts(Accounts.Values.ToList());
                        break;
                    case "2":
                        bank.ListCustomers();
                        break;
                    case "3":
                        bank.AddAccount();
                        SaveAccounts(GetAllAccountsList());
                        break;
                    case "4":
                        bank.ListAccounts();
                        break;
                    case "5":
                        bank.Deposit();
                        break;
                    case "6":
                        bank.Withdraw();
                        break;
                    case "7":
                        bank.Transfer();
                        break;
                    case "8":
                        bank.ApplyInterest();
                        break;
                    case "9":
                        exit = true;
                        break;
                    case "10":
                        Console.WriteLine("Option 10 selected. Selecting Admin mode...");
                        adminmode = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}


