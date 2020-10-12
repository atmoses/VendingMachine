using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;

namespace Capstone.Class
{
    public class VendingMachine
    {
        public decimal Balance { get; private set; }

        public string ErrorMessage { get; private set; }

        public int NumberOfItemLeft { get; private set; }

        private decimal PreviousBalance { get; set; } 

        public string Selection { get; private set; }

        public decimal TotalDue { get; private set; }

        private decimal UpToDateSales { get; set; }

        public Item SelectedItem { get; private set; }

        public Dictionary<Item, int> inventory { get; private set; } = new Dictionary<Item, int>();

        public Dictionary<Item, int> soldItems { get; private set; } = new Dictionary<Item, int>();




        public VendingMachine() // "Turn it on!"
        {

            // Check if there is still an exiting user on the current vending maching
            int n;
            string fileName;
            string currentFolder = Environment.CurrentDirectory;
            string path = Path.Combine(currentFolder, @"..\..\..\..\");
            if (File.Exists(Path.Combine(path, "vendingmachineCurrent.txt")))
            {
                fileName = "vendingmachineCurrent.txt";
                n = 1;
            }
            else
            {
                fileName = "vendingmachine.csv";
                n = 0;
            }

            currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);

            LoadInventory(fullPath, n);
            ReadPreviousSalesReport();

            // Reload the Balance from the current user
            if (File.Exists(Path.Combine(path, "CurrentUserInfo.txt")))
            {
                ReadCurrentUserLog();
            }
        }



        public void Checkout() // Empty the SelectedItem Dictionary and "print out" a receipt; Dean added SalesLog(), AddtoSoldItem() and GenerateSalesReport() => see private methods 
        {
            //SalesLog();            
            //AddToSoldItems();
            //Console.WriteLine($"You have purchased {SelectedItem.Name} for {SelectedItem.Price:c}. Thank you for your business!");

            //GenerateSalesReport();
            //ClearUserLog();
            SelectedItem = new Item();

            
        }


        public void Deposit(decimal deposit) 
        {
            if (deposit <= 0) // The use must always input a positive decimal number
            {
                ErrorMessage = "The bill you inserted is a counterfeit (negative decimal).  Please insert a real bill (positive decimal).";
                Console.WriteLine(ErrorMessage);
            }
            else
            {
                PreviousBalance = Balance; 
                Balance += deposit;
                DepositLog();
            }

            CurrentUserLog();
        }

        public List<string> DisplayItems()
        {
            List<string> allSlotLocations = new List<string>(); // Creat a list for the purpose of testing
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
***********************************
          Items In Stock
***********************************
");
            foreach (KeyValuePair<Item, int> eachItem in inventory)
            {
                if (eachItem.Value == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine($"{eachItem.Key.SlotLocation,-4}{eachItem.Key.Name,-20}{eachItem.Key.Price,-10:c}{eachItem.Value}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else
                {
                    Console.WriteLine($"{eachItem.Key.SlotLocation,-4}{eachItem.Key.Name,-20}{eachItem.Key.Price,-10:c}{eachItem.Value}");
                }
                string itemSlotLocation = eachItem.Key.SlotLocation;
                allSlotLocations.Add(itemSlotLocation);
            }
            Console.ResetColor();
            return allSlotLocations;
        }


        public int EligibleToSelect(string slotLocation) // Can acess/CW ErrorMessage either from the VM class or PurchaseMenu Class
        {
            int n = 0;
            // return n = 0 => item not found
            // return n = 1 => Not enough balance to select this item => "Deposit more money" or "Select another item"
            // return n = 2 => item out of stock
            // return n = 3 => can select

            foreach (KeyValuePair<Item, int> selectedItem in inventory)
            {
                if (!(selectedItem.Key.SlotLocation == slotLocation.ToUpper()))
                {
                    continue;
                }
                else if (selectedItem.Key.SlotLocation == slotLocation.ToUpper())
                {
                    if (Balance >= selectedItem.Key.Price)
                    {
                        if (selectedItem.Value > 0)
                        {
                            n = 3;
                            break;
                        }
                        else
                        {
                            n = 2;
                            ErrorMessage = "Item selected out of stock. Please press ENTER and return to <Purchase Menu> and select again.";
                            break;
                        }
                    }
                    else
                    {
                        n = 1;
                        ErrorMessage = "Not Enough Balance for this Item.  Please press ENTER and return to <Purchase Menu> to deposit more money.";
                        break;
                    }
                }               
            }
            if (n == 0)
            {
                ErrorMessage = "Item Does Not Exist. Please press ENTER and return to <Purchase Menu> and select again.";
            }
            return n;
        }


        public void GenerateSalesReport()
        {
            string fileName = "SalesReport.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            //IOrderedEnumerable<KeyValuePair<Item, int>> sortedSoldItems = from entry in soldItems orderby entry.Value descending select entry;

            using (StreamWriter newReport = new StreamWriter(fullPath, false))
            {
                newReport.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                newReport.WriteLine($"$ Vendo - Matic 800 Sales Report on {DateTime.Now:MM/dd/yyyy hh:mm:ss tt} $");
                newReport.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                newReport.WriteLine();
                foreach (KeyValuePair<Item, int> soldItem in soldItems)
                {
                    newReport.WriteLine($"{soldItem.Key.Name,-20}|{soldItem.Value}");
                    UpToDateSales += soldItem.Key.Price * soldItem.Value;
                }

                newReport.WriteLine();
                newReport.WriteLine($"TOTAL SALE: {UpToDateSales:c}");
            }

        }


        public void ReturnChange() 
        {
            GiveChangeLog();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("$$$$$$$$$$$$$$$$$$$");
            Console.WriteLine();
            if (Balance > 0)
            {
                Console.WriteLine($"Change Due: {Balance:c}");
                int quarters = (int)(Balance / .25M);
                Balance -= quarters * .25M;
                int dimes = (int)(Balance / .1M);
                Balance -= dimes * .1M;
                int nickels = (int)(Balance / .05M);
                Balance -= nickels * .05M;
                int pennies = (int)(Balance / .01M);
                Balance -= pennies * .01M;


                if (quarters > 0)
                {
                    Console.WriteLine($"{"Quarters:",-12}{quarters}");
                }
                if (dimes > 0)
                {
                    Console.WriteLine($"{"Dimes:",-12}{dimes}");
                }
                if (nickels > 0)
                {
                    Console.WriteLine($"{"Nickels:",-12}{nickels}");
                }
                if (pennies > 0)
                {
                    Console.WriteLine($"{"Pennies:",-12}{pennies}");
                }
            }
            else
            {
                Console.WriteLine("No Change Returned.");
            }
            ClearUserLog();
            Console.WriteLine();
            Console.WriteLine("$$$$$$$$$$$$$$$$$$$");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Press ENTER to return to <Purchase Menu>.");
        }


        public void SelectItem(string slotLocation) // Commiting purchase before paying
        {
            string selectionMessage = "";

            foreach (KeyValuePair<Item, int> selectedItem in inventory)
            {
                if (!(selectedItem.Key.SlotLocation == slotLocation.ToUpper()))
                {
                    continue;
                }
                else if (selectedItem.Key.SlotLocation == slotLocation.ToUpper())
                {
                    TotalDue = selectedItem.Key.Price;
                    if (selectedItem.Key.Category.ToLower() == "chip")
                    {
                        //onsole.WriteLine();
                        selectionMessage = "Crunch Crunch, Yum!";
                    }
                    if (selectedItem.Key.Category.ToLower() == "candy")
                    {
                        //Console.WriteLine();
                        selectionMessage = "Munch Munch, Yum!";
                    }
                    if (selectedItem.Key.Category.ToLower() == "drink")
                    {
                        //Console.WriteLine();
                        selectionMessage = "Glug Glug, Yum!";
                    }
                    if (selectedItem.Key.Category.ToLower() == "gum")
                    {
                        //Console.WriteLine();
                        selectionMessage = "Chew Chew, Yum!";
                    }

                    NumberOfItemLeft = selectedItem.Value;
                    NumberOfItemLeft--;
                    inventory[selectedItem.Key] = NumberOfItemLeft;
                    SelectedItem = selectedItem.Key; 
                    PreviousBalance = Balance;  
                    Balance -= TotalDue;  

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(selectionMessage); // Can CW here or creat another SelectionMessage prop in order to access from Purchase Menu
                    Console.WriteLine($"You have purchased {selectedItem.Key.Name} for {selectedItem.Key.Price:c}. Thank you for your business!");
                    Console.ResetColor();

                    UpdateCurrentInventoryLog();
                    SalesLog();
                    AddToSoldItems();
                    GenerateSalesReport();
                }
                break;


            }
            CurrentUserLog();
            Console.WriteLine();
            Console.WriteLine("Press ENTER to return to <Purchase Menu>."); // Can CW here or creat another SelectionMessage prop in order to access from Purchase Menu            
        }


        private void AddToSoldItems()
        {
            int n = 0;
            foreach (KeyValuePair<Item, int> sold in soldItems)
            {
                if (!(sold.Key.Name == SelectedItem.Name))
                {
                    continue;
                }
                else if (sold.Key.Name == SelectedItem.Name)
                {
                    n = soldItems[SelectedItem] + 1;
                    soldItems[SelectedItem] = n;
                    break;
                }
            }
            if (n == 0)
            {
                soldItems.Add(SelectedItem, 1);
            }            
        }


        private void ClearUserLog()
        {
            string fileName = "CurrentUserInfo.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            try
            {
                using (StreamWriter currentUserLog = new StreamWriter(fullPath))
                {                    
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Out of printing paper. Please help us putting more!");
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        private void CurrentUserLog()
        {
            string fileName = "CurrentUserInfo.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            try
            {
                using (StreamWriter currentUserLog = new StreamWriter(fullPath))
                {                    
                    currentUserLog.Write(Balance);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Out of printing paper. Please help us putting more!");
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        private void DepositLog()
        {
            string fileName = "log.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            try
            {
                using (StreamWriter newLog = new StreamWriter(fullPath, true))
                {
                    newLog.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss tt} {"FEED MONEY:",-25} {PreviousBalance,-8:c} {Balance:c}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Out of printing paper. Please help us putting more!");
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        private void GiveChangeLog()
        {
            string fileName = "log.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            try
            {
                using (StreamWriter newLog = new StreamWriter(fullPath, true))
                {
                    newLog.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss tt} {"GIVE CHANGE:",-25} {Balance,-8:c} {0:c}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Out of printing paper. Please help us putting more!");
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        private void LoadInventory(string fullPath, int n)
        {
            //// Check if there is still an exiting user on the current vending maching
           
            try
            {
                using (StreamReader allItemsInTheMachine = new StreamReader(fullPath)) // Read from the data file and add items to the inventory"
                {
                    while (!allItemsInTheMachine.EndOfStream)
                    {
                        Item item = new Item();
                        string[] eachItem = allItemsInTheMachine.ReadLine().Split("|");
                        item.SlotLocation = eachItem[0];
                        item.Name = eachItem[1];
                        item.Price = decimal.Parse(eachItem[2]);
                        item.Category = eachItem[3];
                        if (n == 0)
                        {
                            NumberOfItemLeft = 5;
                        }
                        else
                        {
                            NumberOfItemLeft = Int32.Parse(eachItem[4]);
                        }
                        inventory.Add(item, NumberOfItemLeft);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("OUT OF ORDER. Our deliver truck was lost in traffic.  Please help us relocate it.");
                Console.WriteLine("Please check the path and date file name.");
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        private void ReadCurrentUserLog()
        {
            string fileName = "CurrentUserInfo.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            try
            {
                using (StreamReader currentUserLog = new StreamReader(fullPath))
                {
                    while (!currentUserLog.EndOfStream)
                    {
                        Balance = Decimal.Parse(currentUserLog.ReadLine());
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Out of printing paper. Please help us putting more!");
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        private void ReadPreviousSalesReport()
        {
            string fileName = "SalesReport.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);

            if (!File.Exists(fullPath)) // If the sales report has not been created - creat one
            {
                try
                {
                    using (StreamWriter newLog = new StreamWriter(fullPath))
                    {

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Out of printing paper. Please help us putting more!");
                    Console.WriteLine($"Error: {e.Message}");
                }
            }

            // Read the sales data from the previous record
            try
            {
                using (StreamReader previousLog = new StreamReader(fullPath))
                {
                    while (!previousLog.EndOfStream)
                    {
                        string previousEntry = previousLog.ReadLine();
                        string[] previousSalesRecord = previousEntry.Split("|");
                        foreach (KeyValuePair<Item, int> soldItem in inventory)
                        {
                            if (!(soldItem.Key.Name == previousSalesRecord[0].Trim()))
                            {
                                continue;
                            }
                            else if (soldItem.Key.Name == previousSalesRecord[0].Trim())
                            {
                                soldItems[soldItem.Key] = Int32.Parse(previousSalesRecord[1]);
                            }
                        }
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Previous Sales Record Not Found.");
                Console.WriteLine($"Error: {e.Message}");
            }
        }


        private void SalesLog()
        {
            string fileName = "log.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            try
            {
                using (StreamWriter newLog = new StreamWriter(fullPath, true))
                {
                    newLog.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss tt} {SelectedItem.Name,-20} {SelectedItem.SlotLocation,-4} {PreviousBalance,-8:c} {Balance:c}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Out of printing paper. Please help us putting more!");
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private void UpdateCurrentInventoryLog()
        {
            string fileName = "vendingmachineCurrent.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            try
            {
                using (StreamWriter allItemsInTheMachine = new StreamWriter(fullPath)) // Read from the data file and add items to the inventory"
                {
                    //A1|Potato Crisps|3.05|Chip
                    foreach (KeyValuePair<Item, int> eachitem in inventory)
                    {
                        allItemsInTheMachine.Write(eachitem.Key.SlotLocation + "|");
                        allItemsInTheMachine.Write(eachitem.Key.Name + "|");
                        allItemsInTheMachine.Write(eachitem.Key.Price + "|");
                        allItemsInTheMachine.Write(eachitem.Key.Category + "|");
                        allItemsInTheMachine.WriteLine(eachitem.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OUT OF PAPER.  Please help us reload the paper.");
                Console.WriteLine("Please check the path and date file name.");
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        //private void CleanLog()
        //{
        //    string fileName = "log.txt";
        //    string currentFolder = Environment.CurrentDirectory;
        //    string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);

        //    using (StreamWriter newLog = new StreamWriter(fullPath, false))
        //    {
        //    }
        //}

    }
}
