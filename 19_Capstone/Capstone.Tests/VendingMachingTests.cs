using Capstone.Class;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks.Sources;

namespace Capstone.Tests
{
    [TestClass]
    public class VendingMachineTests
    {

        [TestMethod]
        public void InventoryInitialProductNumTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            int expectedTotal = 16;
            int actual = testMachine.inventory.Count;
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual(expectedTotal, actual, "There should be 16 items in stock.");
        }


        [TestMethod]
        public void InitialIntevntoryTotalCount()
        {
            VendingMachine vm = new VendingMachine();
            vm.Checkout();
            vm.ReturnChange();
            int expectedTotal = 80;
            int actualTotal = 0;
            foreach (KeyValuePair<Item, int> eachItem in vm.inventory)
            {
                actualTotal += eachItem.Value;
            }
            vm.Checkout();
            vm.ReturnChange();
            Assert.AreEqual(expectedTotal, actualTotal);
        }


        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 0)]
        [DataRow(5, 5)]
        [DataRow(10, 10)]
        [DataRow(20, 20)]

        public void InitialDepositTest(int actual, int expectedBalance)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit(actual);
            decimal actualBalance = testMachine.Balance;
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual((decimal)expectedBalance, (decimal)actualBalance, "The initial balance should be the amount that was deposited.");
        }


        [TestMethod]
        public void InitialNegativeDepositTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit(-5);
            decimal actualBalance = testMachine.Balance;
            decimal expectedBalance = 0;
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual(expectedBalance, actualBalance, "The initial balance should be the amount that was deposited.");
        }


        [TestMethod]
        public void InitialInventorySlotLocation()
        {
            VendingMachine vm = new VendingMachine();
            vm.Checkout();
            vm.ReturnChange();
            List<string> expectedSlotLocation = new List<string> { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4" };
            List<string> actualSlotLocation = new List<string> { };
            foreach (KeyValuePair<Item, int> eachItem in vm.inventory)
            {
                actualSlotLocation.Add(eachItem.Key.SlotLocation);
            }
            vm.Checkout();
            vm.ReturnChange();
            CollectionAssert.AreEquivalent(expectedSlotLocation, actualSlotLocation);
        }


        [TestMethod]
        public void InitialNegativeDepositMessageTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit(-5);
            string actualMessage = testMachine.ErrorMessage;
            string expectedMessage = "The bill you inserted is a counterfeit (negative decimal).  Please insert a real bill (positive decimal).";
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual(expectedMessage, actualMessage, "The initial balance should be the amount that was deposited.");
        }

        
        [DataTestMethod]
        [DataRow(9, 9)]
        [DataRow(10, 10)]
        [DataRow(7, 7)]
        [DataRow(0, 0)]
        [DataRow(8, 8)]
        [DataRow(6, 6)]
        [DataRow(5, 5)]
        [DataRow(1, 1)]
        [DataRow(3, 3)]
        [DataRow(2, 2)]

        public void InitialDepoistDataTest(int depoist, int expectedBalance)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit((decimal)depoist);
            decimal actualBalance = testMachine.Balance;
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual((decimal)expectedBalance, actualBalance, "The initial balance should be the amount that was deposited.");
        }


        [DataTestMethod]
        [DataRow(5, 5, 10)]
        [DataRow(8, 9.76, 17.76)]
        [DataRow(8, -5, 8)]
        [DataRow(8, -3, 8)]
        [DataRow(3, 5, 8)]
        [DataRow(2.25, 2.25, 4.50)]
        [DataRow(7, 3.5, 10.5)]
        [DataRow(8, 3, 11)]
        [DataRow(1.30, 1.56, 2.86)]
        [DataRow(7, 0, 7)]
        public void DepoistWithPreBalanceTest(double deposit1, double deposit2, double expectedBalance)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit((decimal)deposit1);
            testMachine.Deposit((decimal)deposit2);
            decimal actualBalance = testMachine.Balance;
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual((decimal)expectedBalance, actualBalance, "The initial balance should be the amount that was deposited.");
        }


        [TestMethod]
        public void SelectItemTestInventoryNum()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            string slotLocation = "A1";
            int expectedNumOfItemLeft = 4;
            testMachine.SelectItem(slotLocation);
            int actualNumOfItemLeft = 0;
            foreach (KeyValuePair<Item, int> item in testMachine.inventory)
            {
                if (item.Key.SlotLocation == slotLocation)
                {
                    actualNumOfItemLeft = item.Value;
                }
            }
            Assert.AreEqual(expectedNumOfItemLeft, actualNumOfItemLeft);

            //testMachine = new VendingMachine();
            slotLocation = "B4";
            expectedNumOfItemLeft = 3;
            testMachine.SelectItem(slotLocation);
            testMachine.SelectItem(slotLocation);
            foreach (KeyValuePair<Item, int> item in testMachine.inventory)
            {
                if (item.Key.SlotLocation == slotLocation)
                {
                    actualNumOfItemLeft = item.Value;
                }
            }
            Assert.AreEqual(expectedNumOfItemLeft, actualNumOfItemLeft);

            slotLocation = "C3";
            expectedNumOfItemLeft = 0;
            testMachine.SelectItem(slotLocation);
            testMachine.SelectItem(slotLocation);
            testMachine.SelectItem(slotLocation);
            testMachine.SelectItem(slotLocation);
            testMachine.SelectItem(slotLocation);
            foreach (KeyValuePair<Item, int> item in testMachine.inventory)
            {
                if (item.Key.SlotLocation == slotLocation)
                {
                    actualNumOfItemLeft = item.Value;
                }
            }
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual(expectedNumOfItemLeft, actualNumOfItemLeft);


            //Assert.AreEqual

        }
                

        [DataTestMethod]
        [DataRow("A1", "3.05")]
        [DataRow("D3", "0.75")]
        [DataRow("A2", "1.45")]
        [DataRow("B1", "1.80")]
        [DataRow("D4", "0.75")]
        [DataRow("A4", "3.65")]
        [DataRow("C4", "1.50")]
        [DataRow("B3", "1.50")]
        [DataRow("B4", "1.75")]
        [DataRow("D1", "0.85")]
        [DataRow("A3", "2.75")]
        [DataRow("C3", "1.50")]
        public void SelectItemTotalDueTest(string slotLocation, string expectedTotalDue)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.SelectItem(slotLocation);
            string actualTotalDue = testMachine.TotalDue.ToString();
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual(expectedTotalDue, actualTotalDue);
            string currentFolder = Environment.CurrentDirectory;
            string path = Path.Combine(currentFolder, @"..\..\..\..\");
            File.Delete(Path.Combine(path, "vendingmachineCurrent.txt"));
            File.Delete(Path.Combine(path, "CurrentUserInfo.txt"));
        }


        [DataTestMethod]
        [DataRow("A5", "Item Does Not Exist. Please press ENTER and return to <Purchase Menu> and select again.")]
        [DataRow("a1", "Not Enough Balance for this Item.  Please press ENTER and return to <Purchase Menu> to deposit more money.")]

        public void EligibleToSelectNotExisisTest(string slotLocation, string expectedErrorMessage)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit(1);
            testMachine.EligibleToSelect(slotLocation);
            string actualMessage = testMachine.ErrorMessage;
            testMachine.Checkout();
            testMachine.ReturnChange();
            Assert.AreEqual(expectedErrorMessage, actualMessage);
        }


        [DataTestMethod]
        [DataRow("c2", "Item selected out of stock. Please press ENTER and return to <Purchase Menu> and select again.")]
        public void EligibleToSelectOFSTest(string slotLocation, string expectedErrorMessage)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit(100);
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.EligibleToSelect(slotLocation);
            string actualMessage = testMachine.ErrorMessage;
            testMachine.Checkout();
            testMachine.ReturnChange();
            string currentFolder = Environment.CurrentDirectory;
            string path = Path.Combine(currentFolder, @"..\..\..\..\");
            File.Delete(Path.Combine(path, "vendingmachineCurrent.txt"));
            File.Delete(Path.Combine(path, "CurrentUserInfo.txt"));
            Assert.AreEqual(expectedErrorMessage, actualMessage);
        }


        [TestMethod]
        public void CheckOutEmptySelectedItemTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit(100);
            testMachine.SelectItem("C2");
            testMachine.Checkout();
            Item expectedSelectedItem = new Item();
            Item actualSelectedItem = testMachine.SelectedItem;
            testMachine.Checkout();
            testMachine.ReturnChange();
            string currentFolder = Environment.CurrentDirectory;
            string path = Path.Combine(currentFolder, @"..\..\..\..\");
            File.Delete(Path.Combine(path, "vendingmachineCurrent.txt"));
            File.Delete(Path.Combine(path, "CurrentUserInfo.txt"));
            Assert.AreEqual(expectedSelectedItem.SlotLocation, actualSelectedItem.SlotLocation);
        }


        [TestMethod]
        public void CheckOutMessageTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            testMachine.Deposit(100);
            testMachine.SelectItem("C2");
            testMachine.Checkout();
            string expectedMessage = "You have purchased  for $0.00.";
            string actualMessage = $"You have purchased {testMachine.SelectedItem.Name} for {testMachine.SelectedItem.Price:c}.";
            testMachine.Checkout();
            testMachine.ReturnChange();
            string currentFolder = Environment.CurrentDirectory;
            string path = Path.Combine(currentFolder, @"..\..\..\..\");
            File.Delete(Path.Combine(path, "vendingmachineCurrent.txt"));
            File.Delete(Path.Combine(path, "CurrentUserInfo.txt"));
            Assert.AreEqual(expectedMessage, actualMessage);
        }


        [TestMethod]
        public void InitialDisplayItemsTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Checkout();
            testMachine.ReturnChange();
            List<string> expectedList = new List<string> { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", };
            List<String> actualList = testMachine.DisplayItems();
            testMachine.Checkout();
            testMachine.ReturnChange();
            CollectionAssert.AreEqual(expectedList, actualList);
        }


        [TestMethod]
        public void CheckIfDepositIsPositive()
        {
            //arrange
            VendingMachine vm = new VendingMachine();
            vm.Checkout();
            vm.ReturnChange();

            //act
            vm.Deposit(3);
            decimal actual = vm.Balance;
            //assert
            vm.Checkout();
            vm.ReturnChange();
            Assert.AreEqual(3, actual);

        }


        [TestMethod]
        public void CheckIfNegativeDepostiIsTaken()
        {
            //arrange
            VendingMachine vm = new VendingMachine();
            vm.Checkout();
            vm.ReturnChange();

            //act
            vm.Deposit(-1);
            decimal actual = vm.Balance;
            //assert
            vm.Checkout();
            vm.ReturnChange();
            Assert.AreEqual(0, actual);

        }


        [TestMethod]
        public void DepositLessThanZero()
        {
            VendingMachine vm = new VendingMachine();
            vm.Checkout();
            vm.ReturnChange();
            vm.Deposit(-1);
            string expected = "The bill you inserted is a counterfeit (negative decimal).  Please insert a real bill (positive decimal).";
            string actual = vm.ErrorMessage;
            vm.Checkout();
            vm.ReturnChange();
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void MenuPrintSalesReportTest()
        {
            VendingMachine vm = new VendingMachine();
            string fileName = "SalesReport.txt";
            string currentFolder = Environment.CurrentDirectory;
            string fullPath = Path.Combine(currentFolder, @"..\..\..\..\", fileName);
            bool actualExist = (File.Exists(fullPath));
            bool expectExist = true;
            Assert.AreEqual(expectExist, actualExist);
            currentFolder = Environment.CurrentDirectory;
            string path = Path.Combine(currentFolder, @"..\..\..\..\");
            File.Delete(Path.Combine(path, "vendingmachineCurrent.txt"));
            File.Delete(Path.Combine(path, "CurrentUserInfo.txt"));
        }                
    }
}
