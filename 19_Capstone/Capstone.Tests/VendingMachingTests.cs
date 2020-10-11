using Capstone.Class;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Capstone.Tests
{
    [TestClass]
    public class VendingMachineTests
    {

        [TestMethod]
        public void VMCtorInventoryInitialProductNumTest()
        {
            VendingMachine testMachine = new VendingMachine();
            int expectedTotal = 16;
            int actual = testMachine.inventory.Count;
            Assert.AreEqual(expectedTotal, actual, "There should be 16 items in stock.");
        }

        [TestMethod]

        public void InitialIntevntoryTotalCount()
        {
            VendingMachine vm = new VendingMachine();
            int expectedTotal = 80;
            int actualTotal = 0;
            foreach (KeyValuePair<Item, int> eachItem in vm.inventory)
            {
                actualTotal += eachItem.Value;
            }

            Assert.AreEqual(expectedTotal, actualTotal);
        }

        //*

        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 0)]
        [DataRow(5, 5)]
        [DataRow(10, 10)]
        [DataRow(20, 20)]

        public void VMInitialDepositTest(int actual, int expectedBalance)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit(actual);
            decimal actualBalance = testMachine.Balance;
            Assert.AreEqual((decimal)expectedBalance, (decimal)actualBalance, "The initial balance should be the amount that was deposited.");
        }


        [TestMethod]
        public void VMInitialNegativeDepositTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit(-5);
            decimal actualBalance = testMachine.Balance;
            decimal expectedBalance = 0;
            Assert.AreEqual(expectedBalance, actualBalance, "The initial balance should be the amount that was deposited.");
        }

        [TestMethod]
        public void InitialInventorySlotLocation()
        {
            VendingMachine vm = new VendingMachine();
            List<string> expectedSlotLocation = new List<string> { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4" };
            List<string> actualSlotLocation = new List<string> { };
            foreach (KeyValuePair<Item, int> eachItem in vm.inventory)
            {
                actualSlotLocation.Add(eachItem.Key.SlotLocation);
            }
            CollectionAssert.AreEquivalent(expectedSlotLocation, actualSlotLocation);
        }


        [TestMethod]
        public void VMInitialNegativeDepositMessageTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit(-5);
            string actualMessage = testMachine.ErrorMessage;
            string expectedMessage = "The bill you inserted is a counterfeit (negative decimal).  Please insert a real bill (positive decimal).";
            Assert.AreEqual(expectedMessage, actualMessage, "The initial balance should be the amount that was deposited.");
        }

        //*

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

        public void VMInitialDepoistDataTest(int depoist, int expectedBalance)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit((decimal)depoist);
            decimal actualBalance = testMachine.Balance;
            Assert.AreEqual((decimal)expectedBalance, actualBalance, "The initial balance should be the amount that was deposited.");
        }

        //*

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
        public void VMIDepoistWithPreBalanceTest(double deposit1, double deposit2, double expectedBalance)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit((decimal)deposit1);
            testMachine.Deposit((decimal)deposit2);
            decimal actualBalance = testMachine.Balance;
            Assert.AreEqual((decimal)expectedBalance, actualBalance, "The initial balance should be the amount that was deposited.");
        }

        [TestMethod]
        public void SelectItemTestInventoryNum()
        {
            VendingMachine testMachine = new VendingMachine();
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
            Assert.AreEqual(expectedNumOfItemLeft, actualNumOfItemLeft);

            //Assert.AreEqual

        }

        //*

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
            testMachine.SelectItem(slotLocation);
            string actualTotalDue = testMachine.TotalDue.ToString();
            Assert.AreEqual(expectedTotalDue, actualTotalDue);
        }

        [DataTestMethod]
        [DataRow("A5", "Item Does Not Exist. Please return to Purchase Menu and select again.")]
        [DataRow("A1", "Not Enough Balance for this Item.  Please return to Main Menu to deposit more money.")]

        public void VMEligibleToSelectNotExisisTest(string slotLocation, string expectedErrorMessage)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.EligibleToSelect(slotLocation);
            string actualMessage = testMachine.ErrorMessage;
            Assert.AreEqual(expectedErrorMessage, actualMessage);
        }

        [DataTestMethod]
        [DataRow("C2", "Item selected out of stock. Please return to Purchase Menu and select again.")]
        public void VMEligibleToSelectOFSTest(string slotLocation, string expectedErrorMessage)
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit(100);
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.SelectItem("C2");
            testMachine.EligibleToSelect(slotLocation);
            string actualMessage = testMachine.ErrorMessage;
            Assert.AreEqual(expectedErrorMessage, actualMessage);
        }

        [TestMethod]
        public void CheckOutEmptySelectedItemTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit(100);
            testMachine.SelectItem("C2");
            testMachine.Checkout();
            Item expectedSelectedItem = new Item();
            Item actualSelectedItem = testMachine.SelectedItem;
            Assert.AreEqual(expectedSelectedItem.SlotLocation, actualSelectedItem.SlotLocation);
        }

        [TestMethod]
        public void CheckOutMessageTest()
        {
            VendingMachine testMachine = new VendingMachine();
            testMachine.Deposit(100);
            testMachine.SelectItem("C2");
            testMachine.Checkout();
            string expectedMessage = "You have purchased  for $0.00.";
            string actualMessage = $"You have purchased {testMachine.SelectedItem.Name} for {testMachine.SelectedItem.Price:c}.";
            Assert.AreEqual(expectedMessage, actualMessage);
        }


        [TestMethod]
        public void InitialDisplayItemsTest()
        {
            VendingMachine testMachine = new VendingMachine();
            List<string> expectedList = new List<string> { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", };
            List<String> actualList = testMachine.DisplayItems();
            CollectionAssert.AreEqual(expectedList, actualList);
        }

        //*

        [TestMethod]
        public void CheckIfDepositIsPositive()
        {
            //arrange
            VendingMachine vm = new VendingMachine();

            //act
            vm.Deposit(3);
            decimal actual = vm.Balance;
            //assert
            Assert.AreEqual(3, actual);

        }

        //*

        [TestMethod]
        public void CheckIfNegativeDepostiIsTaken()
        {
            //arrange
            VendingMachine vm = new VendingMachine();

            //act
            vm.Deposit(-1);
            decimal actual = vm.Balance;
            //assert
            Assert.AreEqual(0, actual);

        }

        //*

        [TestMethod]
        public void DepositLessThanZero()
        {
            VendingMachine vm = new VendingMachine();
            vm.Deposit(-1);
            string expected = "The bill you inserted is a counterfeit (negative decimal).  Please insert a real bill (positive decimal).";
            string actual = vm.ErrorMessage;
            Assert.AreEqual(expected, actual);
        }
    }
}
