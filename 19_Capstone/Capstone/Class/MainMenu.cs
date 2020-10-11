﻿using MenuFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Capstone.Class
{
    public class MainMenu : ConsoleMenu
    {
        public MainMenu()
        {
            AddOption("Display Vending Machine Items", OpenInventory);
            AddOption("Purchase", MakePurchase);
            AddOption("Print Sales Report(Employees Only)", PrintSalesReport);
            AddOption("Exit", Exit);

            
            Configure(cfg =>
            {
                cfg.Title = $@"
####################################
     Welcome to Vendo-Matic 800
####################################
            ___________
           | _   _   _ |       
           ||_| |_| |_||  
           | _   _   _ |
           ||_| |_| |_||
           |       $$$ |
           |  _______  |
           | |-------| |
           | |_______| |
           |___________|
          /_/_|__|__|_\_\
         /_/__|__|__|__\_\ 

************************************
            <<Main Menu>>
************************************
";

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

        private MenuOptionResult PrintSalesReport()
        {
            VendingMachine vendingMaching = new VendingMachine();
            string fileName = "SalesReport.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);

            Console.ForegroundColor = ConsoleColor.Green;
            try
            {
                using (StreamReader currentRecord = new StreamReader(fullPath))
                {
                    while (!currentRecord.EndOfStream)
                    {
                        Console.WriteLine(currentRecord.ReadLine()); 
                        
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Sales data not found. Please check with our sales deparment again (check the path and file name).");
                Console.WriteLine($"Error: {e.Message}");
            }
            Console.ResetColor();

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult Exit()
        {
            //VendingMachine vendingMachine = new VendingMachine();
            //Console.WriteLine($"{vendingMachine.DisplayItems()}"); // Got rid of the extra line at the end of the list

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
        ,,,,,,,,,,,,,
    .;;;;;;;;;;;;;;;;;;;,.
  .;;;;;;;;;;;;;;;;;;;;;;;;,
.;;;;;;;;;;;;;;;;;;;;;;;;;;;;.
;;;;;@;;;;;;;;;;;;;;;;;;;;;;;;' .............
;;;;@@;;;;;;;;;;;;;;;;;;;;;;;;'.................
;;;;@@;;;;;;;;;;;;;;;;;;;;;;;;'...................
`;;;;@;;;;;;;;;;;;;;;@;;;;;;;'.....................
 `;;;;;;;;;;;;;;;;;;;@@;;;;;'..................;....
   `;;;;;;;;;;;;;;;;@@;;;;'....................;;...
     `;;;;;;;;;;;;;@;;;;'...;.................;;....
        `;;;;;;;;;;;;'   ...;;...............;.....
           `;;;;;;'        ...;;..................
              ;;              ..;...............
              `                  ............
             `                      ......
            `                         ..
           `                           '
          `                           '
         `                           '
        `                           `
");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Thank you for using Vendo-Matic 800! See you next time!");
            Console.ResetColor();
            Console.WriteLine();

            return MenuOptionResult.ExitAfterSelection;
        }
    }
}
