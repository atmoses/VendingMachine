﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Capstone.Class
{
    public class VendingMachine
    {
        public decimal StartingBalance { get; private set; }

        public decimal Balance { get; set; }

        public string Selection { get; private set; }

        public Item SelectedItem { get; private set; }

        public decimal TotalDue { get; private set; }

        public string ErrorMessage { get; private set; }

        public int NumberOfItemLeft { get; private set; } = 5;
                
        public Dictionary<Item, int> inventory { get; private set; } = new Dictionary<Item, int>();



        public VendingMachine() // "Turn it on!"
        {
            string fileName = "vendingmachine.csv";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
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
                    inventory.Add(item, NumberOfItemLeft);
                }
            }
            
            }
            catch(Exception e)
            {
                Console.WriteLine("OUT OF ORDER. Our deliver truck was lost in traffic.  Please help us relocate it.");
                Console.WriteLine("Please check the path and date file name.");
                Console.WriteLine($"Error: {e.Message}");
            }
        }



        public List<string> DisplayItems()
        {
            List<string> allSlotLocations = new List<string>(); // Creat a list for the purpose of testing
            Console.WriteLine(@"
***************
Items In Stock:
***************
");
            foreach (KeyValuePair<Item, int> eachItem in inventory)
            {
                Console.WriteLine($"{eachItem.Key.SlotLocation,-10}{eachItem.Key.Name,-10}{eachItem.Key.Price,10:c}");
                string itemSlotLocation = eachItem.Key.SlotLocation;
                allSlotLocations.Add(itemSlotLocation);
            }
            return allSlotLocations;
        }



        public decimal Deposit(decimal deposit)
        {
            if (deposit <= 0) // The use must always input a positive decimal number
            {
                ErrorMessage = "The bill you inserted is a counterfeit (negative decimal).  Please insert a real bill (positive decimal).";
                Console.WriteLine(ErrorMessage);
            }
            else
            {
                Balance += deposit;
            }
            
            return Balance;
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
                if(!(selectedItem.Key.SlotLocation == slotLocation))
                {
                    continue;
                }
                else if (selectedItem.Key.SlotLocation == slotLocation)
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
                            ErrorMessage = "Item selected out of stock. Please return to Purchase Menu and select again.";
                            break;
                        }
                    }
                    else
                    {
                        n = 1;
                        ErrorMessage = "Not Enough Balance for this Item.  Please return to Main Menu to deposit more money.";
                        break;
                    }
                }
                //else
                //{
                //    ErrorMessage = "Item Does Not Exist. Please return to Purchase Menu and select again.";
                //}
                
                
            }
            if (n == 0)
            {
                ErrorMessage = "Item Does Not Exist. Please return to Purchase Menu and select again.";
            }
            return n;
        }



        public void SelectItem(string slotLocation) // Commiting purchase before paying
        {
            string selectionMessage = "";

            foreach (KeyValuePair<Item, int> selectedItem in inventory)
            {
                if (!(selectedItem.Key.SlotLocation == slotLocation))
                {
                    continue;
                }
                else if (selectedItem.Key.SlotLocation == slotLocation)
                {
                    TotalDue = selectedItem.Key.Price;
                    if (selectedItem.Key.Category.ToLower() == "chip")
                    {
                        selectionMessage = "Crunch Crunch, Yum!";
                    }
                    if (selectedItem.Key.Category.ToLower() == "candy")
                    {
                        selectionMessage = "Munch Munch, Yum!";
                    }
                    if (selectedItem.Key.Category.ToLower() == "drink")
                    {
                        selectionMessage = "Glug Glug, Yum!";
                    }
                    if (selectedItem.Key.Category.ToLower() == "gum")
                    {
                        selectionMessage = "Chew Chew, Yum!";
                    }
                    NumberOfItemLeft = selectedItem.Value;
                    NumberOfItemLeft--;
                    inventory[selectedItem.Key] = NumberOfItemLeft;
                    SelectedItem = selectedItem.Key;
                    Balance -= TotalDue;
                }
                break;
                
                
            }
            Console.WriteLine(selectionMessage); // Can CW here or creat another SelectionMessage prop in order to access from Purchase Menu            
        }



        public void Checkout() // Empty the SelectedItem Dictionary and "print out" a receipt; 
        {
            try
            {
                Console.WriteLine($"You have purchased {SelectedItem.Name} for {SelectedItem.Price:c}. Thank you for your business!");
                SelectedItem = new Item();

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e.Message} while printing receipt. Checkout failed.");
            }
        }



        public string[] ReturnChange()
        {
            decimal change = Balance - TotalDue;
            int numOfDollars = (int)(change * 100 / 100);
            int centsForQuarters = (int)(change * 100 % 100);
            int numOfQuarters = centsForQuarters / 25;
            int centsForDimes = centsForQuarters % 25;
            int numOfDime = centsForDimes / 10;
            int centsForNickels = centsForDimes % 10;
            int numOfNickels = centsForNickels / 5;
            int cents = centsForNickels % 5;
            string[] changes = { numOfDollars.ToString(), numOfQuarters.ToString(), numOfDime.ToString(), numOfNickels.ToString(), cents.ToString() };
            Console.WriteLine("Returning {change:c}.");
            Balance = 0;
            return changes;
        }
    }
}
