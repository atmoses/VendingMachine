using Capstone.Class;
using System;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            VendingMachine vendingMachine = new VendingMachine();
            Console.WriteLine(vendingMachine.Balance);
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
        }
    }
}
