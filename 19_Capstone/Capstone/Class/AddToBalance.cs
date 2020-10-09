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
            AddOption("$1", AddOneDollar);
            AddOption("$2", AddTwoDollar);
            AddOption("$5", AddFiveDollar);
            AddOption("$10", AddTenDollar);

        }

        private MenuOptionResult AddTenDollar()
        {
            throw new NotImplementedException();
        }

        private MenuOptionResult AddFiveDollar()
        {
            throw new NotImplementedException();
        }

        private MenuOptionResult AddTwoDollar()
        {
            throw new NotImplementedException();
        }

        private MenuOptionResult AddOneDollar()
        {
            throw new NotImplementedException();
        }
    }
}
