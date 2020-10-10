using MenuFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Class
{
    public class MainMenu : ConsoleMenu
    {
        public MainMenu()
        {
            AddOption("Display Vending Machine Items", OpenInventory);
            AddOption("Purchase", MakePurchase);
            AddOption("Exit", Exit);

            Configure(cfg =>
            {
                cfg.Title = $"Welcome to Vendo-Matic 800, your balance is ";
            });

            
        }
        


        private MenuOptionResult MakePurchase()
        {
            PurchaseMenu purchaseMenu = new PurchaseMenu();
            purchaseMenu.Show();
            return MenuOptionResult.DoNotWaitAfterMenuSelection;
        }

        private MenuOptionResult OpenInventory()
        {
            VendingMachine vendingMachine = new VendingMachine();
            //Console.WriteLine($"{vendingMachine.DisplayItems()}"); // Got rid of the extra line at the end of the list
            vendingMachine.DisplayItems();
            return MenuOptionResult.WaitAfterMenuSelection;
        }
    }
}
