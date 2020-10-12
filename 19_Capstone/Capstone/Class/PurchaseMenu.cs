using MenuFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Capstone.Class
{
    public class PurchaseMenu : ConsoleMenu
    {
        public VendingMachine vendingMachine = new VendingMachine();
        
        public PurchaseMenu()
        {            
            AddOption($"Feed Money", AddToBalance);
            AddOption("Select Product", SelectItemToPurchase);
            AddOption("Finish Purchase", FinishPurchase);
            AddOption("Return to <Main Menu>", ReturnToMain);
            Console.WriteLine("Press ESC to return to <Main Menu>");
        }



        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            Console.WriteLine($@"+++++++++++++++++++++++++
    <<Purchase Menu>>      
Current Balance is: {vendingMachine.Balance:C}
+++++++++++++++++++++++++");

            Console.WriteLine();
        }

        private MenuOptionResult FinishPurchase()
        {
            vendingMachine.ReturnChange();
            //if(vendingMachine.Balance > 0)
            //{
            //    Console.WriteLine($"Change Due: {vendingMachine.Balance:c}");
            //    int quarters = (int)(vendingMachine.Balance / .25M);
            //    vendingMachine.Balance -= quarters * .25M;
            //    int dimes = (int)(vendingMachine.Balance / .1M);
            //    vendingMachine.Balance -= dimes * .1M;
            //    int nickels = (int)(vendingMachine.Balance / .05M);
            //    vendingMachine.Balance -= nickels * .05M;
            //    int pennies = (int)(vendingMachine.Balance / .01M);
            //    vendingMachine.Balance -= pennies * .01M;


            //    if (quarters > 0)
            //    {
            //        Console.WriteLine($"{"Quarters:",-12}{quarters}");
            //    }
            //    if (dimes > 0)
            //    {
            //        Console.WriteLine($"{"Dimes:",-12}{dimes}");
            //    }
            //    if (nickels > 0)
            //    {
            //        Console.WriteLine($"{"Nickels:",-12}{nickels}");
            //    }
            //    if (pennies > 0)
            //    {
            //        Console.WriteLine($"{"Pennies:",-12}{pennies}");
            //    }
            //    Console.WriteLine();
            //    Console.WriteLine("Press ENTER to return <Purchase Menu>.");
                //return MenuOptionResult.WaitAfterMenuSelection;
            //}
            //Console.WriteLine("No Change Returned.");
            //Console.WriteLine();
            //Console.WriteLine("Press ENTER to return to <Purchase Menu>.");
            return MenuOptionResult.WaitAfterMenuSelection;

        }

        private MenuOptionResult SelectItemToPurchase()
        {
            vendingMachine.DisplayItems();
            Console.WriteLine();
            Console.Write("Please enter your selection: ");
            string selectedItem = Console.ReadLine().ToUpper();            
            if (selectedItem == "")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You have not selected anything yet. Please press Enter and select an item to purchase!");
                Console.ResetColor();
            }
            else if (vendingMachine.EligibleToSelect(selectedItem) == 3)
            {
                vendingMachine.SelectItem(selectedItem);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(vendingMachine.ErrorMessage);
                Console.ResetColor();
            }            
            vendingMachine.Checkout();
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult AddToBalance()
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@" 
--------------------------------------------------------------------.
| .--                    FEDERAL RESERVE NOTE                    .-- |
| |_       ......    THE UNTIED STATES OF AMERICA                |_  |
| __)    ``````````             ______            B93810455B     __) |
|      2        ___            /      \                     2        |
|              /|~\\          /  _-\\  \           __ _ _ _  __      |
|             | |-< |        |  //   \  |         |_  | | | |_       |
|              \|_//         | |-  o o| |         |   | `.' |__      |
|               ~~~          | |\   b.' |                            |
|       B83910455B           |  \ '~~|  |                            |
| .--  2                      \_/ ```__/    ....            2    .-- |
| |_        ///// ///// ////   \__\'`\/      ``  //// / ////     |_  |
| __)                   F I V E  D O L L A R S                   __) |
`--------------------------------------------------------------------'
");
            Console.ResetColor();
            int amount = GetInteger("Please Enter The Amount You Would Like to Deposit:");
            if (amount == 1 || amount == 2 || amount == 5 || amount == 10)
            {
                vendingMachine.Deposit(amount);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Your Balance is Now: {vendingMachine.Balance:C}");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press ENTER to return to <Purchase Menu>.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry this bill can not be used");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press ENTER and insert your bill again.");
            }
            return MenuOptionResult.WaitAfterMenuSelection;            
        }

        private MenuOptionResult ReturnToMain()
        {
            return MenuOptionResult.ExitAfterSelection;
        }

        
    }
}
