using MenuFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Class
{
    public class PurchaseMenu : ConsoleMenu
    {
        public VendingMachine vendingMachine = new VendingMachine();
        //private static MenuOptionResult DisplayBalance()
        //{
        //    Console.WriteLine("this is a test");
        //    return MenuOptionResult.WaitAfterMenuSelection;
        //}
        public PurchaseMenu()
        {
            
            AddOption($"Feed Money", AddToBalance);
            AddOption("Select Product", SelectItemToPurchase);
            AddOption("Finish Purchase", FinishPurchase);

            
        }
        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            Console.WriteLine($"Current Balance is: {vendingMachine.Balance:C}");
        }

        private MenuOptionResult FinishPurchase()
        {
            vendingMachine.ReturnChange();
            if(vendingMachine.Balance > 0)
            {
                Console.WriteLine($"Change Due: {vendingMachine.Balance:c}");
                int quarters = (int)(vendingMachine.Balance / .25M);
                vendingMachine.Balance -= quarters * .25M;
                int dimes = (int)(vendingMachine.Balance / .1M);
                vendingMachine.Balance -= dimes * .1M;
                int nickels = (int)(vendingMachine.Balance / .05M);
                vendingMachine.Balance -= nickels * .05M;
                int pennies = (int)(vendingMachine.Balance / .01M);
                vendingMachine.Balance -= pennies * .01M;
                Console.WriteLine($"Quarters: {quarters},");
                Console.WriteLine($"Dimes: {dimes},");
                Console.WriteLine($"Nickels: {nickels},");
                Console.WriteLine($"Pennies: {pennies}");
                return MenuOptionResult.WaitAfterMenuSelection;
            }
            return MenuOptionResult.WaitAfterMenuSelection;

        }

        private MenuOptionResult SelectItemToPurchase()
        {
            vendingMachine.DisplayItems();
            Console.WriteLine();
            Console.Write("Please enter your selection: ");
            string selectedItem = Console.ReadLine();            
            if (selectedItem == "")
            {
                Console.WriteLine("You have not selected anything yet. Please press Enter and select an item to purchase!");
            }
            else if (vendingMachine.EligibleToSelect(selectedItem) == 3)
            {
                vendingMachine.SelectItem(selectedItem);
            }
            else
            {
                Console.WriteLine(vendingMachine.ErrorMessage);
            }            
            vendingMachine.Checkout();
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        public MenuOptionResult AddToBalance()
        {
            
            int amount = GetInteger("Please Enter The Amount You Would Like to Deposit:");
            if (amount == 1 || amount == 2 || amount == 5 || amount == 10)
            {
                vendingMachine.Deposit(amount);
                Console.WriteLine($"Your Balance is Now: {vendingMachine.Balance:C}");
            }
            else
            {
                Console.WriteLine("Sorry this bill can not be used");
            }
            return MenuOptionResult.WaitAfterMenuSelection;
        }
        
    }
}
