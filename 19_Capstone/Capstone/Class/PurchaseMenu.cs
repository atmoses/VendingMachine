using MenuFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Class
{
    public class PurchaseMenu : ConsoleMenu
    {
        public PurchaseMenu()
        {
            AddOption("Feed Money", AddToBalance);
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

        private MenuOptionResult AddToBalance()
        {
            AddToBalance addToBalance = new AddToBalance();
            addToBalance.Show();
            return MenuOptionResult.DoNotWaitAfterMenuSelection;
        }
    }
}
