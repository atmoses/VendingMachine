using MenuFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Class
{
    public class AddToBalance : ConsoleMenu
    {
        public AddToBalance()
        {
            VendingMachine vendingMachine = new VendingMachine();
            Console.WriteLine("Please Enter The Amount You Would Like to Deposit:");
            string deposit = Console.ReadLine();
            int amount = int.Parse(deposit);
            if (amount == 1 || amount == 2 || amount == 5 || amount == 10)
            {
                vendingMachine.Deposit(amount);
            }
            else
            {
                Console.WriteLine("Sorry this bill can not be used");
            }
            AddOption("Return to Make Purchase",);
        }
    }
}
