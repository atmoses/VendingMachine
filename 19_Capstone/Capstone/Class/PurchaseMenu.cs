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
        }

        private MenuOptionResult SelectItemToPurchase()
        {
            throw new NotImplementedException();
        }

        private MenuOptionResult AddToBalance()
        {
            throw new NotImplementedException();
        }
    }
}
