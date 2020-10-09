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
            
            AddOption($"Feed Money: {vendingMachine.Balance}", AddToBalance);
            AddOption("Select Product", SelectItemToPurchase);
            AddOption("Finish Purchase", FinishPurchase);
        }

        private MenuOptionResult FinishPurchase()
        {
            throw new NotImplementedException();
        }

        private MenuOptionResult SelectItemToPurchase()
        {
            throw new NotImplementedException();
        }

        public MenuOptionResult AddToBalance()
        {
            
            Console.WriteLine("Please Enter The Amount You Would Like to Deposit:");
            string deposit = Console.ReadLine();
            int amount = int.Parse(deposit);
            if (amount == 1 || amount == 2 || amount == 5 || amount == 10)
            {
                vendingMachine.Deposit(amount);
                Console.WriteLine($"Your Balance is Now: {vendingMachine.Balance}");
            }
            else
            {
                Console.WriteLine("Sorry this bill can not be used");
            }
            return MenuOptionResult.WaitAfterMenuSelection;
        }
        
    }
}
